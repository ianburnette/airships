using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class Pickup : MonoBehaviour {

	Transform target;
	[SerializeField] float speed;
	[SerializeField] float distanceThreshold;
	public int index;

	void OnTriggerEnter2D(Collider2D other) {
		if (PlayerInventory.staticPlayerInventory.AttemptToAddItemToInventory(index)) target = other.transform;
	}

	void Update() {
		if (!target) return;
		transform.position = Vector2.Lerp(transform.position, target.position, speed * Time.deltaTime);
		if (Vector2.Distance(transform.position, target.position) < distanceThreshold)
			DestroySelf();
	}

	void DestroySelf() => Destroy(gameObject); //TODO set up pooling
}
