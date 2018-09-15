using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
	[SerializeField] Animator anim;

	void OnEnable() {
		PlayerStateMachine.onLandingStateChange += UpdateLandingState;
		PlayerShip.OnShipAnimatorChanged += ChangeShipAnimator;
	}

	void OnDisable() {
		PlayerStateMachine.onLandingStateChange -= UpdateLandingState;
		PlayerShip.OnShipAnimatorChanged -= ChangeShipAnimator;
	}

	void Start() => anim.SetBool("landingState", false);

	void UpdateLandingState(bool state) => anim.SetBool("landingState", state);
	void ChangeShipAnimator(Animator newAnim) {
		UpdateLandingState(true);
		anim = newAnim;
		UpdateLandingState(false);
	}
}
