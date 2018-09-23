using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

public class PlayerInventory : MonoBehaviour {

    [Header("Current Inventory")]
    [SerializeField] Transform inventoryBase;
    [SerializeField] List<Toggle> inventorySlots;
    [SerializeField] int currentItemsInInventory;
    [SerializeField] int currentCapacity;
    [SerializeField] Ship currentShip;


    [Header("Persistent Inventory")]
    [SerializeField] float timeBeforeAdding;
    [SerializeField] float timeAfterLeaving;
    [SerializeField] int totalCollectibles;
    [SerializeField] TMP_Text totalText;
    [SerializeField] GameObject collectibleRepresentation;
    public List<int> pickupIndices;
    GameObject[] representations;
    readonly int representationPoolSize;
    int poolBookmark = 0;

    public static PlayerInventory staticPlayerInventory;
    [FormerlySerializedAs("duration")] [SerializeField]
    float representationAnimDuration;
    public int CurrentItemsInInventory {get { return currentItemsInInventory; }set { currentItemsInInventory = value; }}
    public Ship CurrentShip { get { return currentShip; } set { currentShip = value; } }

    void OnEnable() {
        staticPlayerInventory = this;
        if (currentShip == null)
            currentShip = transform.GetChild(0).GetComponent<Ship>();
        ReloadPickups();
        SetupInventory();
        CalculateInventory(currentShip);
        CreateRepresentationPool();
        StartCoroutine(ToggleInventory(state: false, immediate: true));

        Settlement.OnEnterSettlement += EnterSettlement;
        Settlement.OnExitSettlement += ExitingSettlement;
        PlayerShip.OnShipChanged += CalculateInventory;
        ShipCanvas.OnShipPurchase += Purchase;
    }

    void ReloadPickups() {
        if (!ES2.Exists("pickups")) return;
        pickupIndices = ES2.LoadList<int>("pickups");
        MapGeneration.instance.UpdatePickedUpPickups(pickupIndices);
    }

    void OnDisable() {
        Settlement.OnEnterSettlement -= EnterSettlement;
        Settlement.OnExitSettlement -= ExitingSettlement;
        PlayerShip.OnShipChanged -= CalculateInventory;
        ShipCanvas.OnShipPurchase -= Purchase;
    }

    void ExitingSettlement() {
        StartCoroutine(ToggleInventory(state: false, immediate: false));
    }

    IEnumerator ToggleInventory(bool state, bool immediate) {
        if (!state && !immediate)
            yield return new WaitForSeconds(timeAfterLeaving);
        totalText.enabled = state;
        yield return null;
    }

    void Purchase(Ship newShip) {
        totalCollectibles -= newShip.cost;
        totalText.text = totalCollectibles.ToString();
    }

    void CreateRepresentationPool() {
        representations = new GameObject[100];
        for (var i = 0; i < representationPoolSize; i++) {
            var go = Instantiate(collectibleRepresentation);
            go.transform.parent = transform;
            representations[i] = go;
            go.SetActive(false);
        }
    }

    void SetupInventory() {
        foreach (Transform o in inventoryBase) {
            inventorySlots.Add(o.GetComponent<Toggle>());
            o.gameObject.SetActive(false);
        }
    }

    void CalculateInventory(Ship newShip) {
        currentShip = newShip;
        currentCapacity = (int)(currentShip.capacity * 100);
        for (var i = 0; i < currentCapacity; i++)
            inventorySlots[i].gameObject.SetActive(true);
    }

    public bool AttemptToAddItemToInventory(int pickupIndex) {
        if (currentItemsInInventory >= currentCapacity) return false;
        AddItemToInventory(pickupIndex);
        return true;
    }

    void AddItemToInventory(int pickupIndex) {
        inventorySlots[currentItemsInInventory].isOn = true;
        pickupIndices.Add(pickupIndex);
        currentItemsInInventory++;
    }

    IEnumerator TransferToPersistentInventory() {
        yield return new WaitForSeconds(timeBeforeAdding);
        for (var i = currentItemsInInventory; i > -1; i--) {
            if (inventorySlots[i].isOn) {
                inventorySlots[i].isOn = false;
                totalCollectibles++;
                totalText.text = totalCollectibles.ToString();
                //MoveRepresentation(GetFromPool());
                yield return new WaitForEndOfFrame();
            }
        }
        currentItemsInInventory = 0;
        yield return null;
    }

    void MoveRepresentation(GameObject representation) =>
        representation.transform.DOMove(totalText.transform.position, representationAnimDuration);
                      //.onComplete(() => ReturnToPool(representation));

    void EnterSettlement() {
        StartCoroutine(ToggleInventory(state: true, immediate: true));
        StartCoroutine(TransferToPersistentInventory());
        ES2.Save(pickupIndices, "pickups");
        //var difference = currentItemsInInventory - quantity;
        //for (var i = currentItemsInInventory - 1; i >= difference; i--)
        //    inventorySlots[i].isOn = false;

    }

    //void EnterSettlement() => EnterSettlement(currentItemsInInventory);

    GameObject GetFromPool() {
        for (var i = 0; i < representationPoolSize; i++) {
            var go = representations[i];
            if (go.activeSelf) continue;
            go.SetActive(true);
            return go;
        }
        print("didn't find non-active object in pool");
        return null;
    }

    void ReturnToPool(GameObject toReturn) {
        for (var i = 0; i<representationPoolSize; i++)
            if (representations[i] == toReturn) {
                representations[i].SetActive(false);
                return;
            }
    }

}


