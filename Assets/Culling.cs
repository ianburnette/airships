using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Culling : MonoBehaviour {
    public GameObject[] prefabs;
    public bool cullState;

    public void Cull(bool state) {
        StartCoroutine(CullCoroutine(state));
    }

    public void CullImmediate(bool state) {
        cullState = state;
        foreach (GameObject go in prefabs)
            if (go != null)
                go.SetActive(state);
    }

    IEnumerator CullCoroutine(bool state) {
        cullState = state;
        foreach (GameObject go in prefabs) {
            if (go != null)
                go.SetActive(state);
            yield return new WaitForEndOfFrame();
        }

    }
}
