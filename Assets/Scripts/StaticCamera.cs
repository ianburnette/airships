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

    public float changeSpeed;

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
        DOTween.To(() => (float)transposer.m_LookaheadSmoothing, x => transposer.m_LookaheadSmoothing = x, boostFollowSpeed, changeSpeed);
        DOTween.To(() => (float)transposer.m_LookaheadTime, x => transposer.m_LookaheadTime = x, boostLookaheadTime, changeSpeed);
        DOTween.To(() => (float)transposer.m_XDamping, x => transposer.m_XDamping = x, boostDamping.x, changeSpeed);
        DOTween.To(() => (float)transposer.m_YDamping, x => transposer.m_YDamping = x, boostDamping.y, changeSpeed);
        DOTween.To(() => (float)transposer.m_DeadZoneWidth, x => transposer.m_DeadZoneWidth = x, boostDeadZone.x, changeSpeed);
        DOTween.To(() => (float)transposer.m_DeadZoneHeight, x => transposer.m_DeadZoneHeight = x, boostDeadZone.y, changeSpeed);
        //transposer.m_LookaheadSmoothing = boostFollowSpeed;
        //transposer.m_LookaheadTime = boostLookaheadTime;
        //transposer.m_XDamping = boostDamping.x;
        //transposer.m_YDamping = boostDamping.y;
        //transposer.m_DeadZoneWidth = boostDeadZone.x;
        //transposer.m_DeadZoneHeight = boostDeadZone.y;
        //transposer.m_DeadZoneHeight =
    }

    void NormalCam() {
        DOTween.To(() => (float)transposer.m_LookaheadSmoothing, x => transposer.m_LookaheadSmoothing = x, normalFollowSpeed, changeSpeed);
        DOTween.To(() => (float)transposer.m_LookaheadTime, x => transposer.m_LookaheadTime = x, normalLookaheadTime, changeSpeed);
        DOTween.To(() => (float)transposer.m_XDamping, x => transposer.m_XDamping = x, normalDamping.x, changeSpeed);
        DOTween.To(() => (float)transposer.m_YDamping, x => transposer.m_YDamping = x, normalDamping.y, changeSpeed);
        DOTween.To(() => (float)transposer.m_DeadZoneWidth, x => transposer.m_DeadZoneWidth = x, normalDeadZone.x, changeSpeed);
        DOTween.To(() => (float)transposer.m_DeadZoneHeight, x => transposer.m_DeadZoneHeight = x, normalDeadZone.y, changeSpeed);
        //transposer.m_LookaheadSmoothing = normalFollowSpeed;
        //transposer.m_LookaheadTime = normalLookaheadTime;
        //transposer.m_XDamping = normalDamping.x;
        //transposer.m_YDamping = normalDamping.y;
        //transposer.m_DeadZoneWidth = normalDeadZone.x;
        //transposer.m_DeadZoneHeight = normalDeadZone.y;
    }
}
