using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(SpriteRenderer))]
public class Ship : MonoBehaviour {

	[SerializeField] Sprite shipSprite;
	[SerializeField] SpriteRenderer shipSpriteRenderer;

	[SerializeField] public string shipName;
	[Range(10,100)] [SerializeField] public int capacity;
	[Range(1f,20f)] [SerializeField] public float fuelDepletionRate;
	[Range(25,100)] [SerializeField] public int speed;
	[SerializeField] public float boostCost;
	[SerializeField] public int cost;

	void OnEnable() => shipSpriteRenderer = GetComponent<SpriteRenderer>();

	void Update() {
		if (shipSpriteRenderer.sprite != shipSprite)
			shipSpriteRenderer.sprite = shipSprite;
	}
}
