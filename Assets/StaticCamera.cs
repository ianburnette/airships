using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class StaticCamera : MonoBehaviour {
    public static StaticCamera instance;
    CinemachineVirtualCamera CMVcam;
    public CinemachineFramingTransposer transposer;

    public float normalFollowSpeed, boostFollowSpeed;

    void OnEnable() {
        PlayerMovement.OnBoost += BoostCam;
        PlayerMovement.OnBoostStop += NormalCam;
    }

    void OnDisable() {
        PlayerMovement.OnBoost -= BoostCam;
        PlayerMovement.OnBoostStop -= NormalCam;
    }

    void Start() {
        CMVcam = GetComponent<CinemachineVirtualCamera>();
        transposer = CMVcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        instance = this;
    }

    public void NewTarget(Transform newTarget) => CMVcam.m_Follow = newTarget;

    void BoostCam(float efficiency) {
        transposer.m_LookaheadSmoothing = boostFollowSpeed;
    }

    void NormalCam() {
        transposer.m_LookaheadSmoothing = normalFollowSpeed;
    }
}
