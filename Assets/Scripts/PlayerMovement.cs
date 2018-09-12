using System.Collections;
using System.Collections.Generic;
using ProBuilder2.Common;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] float speed;
    [SerializeField] Transform transform;
    [SerializeField] Vector3 up;
    [SerializeField] float angleOffset;
    [SerializeField] float rotationThresholdVelocity;
    [SerializeField] float angleSmoothSpeed;

    [Header("Movement Sanitation")]
    [SerializeField] Vector2 rayOffset;
    [SerializeField] float movementDirectionRayLength;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] float[] distances;
    [SerializeField] float distanceSimilarityMargin;

    Vector2 previousFramePosition;
    public Vector2 origin;

    public Vector2 mostRecentInput;

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
            FaceInputDirection();
      //  DebugRays();
    }

    void FaceInputDirection() {
        transform.rotation = SmoothedRotation
            (Quaternion.AngleAxis(MovementAngle(mostRecentInput) + angleOffset, up));
    }

    Quaternion SmoothedRotation(Quaternion inputRotation) =>
        Quaternion.Lerp(transform.rotation, inputRotation, angleSmoothSpeed * Time.deltaTime);
    Vector2 MovementVelocity() => rigidbody2D.velocity;
    float MovementAngle(Vector2 dir) => Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

    void ProcessMovementInput(Vector2 input) {
        ApplyMovement(SanitizeInput(input.normalized));
        mostRecentInput = input;
    }

    Vector2 SanitizeInput(Vector2 input) {
        //
        while (!InputAcceptable(input))
//        var distances = CastRays(GetRays(), input);

        ProcessDistances(input, distances);
        return input;
    }

    bool InputAcceptable(Vector2 input) {
        return true;
    }

    float[] CastRays(Ray[] rays, Vector2 input) {
        for (var i = 0; i < 3; i++) distances[i] = Cast(rays[i]);
        return distances;
    }

    Vector2 ProcessDistances(Vector2 input, float[] distances) {
        // 0 = forward, 1 = left, 2 = right
        //if (distances[1] > 0f && distances [0] > 0f)
        //    // wall to the left
        //if (distances[2] > 0f && distances[0] > 0f)
        //    //wall to the right
        //if (((distances[0] + distances[1] + distances[2]) / 3) < distanceSimilarityMargin)
        //    //heading towards wall
        //    if (distances[0] > 0 && distances[1] > 0 && distances[2] > 0)
                //facing wall
        return input;
    }

    float Cast(Ray ray) {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(ray.origin, ray.direction, movementDirectionRayLength, obstacleMask);
        Debug.DrawRay(ray.origin, ray.direction * movementDirectionRayLength, hit.transform != null ?
                                                                                  Color.red : Color.green);
        return hit.distance;
    }

    Ray[] GetRays() {
        var rays = new Ray[3];
        var pos = (Vector2)transform.position;
        var dir = (Vector2)transform.up * movementDirectionRayLength;
        var offset = pos + ((Vector2)transform.up * rayOffset.y);

        rays[0] = new Ray(offset, dir);
        rays[1] = new Ray(offset - ((Vector2)transform.right * rayOffset.x), dir);
        rays[2] = new Ray(offset + ((Vector2)transform.right * rayOffset.x), dir);

        return rays;
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
