﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7.5f;
    [SerializeField] private float jumpForce = 50f;
    [SerializeField] private float groundDist = 100f;

    public bool isRobot;
    public bool Grounded;
    public Transform groundCheck;
    public Animator Animation;

    private bool hasJumped = false; 
    private Rigidbody2D rb;
    private float moveInput;
    



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }


    private void Update()
    {
        Move();
        Jump();
        Grounded = Physics2D.Raycast(transform.position, groundCheck.position, LayerMask.GetMask("Ground"));
        //print(hasJumped);
    }


    private void Move()
    {
        if ((!isRobot && !GameManager.instance.controllingRobot))
        {
            moveInput = UserInput.instance.moveInput.x;

            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            Animation.SetFloat("Moving", Mathf.Abs(moveInput));

            if (moveInput > 0.1f)
            {
                // Face right
                Animation.transform.localScale = new Vector3(Mathf.Abs(Animation.transform.localScale.x), Animation.transform.localScale.y, Animation.transform.localScale.z);
            }
            else if (moveInput < -0.1f)
            {
                // Face left
                Animation.transform.localScale = new Vector3(-Mathf.Abs(Animation.transform.localScale.x), Animation.transform.localScale.y, Animation.transform.localScale.z);
            }
        }
        if ((isRobot && GameManager.instance.controllingRobot && GameManager.instance.Robot == gameObject))
        {
            moveInput = UserInput.instance.moveInput.x;

            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            Animation.SetFloat("Moving", Mathf.Abs(moveInput));

            // Same flipping logic for robot if needed
        }
    }

    private void Jump()
    {
        if (Grounded && UserInput.instance.controls.Movement.Jump.triggered && !hasJumped)
        {
            if ((!isRobot && !GameManager.instance.controllingRobot) || (isRobot && GameManager.instance.controllingRobot))
            {
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                hasJumped = true;
                //Animation.SetTrigger("takeoff");
                Animation.SetBool("up", true);
            }
            Animation.SetTrigger("takeoff");
            Animation.SetBool("isjumping", true);
        }

    }   

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && hasJumped)
        {
            hasJumped = false;
            Animation.SetBool("isjumping", false);

        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDist);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") )
        {
            collision.transform.parent = this.transform;
        }
        if ((!isRobot && !GameManager.instance.controllingRobot) || (isRobot && GameManager.instance.controllingRobot))
        {
            //collision.transform.parent = this.transform;
        }
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }

}
