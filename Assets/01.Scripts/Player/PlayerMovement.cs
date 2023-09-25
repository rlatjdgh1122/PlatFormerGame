using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float speed = 3;
    [SerializeField] private float jumpPower = 10;
    [SerializeField] private float dashPower = 10;

    [SerializeField] private LayerMask WhatIsGround;

    private Vector2 direction = Vector2.zero;

    private bool isDash = false;
    private void Awake()
    {
        _rb = (Rigidbody2D)GetComponent("Rigidbody2D");
    }
    private void FixedUpdate()
    {
        if (isDash)
            _rb.velocity = direction.normalized * dashPower;
        else
            _rb.velocity = new Vector2(direction.x * speed, _rb.velocity.y);
    }
    public void OnConnect_Dash(Vector2 value)
    {
        Debug.Log("OnConnect_Dash");

        StopImmediately();
        OnConnect_Movement(value);

        StartCoroutine(Co_Dash());
    }

    private IEnumerator Co_Dash()
    {
        isDash = true;
        yield return new WaitForSeconds(.5f);
        StopImmediately();
        isDash = false;

    }

    public void OnConnect_Jump()
    {
        if (Physics2D.Raycast(transform.position, Vector3.down, .5f))
            _rb.velocity = Vector3.up * jumpPower;
    }
    public void OnConnect_Movement(Vector2 value)
    {
        direction = value;
    }

    public void StopImmediately()
    {
        direction = Vector2.zero;
    }

}