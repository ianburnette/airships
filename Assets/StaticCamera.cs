using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class StaticCamera : MonoBehaviour {
    public static StaticCamera instance;
    CinemachineVirtualCamera CMVcam;
    // Start is called before the first frame update
    void Start() {
        CMVcam = GetComponent<CinemachineVirtualCamera>();
        instance = this;
    }

    public void NewTarget(Transform newTarget) => CMVcam.m_Follow = newTarget;
}
