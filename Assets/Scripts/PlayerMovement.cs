using System;
using System.Collections;
using System.Collections.Generic;
using ProBuilder2.Common;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] float speed;
    [SerializeField] float baseSpeed;
    [SerializeField] Transform transform;
    [SerializeField] Vector3 up;
    [SerializeField] float angleOffset;
    [SerializeField] float rotationThresholdVelocity;
    [SerializeField] float angleSmoothSpeed;
    Ship currentShip;

    [Header("Boost")]
    [FormerlySerializedAs("boost")] [SerializeField] bool boosting;
    [SerializeField] bool canBoost;
    [SerializeField] TrailRenderer boostTrail;
    [SerializeField] float boostEfficiency;
    [SerializeField] float boostSpeed;
    [SerializeField] float resetTime = 2f;

    [Header("Movement Sanitation")]
    [SerializeField] Vector2 rayOffset;
    [SerializeField] float movementDirectionRayLength;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] float[] distances;
    [SerializeField] float distanceSimilarityMargin;

    public InputEvaluation currentInputEvaluation;
    Vector2 previousFramePosition;
    public Vector2 origin;

    [FormerlySerializedAs("mostRecentInput")] public Vector2 currentInput;
    [SerializeField] int degreesPerInputMutation = 5;
    [SerializeField] Vector2 mostRecentInput;
    [SerializeField] bool inSettlement;

    public delegate void Move(float var);
    public static event Move OnMove;

    public delegate void BoostDelegate(float boostCost);
    public static event BoostDelegate OnBoost;
    public delegate void BoostStopDelegate();
    public static event BoostStopDelegate OnBoostStop;

    void OnEnable() {
        PlayerInput.OnMovement += ProcessMovementInput;
        PlayerInput.OnInteract += Boost;
        PlayerInput.OnInteractEnd += StopBoost;
        PlayerLand.OnLandingStateChange += ToggleMovement;
        ShipCanvas.OnShipPurchase += SetupBoost;
        Settlement.OnEnterSettlement += EnterSettlement;
        Settlement.OnExitSettlement += ExitSettlement;

        currentShip = transform.GetChild(0).GetComponent<Ship>();
        SetupBoost(currentShip);
    }

    void OnDisable() {
        PlayerInput.OnMovement -= ProcessMovementInput;
        PlayerInput.OnInteract -= Boost;
        PlayerInput.OnInteractEnd -= StopBoost;
        PlayerLand.OnLandingStateChange -= ToggleMovement;
        ShipCanvas.OnShipPurchase -= SetupBoost;
        Settlement.OnEnterSettlement -= EnterSettlement;
        Settlement.OnExitSettlement -= ExitSettlement;
    }

    void SetupBoost(Ship newship) {
        currentShip = newship;
        boostTrail = currentShip.transform.GetChild(1).GetChild(2).GetComponent<TrailRenderer>();
        boostEfficiency = currentShip.boostEfficiency;
    }

    void ExitSettlement() {
        inSettlement = false;
        canBoost = true;
    }

    void EnterSettlement() {
        inSettlement = true;
        canBoost = false;
        StopBoost();
        CancelInvoke(nameof(ResetBoost));
    }

    void Boost() {
        if (canBoost) {
            boosting = true;
            boostTrail.emitting = true;
            OnBoost?.Invoke(currentShip.boostEfficiency);
            speed = boostSpeed;
        }
    }

    void StopBoost() {
        if (boosting) {
            boosting = false;
            canBoost = false;
            OnBoostStop?.Invoke();
            Invoke(nameof(ResetBoost), resetTime);
            boostTrail.emitting = false;
            speed = baseSpeed;
        }
    }

    void ResetBoost() {
        if (!inSettlement)
            canBoost = true;
    }

    void Start() {
        previousFramePosition = transform.position;
        distances = new float[3];
    }

    void Update() {
            FaceInputDirection();
      //  DebugRays();
    }

    void FixedUpdate() {
        currentInputEvaluation = EvaluateInput(CastRays(GetRays(currentInput)));
        ApplyMovement(currentInput);
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
        if (boosting) {
            currentInput = input == Vector2.zero ? currentInput : MaxedInput(input);
            mostRecentInput = currentInput;
        } else {
            currentInput = input.normalized; //;
            mostRecentInput = currentInput != Vector2.zero ? currentInput : mostRecentInput;
        }
        //DEBUG
        //END DEBUG
        //if (input!=Vector2.zero)
        //    TestInput (input.normalized);
    }

    Vector2 MaxedInput(Vector2 input) => (input * 20f).normalized;

    void TestInput(Vector2 currentInput) {
        distances = CastRays(GetRays(currentInput));
            var eval = EvaluateInput(distances);
        if (eval == InputEvaluation.valid)
            ApplyMovement(currentInput);
        else
            TestInput(MutateInput(currentInput, eval));
    }

    Vector2 MutateInput(Vector2 inputToMutate, InputEvaluation eval) {
        switch (eval) {
        case InputEvaluation.facingWall:
            return Vector2.zero;
        case InputEvaluation.wallToRight:
            return inputToMutate.Rotate(-degreesPerInputMutation);
        case InputEvaluation.wallToLeft:
            return inputToMutate.Rotate(degreesPerInputMutation);
        case InputEvaluation.valid:
            print("trying to mutate valid input");
            return inputToMutate;
        default:
            throw new ArgumentOutOfRangeException(nameof(eval), eval, null);
        }
    }

    InputEvaluation EvaluateInput(float[] currentDistances) {
        if (AllDistancesAreVirtuallyTheSame(currentDistances) &&
            AllDistancesAreGreaterThanZero(currentDistances))
            return InputEvaluation.facingWall;
        if (WallToTheLeft(currentDistances))
            return InputEvaluation.wallToLeft;
        if (WallToTheRight(currentDistances))
            return InputEvaluation.wallToRight;
        return InputEvaluation.valid;
    }

    static bool WallToTheRight(float[] currentDistances) => currentDistances[2] > 0f && currentDistances[0] > 0f;
    static bool WallToTheLeft(float[] currentDistances) => currentDistances[1] > 0f && currentDistances[0] > 0f;

    static bool AllDistancesAreGreaterThanZero(float[] distances) =>
        (distances[0] > 0 && distances[1] > 0 && distances[2] > 0);

    bool AllDistancesAreVirtuallyTheSame(float[] distances) =>
        ((distances[0] + distances[1] + distances[2]) / 3) < distanceSimilarityMargin;

    float[] CastRays(Ray[] rays) {
        for (var i = 0; i < rays.Length; i++) distances[i] = Cast(rays[i]);
        return distances;
    }

    float Cast(Ray ray) {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(ray.origin, ray.direction, movementDirectionRayLength, obstacleMask);
        Debug.DrawRay(ray.origin, ray.direction * movementDirectionRayLength, hit.transform != null ?
                                                                                  Color.red : Color.green);
        return hit.distance;
    }

    Ray[] GetRays(Vector2 currentInput) {
        //TODO this should look at input, not only facing direction (and perhaps not at all)
        var rays = new Ray[3];
        var pos = (Vector2)transform.position;
        //var dir = (Vector2)transform.up * movementDirectionRayLength;
        var dir = currentInput * movementDirectionRayLength;
        var offset = pos + ((Vector2)currentInput * rayOffset.y);

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

public enum InputEvaluation {
    facingWall,
    wallToRight,
    wallToLeft,
    valid
}
