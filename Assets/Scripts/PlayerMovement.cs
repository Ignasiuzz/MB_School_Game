using UnityEngine;

public class PlayerMovementMobile : MonoBehaviour {
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float deadZonePixels = 10f; // below this = no move, this or more = move
    public DynamicJoystick joystick;   // still used to know when the joystick is active

    [Header("Debug")]
    public bool debugLogs = true;
    public float logInterval = 0.1f;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isHolding;
    private float clickStartTime;
    private Vector2 clickStartPos;
    private float nextLogTime;
    private Vector2 lastDirection = Vector2.down; // remembered facing

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update() {
        // Click started (ignore if joystick is hidden, e.g. a panel is open)
        if (Input.GetMouseButtonDown(0) && joystick != null && joystick.gameObject.activeInHierarchy) {
            isHolding = true;
            clickStartTime = Time.unscaledTime;
            clickStartPos = Input.mousePosition;
            nextLogTime = 0f;

            if (debugLogs)
                Debug.Log($"[Move] Click started at screen position {clickStartPos}");
        }

        Vector2 delta = Vector2.zero;
        float pixelDistance = 0f;
        float elapsed = 0f;

        if (isHolding) {
            delta = (Vector2)Input.mousePosition - clickStartPos;
            pixelDistance = delta.magnitude;
            elapsed = Time.unscaledTime - clickStartTime;
        }

        // 9px = no move, 10px = move (inclusive of 10)
        bool isWalking = isHolding && Input.GetMouseButton(0) && pixelDistance >= deadZonePixels;

        if (isWalking) {
            Vector2 dir = delta.normalized;
            lastDirection = dir;

            rb.linearVelocity = dir * moveSpeed;

            animator.SetBool("isWalking", true);
            animator.SetFloat("InputX", dir.x);
            animator.SetFloat("InputY", dir.y);
        } else {
            rb.linearVelocity = Vector2.zero;

            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", lastDirection.x);
            animator.SetFloat("LastInputY", lastDirection.y);
        }

        if (debugLogs && isHolding) {
            if (Input.GetMouseButton(0) && Time.unscaledTime >= nextLogTime) {
                Debug.Log($"[Move] Time since click: {elapsed:F2}s | Pixels from click point: {pixelDistance:F1}px | Moving: {isWalking} | Speed: {rb.linearVelocity.magnitude:F1}");
                nextLogTime = Time.unscaledTime + logInterval;
            }

            if (Input.GetMouseButtonUp(0))
                Debug.Log($"[Move] Click released after {elapsed:F2}s | Final pixels from click point: {pixelDistance:F1}px");
        }

        if (Input.GetMouseButtonUp(0))
            isHolding = false;
    }
}