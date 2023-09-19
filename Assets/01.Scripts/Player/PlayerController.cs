using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;

    [Header("�Լ� ����")]
    [SerializeField] private UnityEvent<Vector2> OnMovement = null;
    [SerializeField] private UnityEvent OnJump = null;
    [SerializeField] private UnityEvent OnDash = null;
    private void Start()
    {
        _inputReader.MovementEvent += OnHandleMovement;
        _inputReader.JumpEvent += OnHandleJump;
        _inputReader.DashEvent += OnHandleDash;
    }

    private void OnHandleDash(bool value)
    {
        Debug.Log("OnHandleDash");
        if (value == true)
        {
            Debug.Log("OnHandleDash : true  ");
            OnDash?.Invoke();
        }
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
        Debug.Log("��������");
        OnMovement?.Invoke(value);
    }
    private void OnDestroy()
    {
        _inputReader.MovementEvent -= OnHandleMovement;
        _inputReader.JumpEvent -= OnHandleJump;
    }
}
