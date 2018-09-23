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
    [SerializeField] float dampSpeed;

    [Header("Movement Sanitation")]
    [SerializeField] Vector2 rayOffset;
    [SerializeField] float movementDirectionRayLength;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] float[] distances;
    [SerializeField] float distanceSimilarityMargin;
    public int testsThisFrame, maxTestsPerFrame;

    public InputEvaluation currentInputEvaluation;
    Vector2 previousFramePosition;
    public Vector2 origin;

    [FormerlySerializedAs("mostRecentInput")] public Vector2 currentInput;
    [SerializeField] int degreesPerInputMutation = 5;
    [SerializeField] Vector2 mostRecentInput;
    [SerializeField] Vector2 inputDirection;
    [SerializeField] bool inSettlement;
    [SerializeField] float repelForce;
    [SerializeField] float repelSpeed;

    public delegate void Move(float var);
    public static event Move OnMove;

    public delegate void BoostDelegate(float boostCost);
    public static event BoostDelegate OnBoost;
    public delegate void BoostStopDelegate();
    public static event BoostStopDelegate OnBoostStop;

    void OnEnable() {
        PlayerInput.OnMovement += ReceiveMovementInput;
        PlayerInput.OnInteract += Boost;
        //PlayerInput.OnInteractEnd += StopBoost;
        PlayerLand.OnLandingStateChange += ToggleMovement;
        ShipCanvas.OnShipPurchase += SetupBoost;
        Settlement.OnEnterSettlement += EnterSettlement;
        Settlement.OnExitSettlement += ExitSettlement;

        currentShip = transform.GetChild(0).GetComponent<Ship>();
        SetupBoost(currentShip);
    }

    void OnDisable() {
        PlayerInput.OnMovement -= ReceiveMovementInput;
        PlayerInput.OnInteract -= Boost;
        //PlayerInput.OnInteractEnd -= StopBoost;
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
    }

    void FixedUpdate() {
        testsThisFrame = 0;
        //if (currentInput!=Vector2.zero)
        //    TestInput(currentInput);
       // currentInput = Vector2.Lerp(currentInput, currentInput + CastRays(GetRays(currentInput)),
         //                           repelSpeed * Time.deltaTime);
        ProcessMovementInput(currentInput);
        if (boosting)
            DampMovementInput();
        ApplyMovement(currentInput);
        //currentInputEvaluation = EvaluateInput(CastRays(GetRays(thisInput)));
        //ApplyMovement(thisInput);
    }


    void FaceInputDirection() => transform.rotation = SmoothedRotation
                                     (Quaternion.AngleAxis(MovementAngle(inputDirection) + angleOffset, up));

    void ReceiveMovementInput(Vector2 input) {
        currentInput = input;
        if (input != Vector2.zero)
            inputDirection = input;
    }

    Quaternion SmoothedRotation(Quaternion inputRotation) =>
        Quaternion.Lerp(transform.rotation, inputRotation, angleSmoothSpeed * Time.deltaTime);
    Vector2 MovementVelocity() => rigidbody2D.velocity;
    float MovementAngle(Vector2 dir) => Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

    void ProcessMovementInput(Vector2 input) {
        if (boosting) {
            currentInput = MaxedInput(input.magnitude < .01 ? mostRecentInput : currentInput);
            mostRecentInput = currentInput;
        } else {
            currentInput = input.normalized;
            mostRecentInput = currentInput != Vector2.zero ? currentInput : mostRecentInput;
        }
    }

    Vector2 MaxedInput(Vector2 input) => (input * 20f).normalized;

    void DampMovementInput() {
        currentInput = Vector2.Lerp(rigidbody2D.velocity.normalized, mostRecentInput, dampSpeed * Time.deltaTime);
        Debug.DrawRay(transform.position, transform.up * currentInput.magnitude, Color.magenta);
        Debug.DrawRay(transform.position, currentInput, Color.blue);
        Debug.DrawRay(transform.position, rigidbody2D.velocity, Color.red);
    }

    void TestInput(Vector2 thisInput) {
        if (testsThisFrame < maxTestsPerFrame) {
            testsThisFrame++;
           // distances = CastRays(GetRays(thisInput));
            currentInputEvaluation = EvaluateInput(distances);
            if (currentInputEvaluation == InputEvaluation.valid)
                currentInput = thisInput;
            else
                TestInput(MutateInput(thisInput, currentInputEvaluation));
        } else {
            print("reached max tests per frame");
            currentInput = thisInput;
        }
    }

    Vector2 MutateInput(Vector2 inputToMutate, InputEvaluation eval) {
        switch (eval) {
        case InputEvaluation.facingWall:
            return Vector2.zero;
        case InputEvaluation.wallToRight:
            return inputToMutate.Rotate(degreesPerInputMutation);
        case InputEvaluation.wallToLeft:
            return inputToMutate.Rotate(-degreesPerInputMutation);
        case InputEvaluation.valid:
            print("trying to mutate valid input");
            return inputToMutate;
        default:
            throw new ArgumentOutOfRangeException(nameof(eval), eval, null);
        }
    }

    InputEvaluation EvaluateInput(float[] currentDistances) {
        if (WallToTheLeft(currentDistances))
            return InputEvaluation.wallToLeft;
        if (WallToTheRight(currentDistances))
            return InputEvaluation.wallToRight;
        if (AllDistancesAreVirtuallyTheSame(currentDistances) &&
            AllDistancesAreGreaterThanZero(currentDistances))
            return InputEvaluation.facingWall;
        return InputEvaluation.valid;
    }

    static bool WallToTheRight(float[] currentDistances) => currentDistances[2] > 0f && currentDistances[0] == 0f;
    static bool WallToTheLeft(float[] currentDistances) => currentDistances[0] > 0f && currentDistances[2] == 0f;

    static bool AllDistancesAreGreaterThanZero(float[] distances) =>
        (distances[0] > 0 && distances[1] > 0 && distances[2] > 0);

    bool AllDistancesAreVirtuallyTheSame(float[] distances) =>
        Mathf.Abs(distances[0] - distances[1]) < distanceSimilarityMargin &&
        Mathf.Abs(distances[1] - distances[2]) < distanceSimilarityMargin &&
        Mathf.Abs(distances[0] - distances[2]) < distanceSimilarityMargin;

    Vector2 CastRays(Ray[] rays) {
        //for (var i = 0; i < rays.Length; i++) distances[i] = Cast(rays[i]);
        return Cast(rays[0]) + Cast(rays[1]) + Cast(rays[2]);
    }

    Vector2 Cast(Ray ray) {
        var hit = Physics2D.Raycast(ray.origin, ray.direction, movementDirectionRayLength, obstacleMask);
        Debug.DrawRay(ray.origin, ray.direction * movementDirectionRayLength, hit.transform != null ?
                                                                                  Color.red : Color.green);
        if (hit.transform != null && Mathf.Abs(((Vector2)hit.normal - (Vector2)ray.direction).magnitude) > distanceSimilarityMargin)
            return Vector2.Reflect(ray.direction, hit.normal) * hit.distance * repelForce;
        return Vector2.zero;
    }

    Ray[] GetRays(Vector2 currentInput) {

        var rays = new Ray[3];
        var pos = (Vector2)transform.position;
        //var dir = (Vector2)transform.up * movementDirectionRayLength;
        var dir = currentInput * movementDirectionRayLength;
        var offset = pos + ((Vector2)currentInput * rayOffset.y);

        rays[0] = new Ray(offset - ((Vector2)transform.right * rayOffset.x), dir);
        rays[1] = new Ray(offset, dir);
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
        rigidbody2D.AddForce(input * (speed * Time.deltaTime));
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
