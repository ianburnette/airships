using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
	[SerializeField] Animator anim;

	void OnEnable() {
		PlayerLand.OnLandingStateChange += UpdateLandingState;
		PlayerShip.OnShipAnimatorChanged += ChangeShipAnimator;
	}

	void OnDisable() {
		PlayerLand.OnLandingStateChange -= UpdateLandingState;
		PlayerShip.OnShipAnimatorChanged -= ChangeShipAnimator;
	}

	void Start() => anim.SetBool("landingState", false);

	void UpdateLandingState(bool state) => anim.SetBool("landingState", state);
	void ChangeShipAnimator(Animator newAnim) => anim = newAnim;
}
