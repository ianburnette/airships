using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour {
	[SerializeField] PlayerMovement playerMovement;
	[SerializeField] PlayerLand playerLand;

	bool inSettlement;

	public delegate void LandingStateChange(bool state);
	public static LandingStateChange onLandingStateChange;

	void OnEnable() {
		PlayerFuel.OnFuelDepleted += OutOfFuel;
		Settlement.OnEnterSettlement += OnEnteredSettlement;
		Settlement.OnExitSettlement += OnExitedSettlement;
	}

	void OnDisable() {
		PlayerFuel.OnFuelDepleted -= OutOfFuel;
		Settlement.OnEnterSettlement -= OnEnteredSettlement;
		Settlement.OnExitSettlement -= OnExitedSettlement;
	}

	void OnEnteredSettlement() => inSettlement = true;
	void OnExitedSettlement() => inSettlement = false;

	void Land(bool state) {
		onLandingStateChange(state);
		playerMovement.enabled = !state;
	}

	public void TakeOff() {
		Land(false);
	}

	void OutOfFuel() {
		if (playerMovement.enabled && !inSettlement) {
			Land(true);
			Invoke(nameof(RestartIfNotInLandingZoneAfterLanding), 3);
		}
	}

	void RestartIfNotInLandingZoneAfterLanding() {
		if (!inSettlement)
			GameSceneManagement.Restart();
	}

	public void RestartNow() {
		GameSceneManagement.Restart();
	}
}
