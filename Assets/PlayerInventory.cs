using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour {
    [SerializeField] Transform inventoryBase;
    [SerializeField] List<Toggle> inventorySlots;
    [SerializeField] int currentItemsInInventory;

    public static PlayerInventory staticPlayerInventory;
    public int CurrentItemsInInventory {get { return currentItemsInInventory; }set { currentItemsInInventory = value; }}

    void OnEnable() {
        staticPlayerInventory = this;
        CalculateInventory();
        PlayerLand.OnLandingAtSettlement += RemoveItemsFromInventory; //TODO this isn't final behavior
    }

    void OnDisable() {
        PlayerLand.OnLandingAtSettlement -= RemoveItemsFromInventory;
    }

    void CalculateInventory() {
        foreach (Transform o in inventoryBase) inventorySlots.Add(o.GetComponent<Toggle>());
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
