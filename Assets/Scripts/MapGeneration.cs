using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class MapGeneration : MonoBehaviour {

	public bool generate, setup;

	[SerializeField] Texture2D mapTexture;
	[SerializeField] Tilemap tilemap;
	[SerializeField] Vector3Int tilemapChunkSize = new Vector3Int(20, 20,0);

	[SerializeField] GameObject tilemapPrefab;
	[SerializeField] List<TileBase> tiles;
	[SerializeField] List<Color> colorsInMap;
	[SerializeField] List<ColorTileDict> colorTileDict;
	[SerializeField] List<GameObject> tilePrefabs;
	[SerializeField] Transform prefabParent;
	[SerializeField] Vector3 prefabTileOffset;
	[SerializeField] float rotationVariance;
	[SerializeField] bool generating;
	[SerializeField] float waitTime;

	void Update() {
		if (generate) {
			generating = true;
			CreateDict();
			Generate();
			generate = false;
		}

		if (setup) Setup();
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
		for (var i = 0; i<mapTexture.width / chunkWidth; i++)
			for (var j = 0; j < mapTexture.height / chunkHeight; j++) {
				var newMap = Instantiate(tilemapPrefab, new Vector2(i * chunkWidth, j * chunkHeight),
				                         Quaternion.identity, transform);
				var thisTileMap = newMap.GetComponent<Tilemap>();
				thisTileMap.size = tilemapChunkSize;
				thisTileMap.transform.name = "tilemap: " + i + ", " + j;
				for (var k = 0; k < chunkWidth; k++){
					for (var l = 0; l < chunkHeight; l++) {
						var tile = GetTileFromMap(k + i * chunkWidth, l + j * chunkHeight);

						var position = new Vector3Int(k, l, 1);
						thisTileMap.SetTile(position, tile);
						thisTileMap.RefreshTile(position);
						//if (tile == tiles[0] || tile == tiles[1]) CreateTree(i, j, 0);
						//if (tile == tiles[2]) CreateTree(i, j, 1);
					}
			}
		}
		generating = false;
	}

	void CreateTree(int i, int j, int treeType) {
		var thisPrefab = Instantiate(tilePrefabs[treeType], prefabParent);
		thisPrefab.transform.position = new Vector3(i, j, 0) + prefabTileOffset;
		thisPrefab.transform.rotation = Quaternion.Euler
			(UnityEngine.Random.Range(-rotationVariance, rotationVariance),
			 UnityEngine.Random.Range(-rotationVariance, rotationVariance),
			 UnityEngine.Random.Range(-360,360));
	}

	TileBase GetTileFromMap(int i, int j) =>
		colorTileDict.First(c => c.color == mapTexture.GetPixel(i, j)).tile;
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
