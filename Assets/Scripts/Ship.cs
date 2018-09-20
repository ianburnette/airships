using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

[System.Serializable]
[RequireComponent(typeof(SpriteRenderer))]
public class Ship : MonoBehaviour {

	[SerializeField] Sprite shipSprite;
	[SerializeField] SpriteRenderer shipSpriteRenderer;

	[SerializeField] SpriteRenderer shipShadowRenderer;
	[SerializeField] Transform shipShadow;
	[SerializeField] float shadowDistance;
	[SerializeField] Vector2 shadowOffsetDirection;

	[SerializeField] public bool playerShip;

	[SerializeField] public string shipName;
	[Range(.1f,1f)] [SerializeField] public float capacity;
	[Range(0,1)] [SerializeField] public float fuelEfficiency;
	[Range(0,1)] [SerializeField] public float handling;
	[Range(0,1)] [SerializeField] public float boostEfficiency;

	[SerializeField] public int cost;
	[SerializeField] public Vector2 canvasOffset;

	void OnEnable() {
		SetupSprites();
	}

	void SetupSprites() {
		shipSpriteRenderer = GetComponent<SpriteRenderer>();
		shipShadowRenderer = shipShadow.GetComponent<SpriteRenderer>();
		shipSpriteRenderer.sprite = shipSprite;
		shipShadowRenderer.sprite = shipSprite;
	}

	void Update() {
		if (shipSpriteRenderer.sprite != shipSprite)
			shipSpriteRenderer.sprite = shipSprite;
	}

	void LateUpdate() {
		SetShadowPosition();
	}

	void SetShadowPosition() =>
		shipShadow.position = (Vector2)transform.position + (shadowOffsetDirection * shadowDistance);

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
