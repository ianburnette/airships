using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Settlement : MonoBehaviour {
    int playerLayer = 10;
    Transform player;

    [SerializeField] SettlementInfo info;
    [SerializeField] Transform hanger;
    [SerializeField] List<Ship> ships;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer != playerLayer) return;
        SetLandingZone(other, true);
        player = other.transform;
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.layer != playerLayer) return;
        SetLandingZone(other, false);
        player = null;
    }

    void SetLandingZone(Component other, bool state) =>
        other.GetComponent<PlayerLand>().InLandingZone = state;

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
