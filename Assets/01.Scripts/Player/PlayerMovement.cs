using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float speed = 3;
    [SerializeField] private float jumpPower = 10;
    [SerializeField] private LayerMask WhatIsGround;

    private Vector2 direction = Vector2.zero;

    private bool isStopped = false;
    private void Awake()
    {
        _rb = (Rigidbody2D)GetComponent("Rigidbody2D");
    }

    public void OnConnect_Dash()
    {
        if(isStopped) return;
        isStopped = true;

        Debug.Log("OnConnect_Dash");
        _rb.velocity = Vector3.right * 100;
    }
    public void OnConnect_Jump()
    {
        if (Physics2D.Raycast(transform.position, Vector3.down, .7f))
            _rb.velocity = Vector3.up * jumpPower;
    }
    public void OnConnect_Movement(Vector2 value)
    {
        direction = value;
    }

    private void FixedUpdate()
    {
        if (isStopped == false)
            _rb.velocity = new Vector2(direction.x * speed, _rb.velocity.y);
    }
}
