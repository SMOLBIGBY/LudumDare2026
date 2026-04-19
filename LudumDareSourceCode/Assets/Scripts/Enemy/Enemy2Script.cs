using System.Collections;
using UnityEngine;

public class Enemy2PatrolScript : MonoBehaviour, IActivatable
{
    [Header("Movement Settings")]
    public float stepDistance = 1f;
    public float moveInterval = 0.7f;

    [Header("Step Amounts")]
    public int leftSteps = 2;
    public int rightSteps = 3;
    public int upSteps = 0;
    public int downSteps = 0;

    bool DisableForEternity = false;

    bool isDying = false;

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    [Header("Patrol Order")]
    public Direction[] patrolOrder;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    StateManager stateManager;

    [SerializeField] bool HasSignal = false;

    private Vector2 lastDirection = Vector2.down;
    private bool isMoving = false;
    private int currentDirectionIndex = 0;
    private int currentStepIndex = 0;

    private void OnEnable() => RobotManagerScript.OnMyFunction += ChangeSignal;
    private void OnDisable() => RobotManagerScript.OnMyFunction -= ChangeSignal;

    void Start()
    {
        HasSignal = false;
        stateManager = FindObjectOfType<StateManager>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    { 
        if (DisableForEternity)
        {
            HasSignal = false;
        }
    }

    private bool patrolStarted = false;

    void ChangeSignal()
    {
        Debug.Log("Enemy1 Received Signal!");
        if (!HasSignal)
        {
            HasSignal = true;

            if (!patrolStarted)
            {
                patrolStarted = true;
                StartCoroutine(PatrolRoutine(0.1f));
            }
        }
        else
        {
            HasSignal = false;
        }
    }


    IEnumerator PatrolRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        while (true)
        {
            yield return new WaitUntil(() => HasSignal);

            // Continue from where we left off
            while (HasSignal)
            {
                Direction dir = patrolOrder[currentDirectionIndex];
                int steps = GetSteps(dir);

                while (currentStepIndex < steps)
                {
                    yield return StartCoroutine(SmoothMove(GetVector(dir)));

                    currentStepIndex++;

                    // Instead of killing coroutine, just wait
                    if (!HasSignal)
                        yield return new WaitUntil(() => HasSignal);
                }

                // Reset step index and move to next direction
                currentStepIndex = 0;
                currentDirectionIndex = (currentDirectionIndex + 1) % patrolOrder.Length;
            }
        }
    }

    IEnumerator SmoothMove(Vector2 direction)
    {
        if (!isDying)
        {
            if (HasSignal)
            {
                isMoving = true;
                HandleAnimations(direction);

                Vector3 startPos = transform.position;
                Vector3 endPos = startPos + (Vector3)(direction * stepDistance);

                float elapsed = 0f;

                while (elapsed < moveInterval)
                {
                    float t = Mathf.SmoothStep(0f, 1f, elapsed / moveInterval);
                    transform.position = Vector3.Lerp(startPos, endPos, t);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                transform.position = endPos;
                lastDirection = direction;

                HandleAnimations(Vector2.zero);
                isMoving = false;
            }
        }
    }

    int GetSteps(Direction dir)
    {
        switch (dir)
        {
            case Direction.Left: return leftSteps;
            case Direction.Right: return rightSteps;
            case Direction.Up: return upSteps;
            case Direction.Down: return downSteps;
        }
        return 0;
    }

    Vector2 GetVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.Left: return Vector2.left;
            case Direction.Right: return Vector2.right;
            case Direction.Up: return Vector2.up;
            case Direction.Down: return Vector2.down;
        }
        return Vector2.zero;
    }

    void HandleAnimations(Vector2 direction)
    {
        if (HasSignal)
        {
            if (direction != Vector2.zero)
            {
                if (direction == Vector2.right)
                {
                    animator.SetTrigger("Walk_E");
                    spriteRenderer.flipX = false;
                }
                else if (direction == Vector2.left)
                {
                    animator.SetTrigger("Walk_E");
                    spriteRenderer.flipX = true;
                }
                else if (direction == Vector2.up)
                    animator.SetTrigger("Walk_N");
                else if (direction == Vector2.down)
                    animator.SetTrigger("Walk_S");
            }
            else
            {
                if (lastDirection == Vector2.right)
                {
                    animator.SetTrigger("Idle_E");
                    spriteRenderer.flipX = false;
                }
                else if (lastDirection == Vector2.left)
                {
                    animator.SetTrigger("Idle_E");
                    spriteRenderer.flipX = true;
                }
                else if (lastDirection == Vector2.up)
                    animator.SetTrigger("Idle_N");
                else if (lastDirection == Vector2.down)
                    animator.SetTrigger("Idle_S");
            }
        }
        else
        {
            animator.SetTrigger("NotActive");
        }
    }

    void OnTriggerEnter2D(Collider2D wall)
    {
        if (wall.CompareTag("Wall") || wall.CompareTag("Enemy"))
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        Debug.Log("Enemy1 Died!");
        if (!isDying)
        {
            animator.SetTrigger("Die");
        }
        isDying = true;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        patrolStarted = false; // Reset patrol state in case we want to respawn this enemy later
    }

    public void Activate()
    {
        Debug.Log("BugFrozen!");
        DisableForEternity = true;
    }
}