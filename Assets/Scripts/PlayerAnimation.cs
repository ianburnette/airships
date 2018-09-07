using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
	[SerializeField] Animator anim;

	void OnEnable() {
		PlayerLand.OnLandingStateChange += UpdateLandingState;
		PlayerShip.OnShipChanged += ChangeShip;
	}

	void OnDisable() {
		PlayerLand.OnLandingStateChange -= UpdateLandingState;
		PlayerShip.OnShipChanged -= ChangeShip;
	}

	void Start() => anim.SetBool("landingState", false);

	void UpdateLandingState(bool state) => anim.SetBool("landingState", state);
	void ChangeShip(Animator newAnim) => anim = newAnim;
}
