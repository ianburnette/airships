using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Settlement : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) => other.GetComponent<PlayerLand>().InLandingZone = true;
    void OnTriggerExit2D(Collider2D other) => other.GetComponent<PlayerLand>().InLandingZone = false;

    [SerializeField] SettlementInfo info;
    [SerializeField] Transform hanger;
    [SerializeField] List<Ship> ships;

    void OnEnable() {
        foreach (Transform ship in hanger) ships.Add(ship.GetComponent<Ship>());
        PlayerLand.OnLandingAtSettlement += OpenSettlementDialogue;
    }

    void OnDisable() {
        PlayerLand.OnLandingAtSettlement -= OpenSettlementDialogue;
    }

    void OpenSettlementDialogue() => SettlementCanvas.staticSettlementCanvas.OpenCanvas(info, ships);
}
