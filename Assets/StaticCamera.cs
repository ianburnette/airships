using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class StaticCamera : MonoBehaviour {
    public static StaticCamera instance;
    CinemachineVirtualCamera CMVcam;
    public CinemachineFramingTransposer transposer;

    public float normalFollowSpeed, boostFollowSpeed;
    public float normalLookaheadTime, boostLookaheadTime;
    public Vector2 normalDamping, boostDamping;
    public Vector2 normalDeadZone, boostDeadZone;

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
        transposer.m_LookaheadTime = boostLookaheadTime;
        transposer.m_XDamping = boostDamping.x;
        transposer.m_YDamping = boostDamping.y;
        transposer.m_DeadZoneWidth = boostDeadZone.x;
        transposer.m_DeadZoneHeight = boostDeadZone.y;
        //transposer.m_DeadZoneHeight =
    }

    void NormalCam() {
        transposer.m_LookaheadSmoothing = normalFollowSpeed;
        transposer.m_LookaheadTime = normalLookaheadTime;
        transposer.m_XDamping = normalDamping.x;
        transposer.m_YDamping = normalDamping.y;
        transposer.m_DeadZoneWidth = normalDeadZone.x;
        transposer.m_DeadZoneHeight = normalDeadZone.y;
    }
}
