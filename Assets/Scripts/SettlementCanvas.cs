using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets._2D;

public class SettlementCanvas : MonoBehaviour {
    public static SettlementCanvas staticSettlementCanvas;

    [SerializeField] TMP_Text titleIntro;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text description;
    [SerializeField] GameObject tradeButton;
    [SerializeField] RectTransform cursor;

    [SerializeField] List<ShipInfoPanel> shipInfoPanels;

    [SerializeField] Animator anim;
    [SerializeField] List<Ship> ships;
    [SerializeField] Ship currentlySelectedShip;
    [SerializeField] Transform currentlySelectedTransform;
    [SerializeField] Settlement currentSettlement;

    [SerializeField] EventSystem eventSystem;

    [SerializeField] SettlementCanvasState currentState = SettlementCanvasState.Home;
    Dictionary<SettlementCanvasState, string> stateAnimation = new Dictionary<SettlementCanvasState, string>() {
        { SettlementCanvasState.Home, "ShowHome" },
        { SettlementCanvasState.Talk, "ShowTalk" },
        { SettlementCanvasState.Trade, "ShowTrade" },
        { SettlementCanvasState.TradeConfirm, "ShowTradeConfirm" },
    };
    [SerializeField] List<GameObject> firstSelectedButtons;

    void Start() {
        staticSettlementCanvas = this;
        currentState = SettlementCanvasState.Home;
        eventSystem.SetSelectedGameObject(firstSelectedButtons[(int)currentState]);
    }

    public void SetSettlementCanvasState(int newStateInt) =>
        SetSettlementCanvasState((SettlementCanvasState)newStateInt, currentState);

    public void SetSettlementCanvasState(SettlementCanvasState newState, SettlementCanvasState prevState) {
        currentState = newState;
        switch (currentState) {
            case (SettlementCanvasState.Home):
                SetTradeButtonStateDependingOnShipCount();
                break;
            case (SettlementCanvasState.Trade):
                PopulateShipButtonInfo();
                break;
        }
        anim.SetBool(stateAnimation[prevState], false);
        anim.SetBool(stateAnimation[currentState], true);
        eventSystem.SetSelectedGameObject(firstSelectedButtons[(int)currentState]);
    }

    void PopulateShipButtonInfo() {
        for (var i = 0; i < ships.Count; i++) PopulateShipButton(shipInfoPanels[i], ships[i]);
        if (shipInfoPanels.Count>ships.Count)
            for (var i = ships.Count; i < shipInfoPanels.Count; i++)
                shipInfoPanels[i].panel.SetActive(false);
    }

    void PopulateShipButton(ShipInfoPanel shipInfoPanel, Ship ship) {
        shipInfoPanel.panel.SetActive(true);
        shipInfoPanel.nameText.text = ship.shipName;
        shipInfoPanel.capacityText.text = ship.capacity.ToString();
        shipInfoPanel.fuelText.text = ship.fuelDepletionRate.ToString();
        shipInfoPanel.speedText.text = ship.speed.ToString();
        shipInfoPanel.costText.text = ship.cost.ToString();
    }

    public void SetInfo(SettlementInfo info) {
        titleIntro.text = info.titleIntro;
        title.text = info.title;
        description.text = info.description;
    }

    public void OpenCanvas(Settlement settlement, SettlementInfo info, List<Ship> ships, Transform player) {
        currentSettlement = settlement;
        CameraFollow.cam.Target = settlement.transform;
        currentlySelectedTransform = player;
        SetInfo(info);
        this.ships = ships;
        SetTradeButtonStateDependingOnShipCount();
        ShowCanvas(true);
    }

    void SetTradeButtonStateDependingOnShipCount() {
        tradeButton.SetActive(this.ships.Count > 0);
    }

    public void CloseCanvas() {
        CameraFollow.cam.Target = currentlySelectedTransform;
        currentState = 0;
        ShowCanvas(false);
    }

    public void StartTrade(int shipIndex) {
        currentlySelectedShip = ships[shipIndex];
        SetSettlementCanvasState(SettlementCanvasState.TradeConfirm, currentState);
    }

    public void CompleteTrade() {
        PlayerShip.staticPlayerShip.NewShip(currentlySelectedShip);
        currentlySelectedTransform = currentlySelectedShip.transform;
        SetSettlementCanvasState(SettlementCanvasState.Home, currentState);
        currentSettlement.RefreshHanger();
    }

    void ShowCanvas(bool state) {
        anim.SetBool("ShowHome", state);
        anim.SetBool("ShowSettlementPanel", state);
    }

}

[Serializable]
public enum SettlementCanvasState {
    Home = 0,
    Talk = 1,
    Trade = 2,
    TradeConfirm = 3
}

[Serializable]
public class SettlementInfo {
    public string titleIntro = "Welcome to";
    public string title;
    public string description;
    public List<string> dialogue;
}

[Serializable]
public class ShipInfoPanel {
    public GameObject panel;
    public TMP_Text nameText, capacityText, fuelText, speedText, costText;
}
