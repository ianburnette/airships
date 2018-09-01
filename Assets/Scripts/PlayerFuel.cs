using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFuel : MonoBehaviour {
    [SerializeField] Slider fuelGauge;
    [SerializeField] float maxFuel;
    [SerializeField] float currentFuel;
    [SerializeField] float fuelDepletionRate;

    bool outOfFuel;

    public delegate void FuelDepleted();
    public static event FuelDepleted OnFuelDepleted;

    public float MaxFuel { get { return maxFuel; } set { maxFuel = value; } }

    void Start() => outOfFuel = false;

    void OnEnable() {
        PlayerMovement.OnMove += OnMovementApplied;
        PlayerLand.OnLandingAtSettlement += Refuel;
    }

    void OnDisable() {
        PlayerMovement.OnMove -= OnMovementApplied;
        PlayerLand.OnLandingAtSettlement -= Refuel;
    }

    void OnMovementApplied(float magnitude) {
        if (currentFuel > 0) {
            DepleteFuel(magnitude);
            UpdateFuelGauge();
        } else if (!outOfFuel)
            OutOfFuel();
    }

    void OutOfFuel() {
        outOfFuel = true;
        OnFuelDepleted?.Invoke();
    }

    void Refuel() {
        outOfFuel = false;
        currentFuel = maxFuel;
        UpdateFuelGauge();
    }

    void DepleteFuel(float magnitude) => currentFuel -= magnitude * fuelDepletionRate * Time.deltaTime;
    void UpdateFuelGauge() => fuelGauge.value = currentFuel;
}
