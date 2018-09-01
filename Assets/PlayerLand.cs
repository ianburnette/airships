using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLand : MonoBehaviour {
    [SerializeField] bool inLandingZone;
    [SerializeField] bool landingState;
    public bool InLandingZone { get { return inLandingZone; } set { inLandingZone = value; } }
    public bool LandingState { get { return landingState; } set { landingState = value; } }

    public delegate void LandingStateChange(bool state);
    public static event LandingStateChange OnLandingStateChange;

    public delegate void LandingAtSettlement();
    public static event LandingAtSettlement OnLandingAtSettlement;

    void OnEnable() {

    }

    public void ToggleLandingState(bool state) {
        landingState = state;
        OnLandingStateChange?.Invoke(landingState);
        if (landingState)
            LandAtSettlement();
    }

    void LandAtSettlement() => OnLandingAtSettlement?.Invoke();
}
