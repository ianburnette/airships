using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] float speed;
    [SerializeField] Transform transform;
    [SerializeField] Vector3 up;
    [SerializeField] float angleOffset;

    Vector2 previousFramePosition;

    public delegate void Move(float var);
    public static event Move OnMove;

    void OnEnable() {
        PlayerInput.OnMovement += ApplyMovement;
        PlayerLand.OnLandingStateChange += ToggleMovement;
    }

    void OnDisable() {
        PlayerInput.OnMovement -= ApplyMovement;
        PlayerLand.OnLandingStateChange -= ToggleMovement;
    }


    void Start() => previousFramePosition = transform.position;
    void Update() => FaceMovementDirection();

    void FaceMovementDirection() => transform.rotation = Quaternion.AngleAxis
                                        (MovementAngle
                                             (MovementDirection()) + angleOffset, up);
    Vector2 MovementDirection() => rigidbody2D.velocity;
    float MovementAngle(Vector2 dir) => Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

    void ApplyMovement(Vector2 input) {
        rigidbody2D.AddForce(input * speed * Time.deltaTime);
        OnMove?.Invoke(Mathf.Abs(Vector2.Distance(transform.position, previousFramePosition)));
        previousFramePosition = transform.position;
    }

    void ToggleMovement(bool onGround) => enabled = !onGround;
}
