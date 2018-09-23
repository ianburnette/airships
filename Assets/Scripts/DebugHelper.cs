using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour {
    public Transform player;
    public Transform cam;
    public Transform[] settlements;
    public PlayerFogMasking masking;
    public PlayerStateMachine stateMachine;
    public PlayerFuel fuel;
    public KeyCode[] settlementKeys;
    public GameObject fog;

    public void Update() {
        GetNormalSettlementMoveInput(Input.GetKey(KeyCode.LeftShift));
        if (Input.GetKeyDown(KeyCode.Backspace))
            ResetFog();
        if (Input.GetKeyDown(KeyCode.L))
            stateMachine.RestartNow();
        if (Input.GetKeyDown(KeyCode.P))
            fog.SetActive(!fog.activeSelf);
        if (Input.GetKeyDown(KeyCode.O))
            fuel.enabled = !fuel.enabled;
    }

    void GetNormalSettlementMoveInput(bool clearFog) {
        for (var i = 0; i < settlementKeys.Length; i++) {
            var code = settlementKeys[i];
            if (Input.GetKeyDown(code))
                TeleportToSettlement(i, clearFog);
        }
    }

    void TeleportToSettlement(int settlementIndex, bool clearFog) {
        player.position = new Vector3(settlements[settlementIndex].position.x, settlements[settlementIndex].position.y,
                                      player.position.z);
        cam.position = new Vector3(player.position.x, player.position.y, cam.position.z);
        if (clearFog)
            ResetFog();
    }

    void ResetFog() => masking.resetFog = true;
}
