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
		tilemap.size = new Vector3Int(mapTexture.width, mapTexture.height, 1);
		for (var i=0; i<mapTexture.width;i++)
			for (var j = 0; j < mapTexture.height; j++) {
				tilemap.SetTile(new Vector3Int(i, j, 1), GetTileFromMap(i, j));
			}
		//tilemap.
		tilemap.RefreshAllTiles();
		generate = false;
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
