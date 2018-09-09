﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour {

    public static PlayerShip staticPlayerShip;

    [SerializeField] Ship currentShip;
    [SerializeField] LayerMask playerShipLayer;
    [SerializeField] LayerMask npcLayer;

    public delegate void ShipAnimatorChanged(Animator anim);
    public static event ShipAnimatorChanged OnShipAnimatorChanged;

    public delegate void ShipChanged();
    public static event ShipChanged OnShipChanged;

    void Start() => staticPlayerShip = this;

    public void NewShip(Ship newShip) {
        currentShip.transform.parent = null;
        currentShip.gameObject.layer = LayerMask.NameToLayer("NPC");
        currentShip = newShip;

        transform.position = currentShip.transform.position;
        transform.rotation = currentShip.transform.rotation;
        currentShip.gameObject.layer = LayerMask.NameToLayer("PlayerShip");
        currentShip.transform.parent = transform;
        OnShipAnimatorChanged?.Invoke(newShip.GetComponent<Animator>());
        OnShipChanged?.Invoke();
    }
}