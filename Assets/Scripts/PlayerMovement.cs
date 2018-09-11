using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] float speed;
    [SerializeField] Transform transform;
    [SerializeField] Vector3 up;
    [SerializeField] float angleOffset;
    [SerializeField] float rotationThresholdVelocity;

    [Header("Movement Sanitation")]
    [SerializeField] Vector2 rayOffset;
    [SerializeField] float movementDirectionRayLength;

    Vector2 previousFramePosition;
    public Vector2 origin;

    public delegate void Move(float var);
    public static event Move OnMove;

    void OnEnable() {
        PlayerInput.OnMovement += ProcessMovementInput;
        PlayerLand.OnLandingStateChange += ToggleMovement;
    }

    void OnDisable() {
        PlayerInput.OnMovement -= ProcessMovementInput;
        PlayerLand.OnLandingStateChange -= ToggleMovement;
    }


    void Start() => previousFramePosition = transform.position;
    void Update() {
        FaceMovementDirection();
        DebugRays();
    }

    void FaceMovementDirection() {
        if (rigidbody2D.velocity.magnitude < rotationThresholdVelocity) return;
        transform.rotation = Quaternion.AngleAxis
            (MovementAngle
                 (MovementVelocity()) + angleOffset, up);
    }

    Vector2 MovementVelocity() => rigidbody2D.velocity;
    float MovementAngle(Vector2 dir) => Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

    void ProcessMovementInput(Vector2 input) => ApplyMovement(SanitizeInput(input));

    Vector2 SanitizeInput(Vector2 input) {
        return input;
    }

    void DebugRays() {
        origin = transform.position + (transform.up * rayOffset.y) + (transform.right * rayOffset.x);
        Debug.DrawRay(origin, transform.up * movementDirectionRayLength, Color.red);
        origin = transform.position +  (transform.up * rayOffset.y) - (transform.right * rayOffset.x);
        Debug.DrawRay(origin,transform.up * movementDirectionRayLength, Color.red);
        origin = transform.position + (transform.up * rayOffset.y);
        Debug.DrawRay(origin, transform.up * movementDirectionRayLength, Color.red);
    }

    void ApplyMovement(Vector2 input) {
        rigidbody2D.AddForce(input * speed * Time.deltaTime);
        OnMove?.Invoke(Mathf.Abs(Vector2.Distance(transform.position, previousFramePosition)));
        previousFramePosition = transform.position;
    }

    void ToggleMovement(bool onGround) {
        enabled = !onGround;
        StopRigidbodyOnLanding();
    }

    void StopRigidbodyOnLanding() {
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.angularVelocity = 0f;
    }
}
