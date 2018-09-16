using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Settlement : MonoBehaviour {
    int playerLayer = 10;
    PlayerFuel playerFuel;

    public delegate void EnterSettlement();
    public static EnterSettlement OnEnterSettlement;

    public delegate void ExitSettlement();
    public static ExitSettlement OnExitSettlement;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer != playerLayer) return;
        playerFuel = other.GetComponent<PlayerFuel>();
        OnEnterSettlement?.Invoke();
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.layer != playerLayer) return;
        playerFuel = null;
        OnExitSettlement?.Invoke();
    }

    void OnTriggerStay2D(Collider2D other) {
        playerFuel.Increment();
    }
}
