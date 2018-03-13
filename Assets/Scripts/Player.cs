﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Set requiredComponents
[RequireComponent (typeof (Controller2D))]

public class Player : MonoBehaviour {

    // Variables
    public float jumpHeight =4;
    public float timeToJumpApex =.4f;
    float moveSpeed = 6;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;

    // 2d controller
    Controller2D controller;

	// Use this for initialization
	void Start () {
        controller = GetComponent<Controller2D>();

        // Calculations for our gravity
        gravity = -(jumpHeight * 2) /Mathf.Pow (timeToJumpApex, 2);

        // Calculations for our jumpVelocity
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + "Jump Velocity: " + jumpVelocity);
    }

    void Update()
    {
        // Prevent gravity from accumulating if the player is resting on a surface 
        if(controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        // Setup horizontal unit collision
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Jumping!
        // If the player presses space, and there is a collision occuring below them (i.e they are standing on something)
        if(Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        // Every frame, set the x velocity
        velocity.x = input.x * moveSpeed;

        // Set the velocity for our player
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
