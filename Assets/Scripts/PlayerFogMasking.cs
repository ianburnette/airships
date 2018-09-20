using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerFogMasking : MonoBehaviour {

    public bool resetFog;
    public bool revertToSavedFog;

    [SerializeField] Texture2D fogTexture;

    [SerializeField] RenderTexture renderTexture;
    [SerializeField] Material mat;
    [SerializeField] Renderer renderer;

    [SerializeField] float castDist;
    [SerializeField] LayerMask fogMask;
    [SerializeField] Color fogClearColor;

    [FormerlySerializedAs("fogClearingTexture")] [SerializeField] Texture2D fogBrush;

    public Vector2 hitTexCoords;
    [SerializeField] Color[] brush1dimensional;
    [SerializeField] Color[,] brush2dimensional;

    Color[] savedMap;

    void OnEnable() {
        //fogTexture = new Texture2D (renderTexture.width, renderTexture.height);
        //fogTexture.width = renderTexture.width;
        //fogTexture.height = renderTexture.height;
        //fogTexture.filterMode = FilterMode.Point;
        renderer.material.mainTexture = fogTexture;
        SetBrush();
        Settlement.OnExitSettlement += SaveMap;
    }

    void OnDisable() {
        Settlement.OnExitSettlement -= SaveMap;
    }

    void SetBrush() {
        brush1dimensional = new Color[fogBrush.width * fogBrush.height];
        for (var x = 0; x < fogBrush.height; x++) {
            for (var y = 0; y < fogBrush.width; y++) {
                var a = fogBrush.GetPixel(x, y).a;
                brush1dimensional[(x * fogBrush.height) + y] = a > 0 ? fogBrush.GetPixel(x, y) : Color.clear;
            }
        }
    }

    void Update() {
        if (resetFog)
            ResetFog();
        if (revertToSavedFog)
            RevertToSavedFog();
        SetupTextureInput();
        var hit = GetHit();
        if (hit.transform == null) {
            WriteAndCloseTextureInput();
            return;
        }
        hitTexCoords = hit.textureCoord;
        DebugFogHit(hit);
        ApplyTextureFromBrush(hitTexCoords);
        WriteAndCloseTextureInput();
    }


    void ResetFog() {
        SetupTextureInput();
        var cols = new Color[fogTexture.width * fogTexture.height];
        for (var i = 0; i<cols.Length; i++) cols[i] = Color.white;
        fogTexture.SetPixels(0, 0, fogTexture.width,fogTexture.height, cols);
        WriteAndCloseTextureInput();
        resetFog = false;
    }

    static void DebugFogHit(RaycastHit hit) {
        Debug.DrawRay(hit.point, Vector2.left / 3, Color.green);
        Debug.DrawRay(hit.point, Vector2.right / 3, Color.green);
        Debug.DrawRay(hit.point, Vector2.up / 3, Color.green);
        Debug.DrawRay(hit.point, Vector2.down / 3, Color.green);
    }

    void ApplyTextureFromBrush(Vector2 hitTextureCoord) {
        var currentRow = new List<Color>();
        for (var x = 0; x < fogBrush.width; x++) {
            for (var y = 0; y < fogBrush.height; y++) {
                var a = fogBrush.GetPixel(x, y);
                if (a.a < 1)
                    currentRow.Add(new Color(a.a, a.a, a.a, a.a));
            }
            SetPixels(x, hitTextureCoord, currentRow);
            currentRow.Clear();
        }
    }

    void ApplyTextureFromSaved() {
        fogTexture.SetPixels(0, 0, fogTexture.width, fogTexture.height, savedMap);
    }

    void SetPixels(int row, Vector2 hitTextureCoord, List<Color> currentRow) {
        var colors = new Color[currentRow.Count];
        //print("column count for this row is " + colors.Length);
        for (var i = 0; i < currentRow.Count; i++) colors[i] = currentRow[i];

        var xStart = ((((int)(hitTextureCoord.x * fogTexture.width)) - fogBrush.width / 2) - currentRow.Count / 2) +
                     fogBrush.width / 2;
        var yStart = ((int)(hitTextureCoord.y * fogTexture.height) - fogBrush.height / 2) + row;

        fogTexture.SetPixels(xStart, yStart, currentRow.Count, 1, colors);
    }

    void WriteAndCloseTextureInput() {
        fogTexture.Apply();
        RenderTexture.active = null;
    }

    void SetupTextureInput() => RenderTexture.active = renderTexture;

    RaycastHit GetHit() {
        RaycastHit hit;
        Debug.DrawRay(transform.position - (transform.forward/3), transform.forward, Color.magenta);
        Physics.Raycast(transform.position, Vector3.forward, out hit, castDist, fogMask);
        return hit;
    }

    void SaveMap() {
        var colorList = new Color[fogTexture.width * fogTexture.height];
        for (var column = 0; column < fogTexture.width; column++) {
            for (var row = 0; row < fogTexture.height; row++) {
                colorList[(column * fogTexture.height) + (row)] = fogTexture.GetPixel(column, row);
            }
        }

        savedMap = colorList;
    }

    void RevertToSavedFog() {
        ApplyTextureFromSaved();
        revertToSavedFog = false;
    }
}
