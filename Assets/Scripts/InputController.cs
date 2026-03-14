using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private Camera mainCam;
    public delegate void OnHit(RaycastHit hit);
    public event OnHit OnHitHappened;
    public InputAction tapPressAction;

    private void Awake()
    {
        mainCam = Camera.main;
        tapPressAction.performed += OnTap;
    }

    private void OnEnable()
    {
        tapPressAction.Enable();
    }

    private void OnDisable()
    {
        tapPressAction.Disable();
        tapPressAction.performed -= OnTap;
    }

    private void OnTap(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = Vector2.zero;
        switch (context.control.device)
        {
            case Mouse:
                screenPosition = Mouse.current.position.ReadValue();
                break;
            case Touchscreen:
                screenPosition = Touchscreen.current.position.ReadValue();
                break;
            default:
                Debug.LogError("Unknown input used");
                break;
        }

        Ray ray = mainCam.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            OnHitHappened?.Invoke(hit);
        }
    }
}
