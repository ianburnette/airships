using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollider : MonoBehaviour {
    PolygonCollider2D col;

    [SerializeField] float maxMagnitude = 5f;
    [SerializeField] Ship myShip;
    [SerializeField] float resetTime;
    [SerializeField] bool invincible;

    void OnEnable() {
        col = gameObject.AddComponent<PolygonCollider2D>();
    }

    void OnCollisionEnter2D(Collision2D other) {
        //print(other.relativeVelocity.magnitude);
        if (other.relativeVelocity.magnitude > maxMagnitude && !invincible) {
            myShip.Collide();
            invincible = true;
            Invoke(nameof(ResetInvincibility), resetTime);
        }
    }

    void ResetInvincibility() {
        invincible = false;
    }
}
