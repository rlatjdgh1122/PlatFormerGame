using Cinemachine;
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

    [Header("스크립트 연결")]
    [SerializeField] private CinemachineVirtualCamera Vcam = null;

    //private CameraManager CameraManagerCompo = null;

    private void Awake()
    {
        CameraManager.Instance = new CameraManager(transform, Vcam);
    }
    private void Start()
    {
        _inputReader.MovementEvent += OnHandleMovement;
        _inputReader.JumpEvent += OnHandleJump;
        _inputReader.DashEvent += OnHandleDash;
    }

    private void OnHandleDash(Vector2 value)
    {
        Debug.Log("OnHandleDash");
        Vector2 mousePos = CameraManager.Instance.Maincam.ScreenToWorldPoint(value);
        OnDash?.Invoke(mousePos);
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
