using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightScript : MovingObject
{
    public GameObject mainObject;
    private Animator animator;
    private SpriteRenderer sprite;
    private bool starto;

    public override void Start()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (starto == false){
            List<Tuple<int, int>> movements = new List<Tuple<int, int>>();
            Tuple<int, int> x = new Tuple<int, int>(0, 1);
            movements.Add(x);
			x = new Tuple<int, int>(1, 0);
			movements.Add(x);
            Move(movements);
            starto = true;
		}

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

            Move(horizontal, vertical);
        }
    }

	//Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
    protected override IEnumerator SmoothMultipleMovement(List<Tuple<int, int>> movementPoints)
	{
		foreach (Tuple<int, int> movementPoint in movementPoints)
		{
			int xDir = movementPoint.First;
			int yDir = movementPoint.Second;

			//Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
			//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
			if (xDir != 0)
			{
				animator.SetBool("Idle", false);
				animator.SetBool("WalkBack", false);
				animator.SetBool("WalkSideways", true);

				if (xDir < 0)
				{
					sprite.flipX = true;
				}
				else
				{
					sprite.flipX = false;
				}
			}
			else if (yDir != 0)
			{
				if (yDir > 0)
				{
					animator.SetBool("Idle", false);
					animator.SetBool("WalkSideways", false);
					animator.SetBool("WalkBack", true);
				}
			}

			animator.speed = 3.0f;
			Vector3 moveAmount = new Vector3(xDir, yDir, 0f);
			Vector3 end = transform.position + moveAmount;

			//Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
			//Square magnitude is used instead of magnitude because it's computationally cheaper.
			float sqrRemainingDistance = end.sqrMagnitude;

			//While that distance is greater than a very small amount (Epsilon, almost zero):
			while (sqrRemainingDistance > float.Epsilon)
			{
				Vector3 endPoint = Vector3.MoveTowards(transform.position, end, 0.05f);

				//Call MovePosition on attached Rigidbody2D and move it to the calculated position.
				transform.Translate(endPoint - transform.position);

				//Recalculate the remaining distance after moving.
				sqrRemainingDistance = (new Vector3(transform.position.x, transform.position.y) - end).sqrMagnitude;

				//Return and loop until sqrRemainingDistance is close enough to zero to end the function
				yield return null;
			}
		}

		this.isMoving = false;
	}
}
