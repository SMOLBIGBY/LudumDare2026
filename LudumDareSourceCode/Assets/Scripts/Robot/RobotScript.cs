using System.Collections;
using UnityEngine;

public class RobotScript : MonoBehaviour, IActivatable
{
    public Vector2? currentCommand = null;

    private float step = 1f;
    private float moveInterval = 0.7f;

    public bool isMoving = false;

    private Vector2 lastDirection = Vector2.down;

    public bool HasSignal = true;

    Coroutine processRoutine;

    private GhostRobotScript ghost;
    private StateManager stateManager;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private CommandsTextBoxScript uiScript;

    public bool hasMadeFirstMove = false;

    private bool stopRequested = false;

    void Start()
    {
        ghost = FindObjectOfType<GhostRobotScript>();
        stateManager = FindObjectOfType<StateManager>();
        uiScript = FindObjectOfType<CommandsTextBoxScript>();

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!HasSignal)
        {
            stateManager.HasStopped = true;
        }

        if (stateManager.HasWon)
            spriteRenderer.enabled = false;
    }

    private void OnEnable() => RobotManagerScript.OnMyFunction += StartRobot;
    private void OnDisable() => RobotManagerScript.OnMyFunction -= StartRobot;

    void StartRobot()
    {
        hasMadeFirstMove = false;

        if (!stateManager.HasStarted) return;

        stopRequested = false;
        processRoutine = StartCoroutine(ProcessCommands());
    }

    IEnumerator ProcessCommands()
    {
        yield return new WaitForSeconds(0.5f);

        while (ghost.commandList.Count > 0)
        {
            if (stateManager.HasStopped || stopRequested)
                yield break;

            Vector2 cmd = ghost.commandList[0];

            currentCommand = cmd;
            uiScript?.UpdateCommandText();

            yield return StartCoroutine(SmoothMove(cmd));

            if (ghost.commandList.Count > 0)
                ghost.commandList.RemoveAt(0);

            currentCommand = null;
            uiScript?.UpdateCommandText();

            hasMadeFirstMove = true;

            if (stateManager.HasStopped || stopRequested)
                yield break;
        }
    }

    public void StopProcessing()
    {
        stopRequested = true;

        if (processRoutine != null)
        {
            StopCoroutine(processRoutine);
            processRoutine = null;
        }

        isMoving = false;
        currentCommand = null;
    }

    IEnumerator SmoothMove(Vector2 direction)
    {
        isMoving = true;
        HandleAnimations(direction);

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)(direction * step);

        float elapsed = 0f;

        while (elapsed < moveInterval)
        {
            if (stateManager.HasStopped || stopRequested)
            {
                isMoving = false;
                HandleAnimations(Vector2.zero);
                yield break;
            }

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

    void HandleAnimations(Vector2 direction)
    {
        if (!stateManager.HasStopped)
        {
            if (direction != Vector2.zero)
            {
                if (direction == Vector2.right) { animator.SetTrigger("Walk_E"); spriteRenderer.flipX = false; }
                else if (direction == Vector2.left) { animator.SetTrigger("Walk_E"); spriteRenderer.flipX = true; }
                else if (direction == Vector2.up) animator.SetTrigger("Walk_N");
                else if (direction == Vector2.down) animator.SetTrigger("Walk_S");
            }
            else
            {
                if (lastDirection == Vector2.right) { animator.SetTrigger("Idle_E"); spriteRenderer.flipX = false; }
                else if (lastDirection == Vector2.left) { animator.SetTrigger("Idle_E"); spriteRenderer.flipX = true; }
                else if (lastDirection == Vector2.up) animator.SetTrigger("Idle_N");
                else if (lastDirection == Vector2.down) animator.SetTrigger("Idle_S");
            }
        }
        else
        {
            animator.SetTrigger("NotActive");
        }

        if (!stateManager.HasStarted)
        {
            animator.SetTrigger("NotActive");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Enemy"))
        {
            Die();
        }
    }

    void Die()
    {
        stateManager.HadDied = true;
        stateManager.HasStopped = true;
        Debug.Log("Robot has died!");
    }

    public void Activate()
    {
        Debug.Log("BugFrozen!");
        HasSignal = false;
        StartCoroutine(LoseTimer());

    }

    IEnumerator LoseTimer()
    {
        yield return new WaitForSeconds(3f);
        stateManager.HadDied = true;
    }
}