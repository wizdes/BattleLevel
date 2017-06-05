using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightScript : MovingObject
{
    public GameObject mainObject;
    private Animator animator;
    private SpriteRenderer sprite;

    public override void Start()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        //If it's not the player's turn, exit the function.
        if (this.isMoving)
        {
            return;
        }
        else
        {
            animator.SetBool("Idle", true);
            animator.SetBool("WalkSideways", false);
            animator.SetBool("WalkBack", false);
        }

        if (animator.GetBool("Idle") == true)
        {
            sprite.flipX = false;
            animator.speed = 1.0f;
        }

        int horizontal = 0;     //Used to store the horizontal move direction.
        int vertical = 0;       //Used to store the vertical move direction.


        //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));

        //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        //Check if moving horizontally, if so set vertical to zero.
        if (horizontal != 0)
        {
            vertical = 0;
        }

        //Check if we have a non-zero value for horizontal or vertical
        if (horizontal != 0 || vertical != 0)
        {
            //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
            //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
            if (horizontal != 0)
            {
                animator.SetBool("Idle", false);
                animator.SetBool("WalkBack", false);
                animator.SetBool("WalkSideways", true);

                if (horizontal < 0)
                {
                    sprite.flipX = true;
                }
                else
                {
                    sprite.flipX = false;
                }
            }
            else if(vertical != 0)
            {
                if(vertical > 0)
                {
                    animator.SetBool("Idle", false);
                    animator.SetBool("WalkSideways", false);
                    animator.SetBool("WalkBack", true);
                }
            }

            animator.speed = 3.0f;

            AttemptMove<Component>(horizontal, vertical);
        }
    }

}
