using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLand : MonoBehaviour {
    [SerializeField] bool inLandingZone;

    public delegate void LandingStateChange(bool state);
    public static event LandingStateChange OnLandingStateChange;

    bool outOfFuel;

    void OnEnable() => PlayerFuel.OnFuelDepleted += OutOfFuel;
    void OnDisable() => PlayerFuel.OnFuelDepleted -= OutOfFuel;

    public void ToggleLandingState(bool state) {

    }

    void OutOfFuel() => outOfFuel = true;
}
