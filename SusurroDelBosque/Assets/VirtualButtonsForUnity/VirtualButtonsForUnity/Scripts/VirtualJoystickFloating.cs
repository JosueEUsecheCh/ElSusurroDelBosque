using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class VirtualJoystickFloating : VirtualJoystick
{
    private Vector2Control targetVectorControl; 

    [SerializeField] private bool hideOnPointerUp = false;
    [SerializeField] private bool centralizeOnPointerUp = true;

    protected override void Awake()
    {
        base.Awake(); 

        joystickType = VirtualJoystickType.Floating;
        _hideOnPointerUp = hideOnPointerUp;
        _centralizeOnPointerUp = centralizeOnPointerUp;

        if (handleStickController != null && !string.IsNullOrEmpty(handleStickController.controlPath))
        {
            targetVectorControl = InputSystem.FindControl(handleStickController.controlPath) as Vector2Control;
            
            if (targetVectorControl == null)
            {
                Debug.LogError($"VirtualJoystickFloating: No se pudo encontrar el control Vector2 en la ruta: {handleStickController.controlPath}.");
            }
        }
    }

    public float Horizontal 
    {
        get
        {
            if (targetVectorControl != null)
            {
                return targetVectorControl.ReadValue().x;
            }
            return 0f;
        }
    }
    
    public float Vertical 
    {
        get
        {
            if (targetVectorControl != null)
            {
                return targetVectorControl.ReadValue().y;
            }
            return 0f;
        }
    }
}