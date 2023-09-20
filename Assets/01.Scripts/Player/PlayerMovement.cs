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

    private bool isStopped = false;
    private void Awake()
    {
        _rb = (Rigidbody2D)GetComponent("Rigidbody2D");
    }

    public void OnConnect_Dash(Vector2 value)
    {
        Debug.Log("OnConnect_Dash");

        StopImmediately();
        StartCoroutine(Dash_Co(value));
    }

    private IEnumerator Dash_Co(Vector2 value)
    {
        isStopped = true;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, value.normalized, dashPower, WhatIsGround);
        if (hit)
        {
            _rb.velocity = (Vector2)transform.position - (hit.point.normalized * dashPower);
        }
        else
            _rb.velocity = (Vector2)transform.position - (value.normalized * dashPower);

        yield return new WaitForSeconds(.5f);

        isStopped = false;
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
    private void FixedUpdate()
    {
        if (isStopped == false)
            _rb.velocity = new Vector2(direction.x * speed, _rb.velocity.y);
    }
}