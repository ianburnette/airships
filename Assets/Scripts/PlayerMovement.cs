﻿using System;
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

    public InputEvaluation currentInputEvaluation;
    Vector2 previousFramePosition;
    public Vector2 origin;

    public Vector2 mostRecentInput;
    [SerializeField] int degreesPerInputMutation = 5;

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


    void Start() {
        previousFramePosition = transform.position;
        distances = new float[3];
    }

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
        //DEBUG
        currentInputEvaluation = EvaluateInput(CastRays(GetRays()));
        //END DEBUG
        //TestInput (input.normalized);
        ApplyMovement(input);
        mostRecentInput = input;
    }

    void TestInput(Vector2 currentInput) {
        distances = CastRays(GetRays());
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
            break;
        case InputEvaluation.wallToRight:
            return inputToMutate.Rotate(-degreesPerInputMutation);
            break;
        case InputEvaluation.wallToLeft:
            return inputToMutate.Rotate(degreesPerInputMutation);
            break;
        case InputEvaluation.valid:
            print("trying to mutate valid input");
            return inputToMutate;
            break;
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

    Ray[] GetRays() {
        //TODO this should look at input, not only facing direction (and perhaps not at all)
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

public enum InputEvaluation {
    facingWall,
    wallToRight,
    wallToLeft,
    valid
}
