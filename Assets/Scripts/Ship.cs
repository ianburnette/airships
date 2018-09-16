using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(SpriteRenderer))]
public class Ship : MonoBehaviour {

	[SerializeField] Sprite shipSprite;
	[SerializeField] SpriteRenderer shipSpriteRenderer;

	[SerializeField] public bool playerShip;

	[SerializeField] public string shipName;
	[Range(.1f,1f)] [SerializeField] public float capacity;
	[Range(0,1)] [SerializeField] public float fuelEfficiency;
	[Range(0,1)] [SerializeField] public float handling;
	[Range(0,1)] [SerializeField] public float boostEfficiency;

	[SerializeField] public int cost;
	[SerializeField] public Vector2 canvasOffset;

	void OnEnable() => shipSpriteRenderer = GetComponent<SpriteRenderer>();

	void Update() {
		if (shipSpriteRenderer.sprite != shipSprite)
			shipSpriteRenderer.sprite = shipSprite;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (!playerShip) ShipCanvas.instance.Activate(true, this);
	}

	void OnTriggerExit2D(Collider2D other) {
		if (!playerShip) ShipCanvas.instance.Activate(false, null);
	}

	public void Collide() {
		if (playerShip)
			PlayerFuel.instance.Collide();
	}
}
