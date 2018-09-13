using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public static CameraFollow cam;

    [SerializeField] Transform target;
    [SerializeField] float followSpeed;
    [SerializeField] float lookAheadDistMult;
    [SerializeField] Vector2 lookAheadAspectRatio;
    [SerializeField] Vector2 offset;
    [SerializeField] float lookAheadSmootheTime;
    Rigidbody2D rb;
    Vector2 currentLookAheadOffset;
    public Vector2 Offset { get { return offset; } set { offset = value; } }
    public Transform Target { get { return target; } set { target = value; } }
    void OnEnable() {
        cam = this;
        GetCameraTarget();
    }

    void GetCameraTarget() {
        rb = target.GetComponent<Rigidbody2D>();
    }

    void Update() =>
        transform.position = Vector2.Lerp(transform.position, GetPosition(), followSpeed * Time.deltaTime);

    Vector2 GetPosition() {
        if (rb == null) return target.position;
        currentLookAheadOffset = SmoothePosition(GetLookAheadPosition());
        return (Vector2)target.position + currentLookAheadOffset;
    }

    Vector2 SmoothePosition(Vector2 pos) =>
        Vector2.Lerp(currentLookAheadOffset, pos, lookAheadSmootheTime * Time.deltaTime);

    Vector2 GetLookAheadPosition() {
        lookAheadAspectRatio = CorrectForAspectRatio(rb.velocity);
        return (new Vector2
                    (rb.velocity.x * lookAheadAspectRatio.x,
                     rb.velocity.y * lookAheadAspectRatio.y)
                + offset);
    }

    Vector2 CorrectForAspectRatio(Vector2 vel) =>
        new Vector2(lookAheadDistMult, lookAheadDistMult * ((float)Screen.height/(float)Screen.width));
}
