﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ProBuilder2.Common;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class MapGeneration : MonoBehaviour {

	public static MapGeneration instance;

	public bool generate, setup, includePrefabs;

	[SerializeField] Texture2D mapTexture;
	[SerializeField] Tilemap tilemap;
	[SerializeField] Vector3Int tilemapChunkSize = new Vector3Int(20, 20,0);

	[SerializeField] GameObject tilemapPrefab;
	[SerializeField] List<TileBase> tiles;
	[SerializeField] List<Color> colorsInMap;
	[SerializeField] List<ColorTileDict> colorTileDict;
	[SerializeField] List<GameObject> tilePrefabs;
	[SerializeField] List<GameObject> tileChunks;
	[SerializeField] Transform prefabParent;
	[SerializeField] GameObject pickup;
	[SerializeField] int pickupIndex;
	[SerializeField] Vector3 prefabTileOffset;
	[SerializeField] float rotationVariance;
	[SerializeField] float positionVariance;
	[SerializeField] bool generating;
	[SerializeField] float waitTime;
	[SerializeField] int settlementIndex;
	[SerializeField] int groundIndex;
	[SerializeField] PlayerCulling playerCulling;

	public List<GameObject> pickups;

	void OnEnable() {
		instance = this;
		playerCulling.cullMapsGrid = new Culling [mapTexture.width/tilemapChunkSize.x, mapTexture.height/tilemapChunkSize.y];
		foreach (var chunk in tileChunks) playerCulling.NewMap(chunk.GetComponent<Culling>());
	}

	void Update() {
		if (generate) {
			generating = true;
			ClearOldChunks();
			CreateDict();
			Generate();
			generate = false;
		}

		if (setup) Setup();
	}

	void ClearOldChunks() {
		foreach (var go in tileChunks) DestroyImmediate(go);
		tileChunks.Clear();
	}

	void Setup() {
		colorsInMap.Clear();
		for (var i = 0; i < mapTexture.width; i++)
			for (var j = 0; j < mapTexture.height; j++) {
				var col = mapTexture.GetPixel(i, j);
				if (!colorsInMap.Contains(col))
					colorsInMap.Add(col);
			}

		setup = false;
	}

	void CreateDict() {
		colorTileDict = new List<ColorTileDict>(colorsInMap.Count);
		for (var i = 0; i < colorsInMap.Count; i++) colorTileDict.Add(new ColorTileDict(colorsInMap[i], tiles[i]));
	}

	void Generate() {
		var chunkWidth = tilemapChunkSize.x;
		var chunkHeight = tilemapChunkSize.y;

		var gridWidth = mapTexture.width / chunkWidth;
		var gridHeight = mapTexture.height / chunkHeight;
		playerCulling.SetupMapGrid(gridWidth, gridHeight);


		for (var i = 0; i<gridWidth; i++) {
			for (var j = 0; j < gridHeight; j++) {
				var newMap = Instantiate(tilemapPrefab, new Vector2(i * chunkWidth, j * chunkHeight),
				                         Quaternion.identity, transform);
				var thisTileMap = newMap.GetComponent<Tilemap>();
				thisTileMap.size = tilemapChunkSize;
				newMap.name = "tilemap: " + i + ", " + j;
				var tilemapCulling = newMap.GetComponent<Culling>();
				for (var k = 0; k < chunkWidth; k++){
					for (var l = 0; l < chunkHeight; l++) {
						var tile = GetTileFromMap(k + i * chunkWidth, l + j * chunkHeight);

						var position = new Vector3Int(k, l, 1);
						thisTileMap.SetTile(position, tile);
						thisTileMap.RefreshTile(position);
						if (tile != tiles[groundIndex] && tile != tiles[settlementIndex] && includePrefabs) {
							tilemapCulling.prefabs.Add(CreateTree(k + i * chunkWidth, l + j * chunkHeight, 0,
							                                      newMap.transform));
						}

						if (tile == tiles[pickupIndex]) {
							tilemapCulling.prefabs.Add(CreatePickup(k + i * chunkWidth, l + j * chunkHeight,
							                                        newMap.transform));
						}
					}
				}

				tilemapCulling.CullImmediate(true);
				tilemapCulling.coords = new Vector2Int(i, j);
				playerCulling.NewMap(tilemapCulling);
				tileChunks.Add(newMap);
			}
		}

		generating = false;
	}

	GameObject CreatePickup(int i, int j, Transform parent) {
		var thisPickup = Instantiate(pickup, parent);
		PlacePrefab(i, j, thisPickup);
		thisPickup.GetComponent<Pickup>().index = pickups.Count;
		pickups.Add(thisPickup);
		return thisPickup;
	}

	GameObject CreateTree(int i, int j, int treeType, Transform parent) {
		var thisPrefab = Instantiate(tilePrefabs[treeType], parent);
		PlacePrefab(i, j, thisPrefab);
		RotatePrefab(thisPrefab);
		thisPrefab.SetActive(false);
		return thisPrefab;
	}

	void RotatePrefab(GameObject thisPrefab) {
		thisPrefab.transform.rotation = Quaternion.Euler
			(UnityEngine.Random.Range(-rotationVariance, rotationVariance),
			 UnityEngine.Random.Range(-rotationVariance, rotationVariance),
			 UnityEngine.Random.Range(-360, 360));
	}

	void PlacePrefab(int i, int j, GameObject thisPrefab) =>
		thisPrefab.transform.position = new Vector3(i + UnityEngine.Random.Range(-positionVariance, positionVariance),
		                                            j + UnityEngine.Random.Range(-positionVariance, positionVariance),
	                                                                                                     0) +
		                                prefabTileOffset;

	public void UpdatePickedUpPickups(List<int> alreadyPickedUp) {
		foreach (var t in alreadyPickedUp)
			pickups[t].SetActive(false);
	}

	TileBase GetTileFromMap(int i, int j) => (colorTileDict.First(c => c.color == mapTexture.GetPixel(i, j)).tile);
}

[Serializable]
class ColorTileDict {
	public Color color;
	public TileBase tile;

	public ColorTileDict(Color col, TileBase tile) {
		color = col;
		this.tile = tile;
	}
}
