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
    [SerializeField] TMP_Text[] buttons;
    [SerializeField] GameObject tradeButton;
    [SerializeField] RectTransform selectionCursor, arrow;

    [SerializeField] List<TMP_Text> shipInfoPanels;

    [SerializeField] Animator anim;
    [SerializeField] List<Ship> ships;
    [SerializeField] Ship currentlySelectedShip;
    [SerializeField] Transform currentlySelectedTransform;
    [SerializeField] Settlement currentSettlement;

    [SerializeField] EventSystem eventSystem;

    [SerializeField] SettlementCanvasState currentState = SettlementCanvasState.Home;
    Dictionary<SettlementCanvasState, string> stateAnimation = new Dictionary<SettlementCanvasState, string>() {
        { SettlementCanvasState.Home, "ShowHome" },
        { SettlementCanvasState.Trade, "ShowTrade" },
        { SettlementCanvasState.TradeConfirm, "ShowTradeConfirm" },
    };

    [SerializeField] int currentlySelectedButtonIndex;
    [SerializeField] float arrowSpeed;
    [SerializeField] float inputThreshold = .5f;
    [SerializeField] float noInputThreshold = .05f;
    bool waitForInputReset;

    void OnEnable() {
        PlayerInput.OnMovement += MoveCursor;
        PlayerInput.OnInteract += Interact;
    }

    void OnDisable() {
        PlayerInput.OnMovement -= MoveCursor;
        PlayerInput.OnInteract -= Interact;
    }

    void MoveCursor(Vector2 input) {
        if (Mathf.Abs(input.y) > inputThreshold && !waitForInputReset) {
            AttemptButtonChange(!(input.y > inputThreshold));
            waitForInputReset = true;
        } else if (Mathf.Abs(input.y) < noInputThreshold) waitForInputReset = false;
    }

    void AttemptButtonChange(bool down) {
        if (down) {
            for (var i = currentlySelectedButtonIndex + 1; i < buttons.Length; i++) {
                if (!buttons[i].gameObject.activeSelf) continue;
                currentlySelectedButtonIndex = i;
                break;
            }
        } else {
            for (var i = currentlySelectedButtonIndex - 1; i > -1; i--) {
                if (!buttons[i].gameObject.activeSelf) continue;
                currentlySelectedButtonIndex = i;
                break;
            }
        }
    }

    void Interact() {
        switch (currentState) {
        case SettlementCanvasState.Home:
            switch (currentlySelectedButtonIndex) {
            case 0:
                SetSettlementCanvasState(SettlementCanvasState.Trade);
                break;
            case 3:
                CloseCanvas();
                break;
            }
            break;
        case SettlementCanvasState.Trade:
            switch (currentlySelectedButtonIndex) {
            case 0:
                SetSettlementCanvasState(SettlementCanvasState.TradeConfirm);
                break;
            case 1:
                SetSettlementCanvasState(SettlementCanvasState.TradeConfirm);
                break;
            case 2:
                SetSettlementCanvasState(SettlementCanvasState.TradeConfirm);
                break;
            case 3:
                SetSettlementCanvasState(SettlementCanvasState.Home);
                break;
            }
            break;
        case SettlementCanvasState.TradeConfirm:
            switch (currentlySelectedButtonIndex) {
            case 0:
                CompleteTrade(ships[0]);
                CloseCanvas();
                break;
            case 1:
                CompleteTrade(ships[2]);
                CloseCanvas();
                break;
            case 2:
                CompleteTrade(ships[3]);
                CloseCanvas();
                break;
            case 3:
                SetSettlementCanvasState(SettlementCanvasState.Trade);
                break;
            }
            break;
        }
    }

    void Start() {
        staticSettlementCanvas = this;
        currentState = SettlementCanvasState.Home;
    }

    void Update() {
 //       arrow.position = Vector2.Lerp(arrow.position,
 //                                     new Vector2(arrow.position.x,
 //                                                 buttons[currentlySelectedButtonIndex].transform.position.y),
 //                                     arrowSpeed * Time.deltaTime);
    }

    public void OpenCanvas(Settlement settlement,
                           SettlementInfo info,
                           List<Ship> shipsFromThisSettlement,
                           Transform player) {
        currentSettlement = settlement;
        StaticCamera.instance.NewTarget(settlement.transform);
        currentlySelectedTransform = player;
        SetInfo(info);
        ships = shipsFromThisSettlement;
        SetTradeButtonStateDependingOnShipCount();
        currentlySelectedButtonIndex = buttons[0].gameObject.activeSelf ? 0 : 3;
        ShowCanvas(true);
    }

    public void SetSettlementCanvasState(SettlementCanvasState newState) {
        currentState = newState;
        switch (currentState) {
            case (SettlementCanvasState.Home):
                // TODO
                //SetTradeButtonStateDependingOnShipCount();
                break;
            case (SettlementCanvasState.Trade):
                //PopulateShipButtonInfo();
                break;
            case (SettlementCanvasState.TradeConfirm):

                break;
        }
    }

    void PopulateShipButtonInfo() {
        for (var i = 0; i < ships.Count; i++) PopulateShipButton(buttons[i], ships[i]);
        if (buttons.Length-1 > ships.Count)
            for (var i = ships.Count; i < buttons.Length - 1; i++)
                buttons[i].gameObject.SetActive(false);
    }

    void PopulateShipButton(TMP_Text buttonForShipInfo, Ship ship) {
        buttonForShipInfo.gameObject.SetActive(true);
        buttonForShipInfo.text = "<b>" + ship.shipName + "</b>";
        buttonForShipInfo.text += "/n Capacity: " + ship.capacity;
        buttonForShipInfo.text += "/n Fuel Efficiency: " + ship.fuelEfficiency;
        buttonForShipInfo.text += "/n Handling: " + ship.handling;
        buttonForShipInfo.text += "/n Boost Fuel Cost:" + ship.boostEfficiency;
        buttonForShipInfo.text += "/n Trade Cost:" + ship.cost;
    }

    public void SetInfo(SettlementInfo info) {
        titleIntro.text = info.titleIntro;
        title.text = info.title;
        description.text = info.description;
    }

    void SetTradeButtonStateDependingOnShipCount() => buttons[0].gameObject.SetActive(ships.Count > 0);

    public void CloseCanvas() {
        StaticCamera.instance.NewTarget(currentlySelectedTransform);
        currentState = 0;
        ShowCanvas(false);
    }

    public void CompleteTrade(Ship newShip) {
        PlayerShip.staticPlayerShip.NewShip(newShip);
        currentlySelectedTransform = newShip.transform;
        //currentSettlement.RefreshHanger();
    }

    void ShowCanvas(bool state) {
        anim.SetBool("ShowHome", true);
        anim.SetBool("ShowSettlementPanel", state);
    }

}

[Serializable]
public enum SettlementCanvasState {
    Home = 0,
    Trade = 1,
    TradeConfirm = 2
}

[Serializable]
public class SettlementInfo {
    public string titleIntro = "Welcome to";
    public string title;
    public string description;
    public List<string> dialogue;
}

