using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightScript : MovingObject
{
    public GameObject mainObject;

    private bool isSelected;
    private Animator animator;
    private SpriteRenderer sprite;
    private bool starto;

    private Vector3 position;

    public override void Start()
    {
        position = new Vector3(-1,-1);
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

        if (!isSelected && Util.IsTouchEquivalent(position.x, transform.position.x)
            && Util.IsTouchEquivalent(position.y, transform.position.y))
        {
            isSelected = true;
            animator.speed = 2.0f;
        }

        if (isSelected && !(Util.IsTouchEquivalent(position.x, transform.position.x)
                            && Util.IsTouchEquivalent(position.y, transform.position.y)))
        {
            int xDiff = (int)(position.x - transform.position.x);
            int yDiff = (int)(position.y - transform.position.y);

            List<Tuple<int, int>> movements = new List<Tuple<int, int>>();
            Tuple<int, int> x = new Tuple<int, int>(0, 1);
            movements.Add(x);
            x = new Tuple<int, int>(1, 0);
            movements.Add(x);

            while (xDiff != 0 && yDiff != 0)
            {
                xDiff = AddMovement(xDiff, movements, true);
                yDiff = AddMovement(yDiff, movements, false);
            }


            Move(movements);
            isSelected = false;
            animator.speed = 1.0f;
        }

        HandleTouchMouseInput();
    }

    private static int AddMovement(int increment, List<Tuple<int, int>> movements, bool isX)
    {
        if (increment > 0)
        {
            if(isX){
				movements.Add(new Tuple<int, int>(1, 0));
            }
            else{
                movements.Add(new Tuple<int, int>(0, 1));
            }
            increment--;
        }
        else
        {
			if (isX)
			{
				movements.Add(new Tuple<int, int>(-1, 0));
			}
			else
			{
				movements.Add(new Tuple<int, int>(0, -1));
			}
			increment++;
        }

        return increment;
    }

    private void HandleTouchMouseInput()
    {
        foreach (Touch touch in Input.touches)
        {
            HandleTouch(touch.fingerId, Camera.main.ScreenToWorldPoint(touch.position), touch.phase);
        }

        // Simulate touch events from mouse events
        if (Input.touchCount == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Began);
            }
            if (Input.GetMouseButton(0))
            {
                HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Moved);
            }
            if (Input.GetMouseButtonUp(0))
            {
                HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Ended);
            }
        }
    }

    private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase)
    {
        switch (touchPhase)
        {
            case TouchPhase.Began:
				position = touchPosition;
				break;
            case TouchPhase.Moved:
                // TODO
                break;
            case TouchPhase.Ended:
                // TODO
                break;
        }
    }

    private void PerformKeyMovements()
    {
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
            AddAnimation(horizontal, vertical);
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

            AddAnimation(xDir, yDir);

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

    private void AddAnimation(int xDir, int yDir)
    {
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
    }
}
