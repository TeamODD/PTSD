using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxSpeed = 8f;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float moveHorizontal = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            moveHorizontal = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveHorizontal = 1f;
        }

        float currentspeed = Input.GetKey(KeyCode.LeftShift) ? maxSpeed : moveSpeed;

        Vector2 movement = new Vector2(moveHorizontal * currentspeed, rb.linearVelocity.y);
        rb.linearVelocity = movement;
    }

}
