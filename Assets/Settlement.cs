using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) => other.GetComponent<PlayerLand>().InLandingZone = true;
    void OnTriggerExit2D(Collider2D other) => other.GetComponent<PlayerLand>().InLandingZone = false;
}
