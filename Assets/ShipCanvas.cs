using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipCanvas : MonoBehaviour {
    public static ShipCanvas instance;

    [SerializeField] Canvas canvas;
    [SerializeField] List<Slider> stats;
    [SerializeField] TMP_Text title, cost;

    public bool activated, confirming;
    public Ship currentShip;

    void OnEnable() {
        instance = this;
        PlayerInput.OnInteract += Interact;
    }

    void OnDisable() {
        PlayerInput.OnInteract -= Interact;
    }

    void Interact() {
        if (activated && !confirming)
            confirming = true;
        else if (activated && confirming)
            Purchase();
    }

    public void Activate(bool state, [CanBeNull] Ship ship) {
        activated = state;
        if (state) {
            title.text = ship.name;
            stats[0].value = ship.fuelEfficiency;
            stats[1].value = ship.capacity;
            stats[2].value = ship.boostEfficiency;
            stats[3].value = ship.handling;
            cost.text = ship.cost.ToString();
            transform.position = (Vector2)ship.transform.position + ship.canvasOffset;
            currentShip = ship;
        } else {
            title.text = "";
            stats[0].value = 0;
            stats[1].value = 0;
            stats[2].value = 0;
            stats[3].value = 0;
            cost.text = "";
            currentShip = null;
        }
        canvas.enabled = state;
    }

    void Purchase() {
        if (currentShip != null)
            PlayerShip.staticPlayerShip.NewShip(currentShip);
        canvas.enabled = false;
    }
}
