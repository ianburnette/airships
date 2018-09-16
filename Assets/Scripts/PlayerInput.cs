using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
    public delegate void MovementInput(Vector2 input);
    public static MovementInput OnMovement;

    public delegate void InteractInput();
    public static InteractInput OnInteract;


    public delegate void InteractInputEnd();
    public static InteractInputEnd OnInteractEnd;

    void Update() {
        GetMovementInput();
        GetInteractInput();
    }

    void GetInteractInput() {
        if (Input.GetButtonDown("Interact"))
            OnInteract?.Invoke();
        if (Input.GetButtonUp("Interact"))
            OnInteractEnd?.Invoke();
    }

    void GetMovementInput() => OnMovement?.Invoke(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
}
