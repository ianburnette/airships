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
	[SerializeField] List<TileBase> tiles;
	[SerializeField] List<Color> colorsInMap;
	[SerializeField] List<ColorTileDict> colorTileDict;
	[SerializeField] List<GameObject> tilePrefabs;
	[SerializeField] Transform prefabParent;
	[SerializeField] Vector3 prefabTileOffset;
	[SerializeField] float rotationVariance;

	void Update() {
		if (generate) {
			CreateDict();
			Generate();
		}

		if (setup) {
			Setup();
		}
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
		tilemap.ClearAllTiles();
		var prevPar = prefabParent.parent;
		var prevPos = prefabParent.position;
		DestroyImmediate(prefabParent);
		var parent = new GameObject("prefabParent");
		parent.transform.parent = prevPar;
		parent.transform.position = prevPos;

		//foreach (Transform child in prefabParent)
		//	DestroyImmediate(child.gameObject);
		tilemap.size = new Vector3Int(mapTexture.width, mapTexture.height, 1);
		for (var i=0; i<mapTexture.width;i++)
			for (var j = 0; j < mapTexture.height; j++) {
				var tile = GetTileFromMap(i, j);
				tilemap.SetTile(new Vector3Int(i, j, 1), tile);
				if (tile == tiles[0] || tile == tiles[1]) CreateTree(i, j, 0);
				if (tile == tiles[2]) CreateTree(i, j, 1);
			}
		tilemap.RefreshAllTiles();
		generate = false;
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
