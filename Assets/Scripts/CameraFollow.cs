using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public static CameraFollow cam;

    [SerializeField] Transform target;
    [SerializeField] float followSpeed;
    [SerializeField] float lookAheadDistMult;
    [SerializeField] Vector2 lookAheadAspectRatio;
    [SerializeField] Vector2 offset;
    public Vector2 Offset { get { return offset; } set { offset = value; } }
    public Transform Target { get { return target; } set { target = value; } }
    void OnEnable() => cam = this;

    void FixedUpdate() =>
        transform.position = Vector2.Lerp(transform.position, GetPostition(), followSpeed * Time.deltaTime);

    Vector2 GetPostition() {
        var rb = target.GetComponent<Rigidbody2D>();
        if (rb == null) return target.position;
        lookAheadAspectRatio = CorrectForAspectRatio(rb.velocity);
        return (Vector2)target.position + (new Vector2
                                               (rb.velocity.x * lookAheadAspectRatio.x,
                                                rb.velocity.y * lookAheadAspectRatio.y)
                                           + offset);
    }

    Vector2 CorrectForAspectRatio(Vector2 vel) =>
        new Vector2(lookAheadDistMult, lookAheadDistMult * ((float)Screen.height/(float)Screen.width));
}
