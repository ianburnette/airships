using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour {
	[SerializeField] PlayerMovement playerMovement;
	[SerializeField] PlayerLand playerLand;

	void OnEnable() {
		PlayerInput.OnInteract += AttemptToggleLandingState;
		PlayerFuel.OnFuelDepleted += OutOfFuel;
	}

	void OnDisable() {
		PlayerInput.OnInteract -= AttemptToggleLandingState;
		PlayerFuel.OnFuelDepleted -= OutOfFuel;
	}

	void AttemptToggleLandingState() {
		if (playerMovement.enabled && playerLand.InLandingZone && !playerLand.LandingState) Land(true);
		//else if (!playerMovement.enabled && playerLand.LandingState) Land(false);
	}

	void Land(bool state) {
		playerLand.ToggleLandingState(state);
		playerMovement.enabled = !state;
		playerLand.enabled = !state;
	}

	public void TakeOff() {
		Land(false);
	}

	void OutOfFuel() {
		if (playerMovement.enabled && !playerLand.LandingState && !playerLand.InLandingZone) {
			Land(true);
			Invoke(nameof(RestartIfNotInLandingZoneAfterLanding), 3);
		}
	}

	void RestartIfNotInLandingZoneAfterLanding() {
		if (!playerLand.InLandingZone)
			GameSceneManagement.Restart();
	}
}
