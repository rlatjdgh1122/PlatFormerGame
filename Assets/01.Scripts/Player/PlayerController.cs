using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;

    [Header("함수 연결")]
    [SerializeField] private UnityEvent<Vector2> OnMovement = null;
    [SerializeField] private UnityEvent<Vector2> OnDash = null;
    [SerializeField] private UnityEvent OnJump = null;
    private void Start()
    {
        _inputReader.MovementEvent += OnHandleMovement;
        _inputReader.JumpEvent += OnHandleJump;
        _inputReader.DashEvent += OnHandleDash;
    }

    private void OnHandleDash(Vector2 value)
    {
        Debug.Log("OnHandleDash");

        OnDash?.Invoke(value);
    }
    private void OnHandleJump(bool value)
    {
        if (value == true)
        {
            OnJump?.Invoke();
        }
        else if (value == false)
        {

        }
    }

    private void OnHandleMovement(Vector2 value)
    {
        Debug.Log("움직여라");
        OnMovement?.Invoke(value);
    }
    private void OnDestroy()
    {
        _inputReader.MovementEvent -= OnHandleMovement;
        _inputReader.JumpEvent -= OnHandleJump;
        _inputReader.DashEvent -= OnHandleDash;
    }
}
