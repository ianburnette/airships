using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Settlement : MonoBehaviour {
    int playerLayer = 10;
    Transform player;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer != playerLayer) return;
        other.GetComponent<PlayerLand>().InLandingZone = true;
        player = other.transform;
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.layer != playerLayer) return;
        other.GetComponent<PlayerLand>().InLandingZone = false;
        player = null;
    }

    [SerializeField] SettlementInfo info;
    [SerializeField] Transform hanger;
    [SerializeField] List<Ship> ships;

    void OnEnable() {
        RefreshHanger();
        PlayerLand.OnLandingAtSettlement += OpenSettlementDialogue;
    }

    void OnDisable() {
        PlayerLand.OnLandingAtSettlement -= OpenSettlementDialogue;
    }

    void OpenSettlementDialogue() => SettlementCanvas.staticSettlementCanvas.OpenCanvas(this, info, ships, player);

    public void RefreshHanger() {
        ships.Clear();
        foreach (Transform ship in hanger) ships.Add(ship.GetComponent<Ship>());
    }
}
