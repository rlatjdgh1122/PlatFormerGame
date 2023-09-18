using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float speed = 3;
    [SerializeField] private float jumpPower = 10;

    private Vector2 direction = Vector2.zero;
    private void Awake()
    {
        _rb = (Rigidbody2D)GetComponent("Rigidbody2D");
    }

    public void OnConnect_Jump()
    {
        _rb.velocity = Vector3.up * jumpPower;
    }
    public void OnConnect_Movement(Vector2 value)
    {
        direction = value;
    }

    private void FixedUpdate()
    {
        _rb.velocity = new Vector2(direction.x * speed, _rb.velocity.y);
    }
}
