using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFuel : MonoBehaviour {
    public static PlayerFuel instance;

    [SerializeField] Slider fuelGauge;
    [SerializeField] float maxFuel;
    [SerializeField] float currentFuel;
    [SerializeField] float fuelDepletionRate;
    [SerializeField] float masterFuelDepletionRate;
    [SerializeField] float masterRefuelRate;
    [SerializeField] float masterBoostCost;
    [SerializeField] float masterCollisionPenalty;

    public bool boosting;
    public bool outOfFuel;

    public delegate void FuelDepleted();
    public static event FuelDepleted OnFuelDepleted;

    public float CurrentFuel { get { return currentFuel; } set {
        currentFuel = value;
        UpdateFuelGauge();
        CheckForEmpty();
    } }


    public float MaxFuel { get { return maxFuel; } set { maxFuel = value; } }

    void Start() => outOfFuel = false;

    void OnEnable() {
        instance = this;
        PlayerMovement.OnMove += OnMovementApplied;
        PlayerShip.OnShipChanged += NewShipFuel;
        PlayerMovement.OnBoost += BoostFuel;
        PlayerMovement.OnBoostStop += BoostStop;

        NewShipFuel(transform.GetChild(0).GetComponent<Ship>());
    }

    void OnDisable() {
        PlayerMovement.OnMove -= OnMovementApplied;
        PlayerShip.OnShipChanged -= NewShipFuel;
        PlayerMovement.OnBoost -= BoostFuel;
        PlayerMovement.OnBoostStop -= BoostStop;
    }

    void CheckForEmpty() {
        if (!outOfFuel && CurrentFuel <= 0f)
            OutOfFuel();
        if (outOfFuel && currentFuel <= 0f)
            OutOfFuel();
    }
    void BoostStop() {
        boosting = false;
    }

    void BoostFuel(float efficiency) => StartCoroutine(DepleteFromBoost(masterBoostCost * (1f - efficiency)));

    IEnumerator DepleteFromBoost(float amtToDeplete) {
        boosting = true;
        CurrentFuel -= amtToDeplete;
        print(("deducting fuel for boost"));
        yield return null;
        //var diff = (currentFuel - amtToDeplete) * 100f;
        //for (var i = 0; i < diff; i++) {
        //    currentFuel -= diff / 100f;
        //    yield return new WaitForEndOfFrame();
        //}
    }

    void NewShipFuel(Ship newShip) {
        fuelDepletionRate = newShip.fuelEfficiency;
    }

    void OnMovementApplied(float magnitude) {
        if (CurrentFuel > 0 && !boosting) {
            DepleteFuel(magnitude);
        }
    }

    void OutOfFuel() {
        outOfFuel = true;
        //OnFuelDepleted?.Invoke();
    }

    public void Increment() {
        outOfFuel = false;
        if (CurrentFuel < 0)
            CurrentFuel = 0;
        if (CurrentFuel < 1)
            CurrentFuel += masterRefuelRate * Time.deltaTime;
    }

    void DepleteFuel(float magnitude) {
        CurrentFuel -=
            magnitude *
            (masterFuelDepletionRate * (1f - fuelDepletionRate)) *
            Time.deltaTime;
    }

    void UpdateFuelGauge() => fuelGauge.value = CurrentFuel;

    public void Collide() {
        print("deducting fuel for collision");
        CurrentFuel -= masterCollisionPenalty;
    }
}
