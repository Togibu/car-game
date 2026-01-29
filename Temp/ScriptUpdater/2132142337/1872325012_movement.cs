
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 15f;
    public float acceleration = 4.5f;
    public float brakeForce   = 8f;

    [Header("Turning")]
    public float turnSpeed = 45f; // deg/s

    [Header("Physics")]
    public float drag          = 0.5f;
    public float angularDrag   = 1.0f;

    private Rigidbody rb;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping        = drag;
        rb.angularDamping = angularDrag;
    }

    void FixedUpdate()
    {
        // Ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down,
                                     1.0f + 0.5f, LayerMask.GetMask("Ground"));

        // Input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Drive forward/backward
        if (isGrounded)
        {
            float target = v * maxSpeed;
            float current = Vector3.Dot(rb.linearVelocity, transform.forward);
            float diff = target - current;
            float accel = (v == 0) ? -brakeForce : acceleration;
            float move = Mathf.Clamp(diff, -accel * Time.fixedDeltaTime,
                                     accel * Time.fixedDeltaTime);
            rb.AddForce(transform.forward * move, ForceMode.VelocityChange);
        }

        // Turning
        float currentSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        if (Mathf.Abs(currentSpeed) > 0.1f && Mathf.Abs(h) > 0.01f)
        {
            float turnDir = (currentSpeed > 0) ? h : -h;
            float turn    = turnDir * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
        }
    }
}
