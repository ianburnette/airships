using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
	[SerializeField] Animator anim;

	void OnEnable() => PlayerLand.OnLandingStateChange += UpdateLandingState;
	void OnDisable() => PlayerLand.OnLandingStateChange -= UpdateLandingState;

	void UpdateLandingState(bool state) => anim.SetBool("landingState", state);
}
