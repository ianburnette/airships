using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFogMasking : MonoBehaviour {

    [SerializeField] GameObject fogMaskPrefab;
    [SerializeField] Transform fogParent;
    [SerializeField] float fogCheckIncrement;

    public bool currentlyInMask = false;

    void OnEnable() => InvokeRepeating(nameof(CheckFog), fogCheckIncrement, fogCheckIncrement);
    void CheckFog() => StartCoroutine(CheckFogCoroutine());

    public IEnumerator CheckFogCoroutine() {
        currentlyInMask = false;
        yield return new WaitForEndOfFrame();
        //if (!currentlyInMask)
        //    CreateNewMask();
    }

    void CreateNewMask() => Instantiate(fogMaskPrefab, transform.position, transform.rotation, fogParent);
    void OnTriggerStay(Collider other) => currentlyInMask = true;
}
