using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class FollowCamera2D : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    public float followSpeed = 5f;
    public Vector2 offset = Vector2.zero;

    [Header("Bounds Settings")]
    public PolygonCollider2D[] boundsColliders;
    public bool EnableBounds = true;

    [Header("Camera Settings")]
    public float targetWidth = 1f; // dynamic width for non-boss states

    [Header("Bossfight Zoom")]
    public float startSize = 5.31f;
    public float targetSize = 6f;
    public float zoomSpeed = 1.5f;

    private Camera cam;
    private float halfWidth;
    private float halfHeight;

    private Vector3 shakeOffset = Vector3.zero;
    private Tween shakeTween;

    private Bounds combinedBounds;


    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = startSize;

        if (boundsColliders != null && boundsColliders.Length > 0)
            RecalculateCombinedBounds();
    }

    private void Start()
    {


    }

    private void Update()
    {
        UpdateCameraSize();
        UpdateTarget();
        UpdateBounds();
        FollowTarget();
    }

    // -----------------------------
    // CAMERA SIZE LOGIC
    // -----------------------------
    private void UpdateCameraSize()
    {
        //if (_stateManager != null && _stateManager.IsBossfight && _stateManager.IsFishing)
        //{
        //    // Smooth zoom to bossfight size
        //    cam.orthographicSize = Mathf.Lerp(
        //        cam.orthographicSize,
        //        targetSize,
        //        Time.deltaTime * zoomSpeed
        //    );
        //}
        //else
        //{
        //    // Dynamic width adjustment
        //    float windowAspect = (float)Screen.width / Screen.height;
        //    float baseSize = targetWidth / 2f;

        //    cam.orthographicSize = windowAspect < 1f
        //        ? targetWidth / windowAspect / 2f
        //        : baseSize;
        //}

        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    // -----------------------------
    // FOLLOW TARGET LOGIC
    // -----------------------------
    private void UpdateTarget()
    {
        //target = FindObjectOfType<PlayerController>()?.transform;

    }

    private void FollowTarget()
    {
        if (target == null)
            return;

        Vector3 targetPos = target.position;
        if (target.TryGetComponent<Rigidbody2D>(out var rb))
            targetPos = rb.position;

        Vector3 desiredPos = targetPos + (Vector3)offset;
        desiredPos.z = transform.position.z;

        Vector3 smoothedPos = Vector3.Lerp(
            transform.position - shakeOffset,
            desiredPos,
            followSpeed * Time.deltaTime
        );

        if (EnableBounds)
            smoothedPos = ClampToBounds(smoothedPos);

        transform.position = smoothedPos + shakeOffset;
    }

    // -----------------------------
    // BOUNDS LOGIC
    // -----------------------------
    private void UpdateBounds()
    {
        if (EnableBounds)
            RecalculateCombinedBounds();
    }

    private Vector3 ClampToBounds(Vector3 position)
    {
        if (boundsColliders == null || boundsColliders.Length == 0)
            return position;

        Bounds chosen = default;
        bool inside = false;

        foreach (var col in boundsColliders)
        {
            if (col.OverlapPoint(position))
            {
                chosen = col.bounds;
                inside = true;
                break;
            }
        }

        if (!inside)
        {
            float closest = float.MaxValue;

            foreach (var col in boundsColliders)
            {
                float dist = Vector2.Distance(position, col.bounds.center);

                if (dist < closest)
                {
                    closest = dist;
                    chosen = col.bounds;
                }
            }
        }

        float minX = chosen.min.x + halfWidth;
        float maxX = chosen.max.x - halfWidth;
        float minY = chosen.min.y + halfHeight;
        float maxY = chosen.max.y - halfHeight;

        return new Vector3(
            Mathf.Clamp(position.x, minX, maxX),
            Mathf.Clamp(position.y, minY, maxY),
            position.z
        );
    }

    private void RecalculateCombinedBounds()
    {
        if (boundsColliders == null || boundsColliders.Length == 0)
            return;

        combinedBounds = boundsColliders[0].bounds;
        for (int i = 1; i < boundsColliders.Length; i++)
            combinedBounds.Encapsulate(boundsColliders[i].bounds);
    }

    // -----------------------------
    // SHAKE
    // -----------------------------
    public void Shake(float duration = 0.5f, float strength = 0.5f)
    {
        if (shakeTween != null && shakeTween.IsActive())
            shakeTween.Kill();

        shakeTween = DOTween.To(() => shakeOffset, x => shakeOffset = x, Vector3.zero, duration)
            .SetEase(Ease.OutQuad)
            .OnStart(() => shakeOffset = Random.insideUnitCircle * strength)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
