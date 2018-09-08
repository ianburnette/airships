using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] Rigidbody2D target;
    [SerializeField] float followSpeed;
    [SerializeField] float lookAheadDistMult;
    [SerializeField] Vector2 offset;
    public Vector2 Offset { get { return offset; } set { offset = value; } }

    void FixedUpdate() =>
        transform.position = Vector2.Lerp(transform.position, GetPostition(), followSpeed * Time.deltaTime);

    Vector2 GetPostition() => target.position + (target.velocity * lookAheadDistMult) + offset;
}
