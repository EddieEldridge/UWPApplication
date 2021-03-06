﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Set requiredComponents
[RequireComponent (typeof (Controller2D))]

public class Player : MonoBehaviour
{

    // Variables
    public float jumpHeight =4;
    public float timeToJumpApex =.4f;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallslideSpeedMax = 5;
    public float wallStickTime = .6f;
    float timeToWallUnstick;

    public float moveSpeed = 15;
    private bool lockSpeed = false;
    public int speedBoostAvailable = 3;

    float accelerationTimeAirborne =.2f;
    float accelerationTimeGrounded =.1f;

    public float gravity;
    float jumpVelocity;
    float velocityXSmoothing;
    public Vector3 velocity;


    public Text speedBoostAvailableText;


    // 2D controller
    Controller2D controller;

    // Use this for initialization
    void Start ()
    {
        // Setup our controller
        controller = GetComponent<Controller2D>();

        // Calculations for our gravity
        gravity = -(2 * jumpHeight) /Mathf.Pow (timeToJumpApex, 2);

        // Calculations for our jumpVelocity
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

    }

    void Update()
    {
        if(speedBoostAvailableText!=null)
        {
            // Display number of available speed boosts
            speedBoostAvailableText.text = "BOOSTS: " + speedBoostAvailable.ToString();
        }

        // Setup horizontal unit collision
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        int wallDirX = (controller.collisions.left) ? -1 : 1;

        // Smooth out animations
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        // Wall sliding
        bool wallSliding = false;

        // Depending on what's around the player do the following
        if(controller.collisions.left || controller.collisions.right && !controller.collisions.below && velocity.y <0)
        {
            wallSliding = true;

            // Set our downward velocity when on a wall
            if (velocity.y < -wallslideSpeedMax)
            {
                velocity.y = -wallslideSpeedMax;
            }

            // Smoothen up wall jumping from left to right and vice versa
            if(timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if(input.x != wallDirX && input.x !=0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }

                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }

            // In the case of timeToWallUnstick being less than 0
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }

        // Prevent gravity from accumulating if the player is resting on a surface 
        if(controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

       
        // Jumping!
        // If the player presses space, and there is a collision occuring below them (i.e they are standing on something)
        if(Input.GetKeyDown(KeyCode.Space))
        {

            // Wall jumping
            if(wallSliding)
            {
                if (wallDirX == input.x)
                {
                    // Move away from wall
                    velocity.x = -wallDirX * wallJumpClimb.x;

                    velocity.y = wallJumpClimb.y;
                }

                else if (input.x == 0)
                {
                    velocity.x = -wallDirX * wallJumpOff.x;

                    velocity.y = wallJumpOff.y;
                }

                else
                {
                    velocity.x = -wallDirX * wallLeap.x;

                    velocity.y = wallLeap.y;
                }
            }

            // Normal jump
            if(controller.collisions.below)
            {
                velocity.y = jumpVelocity;
            }
           
        }

        // If the player presses Q give them a speedboost for a short time
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (lockSpeed == false)
            {
                if(speedBoostAvailable>0)
                {
                    // Double our movespeed
                    moveSpeed = moveSpeed * 2;

                    // Remove a speedBoost
                    speedBoostAvailable -= 1;

                    // Lock our speed so our player can't spam the speedboost
                    lockSpeed = true;
                }
               

                else
                {
                    print("No boosts left");
                }
            }

            else
            {
                print("Boost is on cooldown.");
            }
            // Reset movespeed buff after 3 seconds
            Invoke("ResetSpeed", 3.0f);

        }

        // Set the velocity for our player
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

    }

    // Reset speed
    void ResetSpeed()
    {
        lockSpeed = false;
        moveSpeed = 10;
    }

}
