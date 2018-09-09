using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour {
    [SerializeField] Transform inventoryBase;
    [SerializeField] List<Toggle> inventorySlots;
    [SerializeField] int currentItemsInInventory;
    [SerializeField] Ship currentShip;

    public static PlayerInventory staticPlayerInventory;
    public int CurrentItemsInInventory {get { return currentItemsInInventory; }set { currentItemsInInventory = value; }}
    public Ship CurrentShip { get { return currentShip; } set { currentShip = value; } }

    void OnEnable() {
        staticPlayerInventory = this;
        if (currentShip == null)
            currentShip = transform.GetChild(0).GetComponent<Ship>();
        SetupInventory();
        CalculateInventory();
        PlayerLand.OnLandingAtSettlement += RemoveItemsFromInventory; //TODO this isn't final behavior
        PlayerShip.OnShipChanged += CalculateInventory;
    }

    void OnDisable() => PlayerLand.OnLandingAtSettlement -= RemoveItemsFromInventory;

    void SetupInventory() {
        foreach (Transform o in inventoryBase) {
            inventorySlots.Add(o.GetComponent<Toggle>());
            o.gameObject.SetActive(false);
        }
    }

    void CalculateInventory() {
        for (var i = 0; i < currentShip.capacity; i++)
            inventorySlots[i].gameObject.SetActive(true);
    }

    public bool AttemptToAddItemToInventory() {
        if (currentItemsInInventory >= inventorySlots.Count) return false;
        AddItemToInventory();
        return true;
    }

    void AddItemToInventory() {
        inventorySlots[currentItemsInInventory].isOn = true;
        currentItemsInInventory++;
    }

    void RemoveItemsFromInventory(int quantity)  {
        var difference = currentItemsInInventory - quantity;
        for (var i = currentItemsInInventory - 1; i >= difference; i--)
            inventorySlots[i].isOn = false;
        currentItemsInInventory = difference;
    }

    void RemoveItemsFromInventory() => RemoveItemsFromInventory(currentItemsInInventory);
}
