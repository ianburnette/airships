using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
    public delegate void MovementInput(Vector2 input);
    public static MovementInput OnMovement;

    public delegate void InteractInput();
    public static InteractInput OnInteract;

    void Update() {
        GetMovementInput();
        GetInteractInput();
    }

    void GetInteractInput() {
        if (Input.GetButtonDown("Interact"))
            OnInteract?.Invoke();
    }

    void GetMovementInput() => OnMovement?.Invoke(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
}
