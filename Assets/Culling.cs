using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Culling : MonoBehaviour {
    public List<GameObject> prefabs;
    public bool cullState;
    public Vector2Int coords;
    public Behaviour[] tilemapComponents;
    public TilemapRenderer tilemapRenderer;

    void OnEnable() {
      //  StartCoroutine(CullCoroutine(false));
    }

    public void Cull(bool state) {
        //StartCoroutine(CullCoroutine(state));
        CullImmediate(state);
    }

    public void CullImmediate(bool state) {
        cullState = state;
        foreach (var go in prefabs)
            if (go != null)
                go.SetActive(state);
        foreach (var co in tilemapComponents)
            co.enabled = state;
        cullState = !state;
        tilemapRenderer.enabled = state;
    }

    IEnumerator CullCoroutine(bool activeState) {
        cullState = activeState;
        foreach (var go in prefabs) {
            if (go != null)
                go.SetActive(activeState);
            yield return new WaitForEndOfFrame();
        }

        foreach (var co in tilemapComponents)
            co.enabled = activeState;

        tilemapRenderer.enabled = activeState;

        cullState = !activeState;

    }
}
