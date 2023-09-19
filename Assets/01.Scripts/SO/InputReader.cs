using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputSystem;

[CreateAssetMenu(menuName = "SO/Input/Reader", fileName = "New Input reader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    public event Action<Vector2> MovementEvent;
    public event Action<bool> JumpEvent;
    public event Action<Vector2> DashEvent;
    public Vector2 AimPostion { get; private set; }

    private InputSystem _input;
    private void OnEnable()
    {
        if (_input == null)
        {
            _input = new InputSystem();
            _input.Player.SetCallbacks(this);
        }
        _input.Player.Enable();
    }
    public void OnFirePos(InputAction.CallbackContext context)
    {
        Debug.Log("마우스 위치");
        AimPostion = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started) JumpEvent?.Invoke(true);
        else JumpEvent?.Invoke(false);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        Debug.Log("움직이세요");

        Vector2 value = context.ReadValue<Vector2>();
        MovementEvent?.Invoke(value);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        Debug.Log("OnDash");
        if (context.started) DashEvent?.Invoke(AimPostion);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {

    }
}
