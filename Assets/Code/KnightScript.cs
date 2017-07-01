using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightScript : MovingObject
{
    private bool isSelected;
    private Animator animator;
    private SpriteRenderer sprite;
    private int range;
    public GameObject squareUnit;

    public GameObject[] arrowUnits;

    private Vector3 position;
    private List<GameObject> createdSquares;
    private List<GameObject> createdArrows;

    //0 not selected
    // 1 selected, no movement
    // 2 movement selected, confirm not selected

    private int selectState;
    private Tuple<int, int> selectedMovement;

    public override void Start()
    {
        range = 4;
        selectState = 0;
        selectedMovement = new Tuple<int, int>(-99,  -99);
        position = new Vector3(-1, -1);
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        createdArrows = new List<GameObject> ();
        createdSquares = new List<GameObject>();

		base.Start();
    }

    private void DrawMovementRange()
    {
        for (int i = 0; i < range; i++)
        {
            for (int j = 0; j < range; j++)
            {
                // outside the range; no need to be considered
                if ((i + j > range - 1) || (i == 0 && j == range) || (j == 0 && i == range))
                {
                    continue;
                }

                // boundary check
                HashSet<int> markedSquare = new HashSet<int>();

                foreach (var xMulti in new int[] { -1, 1 })
                {
                    foreach (var yMulti in new int[] { -1, 1 })
                    {
                        int markedSquareValue = (int)(transform.position.x + xMulti * i + 100 * (transform.position.y + yMulti * j));
                        if (transform.position.x + xMulti * i >= 0 && transform.position.x + xMulti * i < 16
                            && transform.position.y + yMulti * j >= 0 && transform.position.y + yMulti * j < 9
                            && !markedSquare.Contains(markedSquareValue))
                        {
							bool skip = false;
							//skip if it's a collision
							foreach (string collisionMarker in BoardManager.collisionCoordinateMap) {
								int xCollideVal = int.Parse (collisionMarker.Split (',') [0]);
								int yCollideVal = int.Parse (collisionMarker.Split (',') [1]);

								if (xCollideVal == (int) transform.position.x + xMulti * i && 9 - yCollideVal -1 == (int) transform.position.y + yMulti * j) {
									skip = true;
								}
							}

							if (skip)
								continue;

                            GameObject SquareInstance = Instantiate(
                                squareUnit,
                                new Vector3(transform.position.x + xMulti * i, transform.position.y + yMulti * j, 1f), Quaternion.identity) as GameObject;
                            SquareInstance.transform.SetParent(transform);
                            createdSquares.Add(SquareInstance);
                            markedSquare.Add(markedSquareValue);
                        }
                    }
                }
            }
        }
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

        HandleTouchMouseInput();

        if (!isSelected && Util.IsTouchEquivalent(position.x, transform.position.x)
            && Util.IsTouchEquivalent(position.y, transform.position.y))
        {
            isSelected = true;
            selectState = 1;
            animator.speed = 2.0f;
            DrawMovementRange();
        }

        if (isSelected && !(Util.IsTouchEquivalent(position.x, transform.position.x)
                            && Util.IsTouchEquivalent(position.y, transform.position.y)))
        {
            int xDiff = CalculatePositionDifference(position.x, transform.position.x);
            int yDiff = CalculatePositionDifference(position.y, transform.position.y);

            bool isSame = (xDiff == selectedMovement.First && yDiff == selectedMovement.Second);
			selectedMovement = new Tuple<int, int>(xDiff, yDiff);

			List<Tuple<int, int>> movements = new List<Tuple<int, int>>();

            while (xDiff != 0 || yDiff != 0)
            {
                xDiff = AddMovement(xDiff, movements, true);
                yDiff = AddMovement(yDiff, movements, false);
            }

            if(selectState == 1 || (selectState == 2 && !isSame)){

                foreach(var createdArrow in createdArrows){
                    Destroy(createdArrow);
                }

				EncodeArrowMovements(movements);
                selectState = 2;

                // this is a hack! this is only set so it doesn't trigger until the click is 'ended'
                position.x = transform.position.x;
                position.y = transform.position.y;
                return;
			}
            else if(selectState == 2){
                Move(movements);
                selectState = 0;
				isSelected = false;
				position = new Vector3(-1, -1);
				animator.speed = 3.0f;

				foreach (var createdArrow in createdArrows)
				{
					Destroy(createdArrow);
				}

                foreach (var createdSquare in createdSquares)
				{
					Destroy(createdSquare);
				}
			}
        }
    }

    private int CalculatePositionDifference(float toucchPositionDim, float objectPositionDim)
    {
        float xcalcPosition = objectPositionDim;
        if (toucchPositionDim > objectPositionDim) xcalcPosition -= 0.5f;
        else xcalcPosition += 0.5f;

        return (int)(toucchPositionDim - xcalcPosition);
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
        else if(increment < 0)
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

    private void EncodeArrowMovements(List<Tuple<int, int>> movementPoints){
        int initX = (int)transform.position.x;
        int initY = (int)transform.position.y;
        string type = "";

        for (int i = 0; i < movementPoints.Count - 1; i++){
            initX += movementPoints[i].First;
            initY += movementPoints[i].Second;
            type = transformMovements(movementPoints[i].First * -1, movementPoints[i].Second * -1, movementPoints[i+1].First * -1, movementPoints[i+1].Second * -1);
            AddArrows(type, initX, initY);
        }

        Tuple<int, int> lastMovement = movementPoints[movementPoints.Count - 1];
        type = transformMovements(lastMovement.First * -1, lastMovement.Second * -1, 9,9);

        initX += lastMovement.First;
        initY += lastMovement.Second;
        AddArrows(type, initX, initY);
	}

    private string transformMovements(int x1, int y1, int x2, int y2){
        string key = x1.ToString() + y1.ToString() + x2.ToString() + y2.ToString();

        switch(key){
            case "-10-10":
                return "A";
			case "-100-1":
				return "I";
			case "0-10-1":
				return "D";
			case "0-1-10":
				return "N";
			case "-1001":
				return "O";
			case "0101":
				return "C";
			case "01-10":
				return "J";
			case "0110":
				return "Z";
			case "1010":
				return "B";
			case "1001":
				return "K";
			case "0-110":
				return "L";
			case "100-1":
				return "M";
			case "-1099":
				return "E";
			case "1099":
				return "H";
			case "0199":
				return "P";
			case "0-199":
				return "R";
			default:
                return null;
        }
    }

    private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase)
    {
        switch (touchPhase)
        {
            case TouchPhase.Began:
				position.x = touchPosition.x;
                position.y = touchPosition.y;
                break;
            case TouchPhase.Moved:
                break;
            case TouchPhase.Ended:
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

    private void AddArrows(string key, int xPosition, int yPosition){

        GameObject arrowObjectToInstantiate = null;

        // 0 - 0, 1 - 90, 2 - 180, 3 - 270
        int rotation = 0;
        bool flipX = false;
        bool flipY = false;
        switch(key){
            case "A":
                arrowObjectToInstantiate = arrowUnits[0];
                break;
			case "B":
                rotation = 2;
                arrowObjectToInstantiate = arrowUnits[0];
				break;
			case "C":
                rotation = 1;
				arrowObjectToInstantiate = arrowUnits[0];
				break;
			case "D":
                rotation = 3;
				arrowObjectToInstantiate = arrowUnits[0];
				break;
			case "I":
                arrowObjectToInstantiate = arrowUnits[2];
				break;
			case "J":
                rotation = 1;
				arrowObjectToInstantiate = arrowUnits[2];
				break;
			case "K":
                rotation = 2;
				arrowObjectToInstantiate = arrowUnits[2];
				break;
			case "L":
                rotation = 3;
				arrowObjectToInstantiate = arrowUnits[2];
				break;
			case "Z":
                rotation = 1;
				arrowObjectToInstantiate = arrowUnits[3];
				break;
			case "M":
                rotation = 2;
				arrowObjectToInstantiate = arrowUnits[3];
				break;
			case "N":
                rotation = 3;
				arrowObjectToInstantiate = arrowUnits[3];
				break;
			case "O":
				arrowObjectToInstantiate = arrowUnits[3];
				break;
			case "E":
				rotation = 0;
				arrowObjectToInstantiate = arrowUnits[1];
				break;
			case "H":
				rotation = 2;
				arrowObjectToInstantiate = arrowUnits[1];
				break;
			case "P":
				rotation = 1;
				arrowObjectToInstantiate = arrowUnits[1];
				break;
			case "R":
				rotation = 3;
				arrowObjectToInstantiate = arrowUnits[1];
				break;
			default:
                break;
        }

        Quaternion rotationQuaternion = Quaternion.identity;

        switch(rotation){
            case 1:
                rotationQuaternion = Quaternion.FromToRotation(Vector3.up, Vector3.right);
                break;
			case 2:
				//rotationQuaternion = Quaternion.FromToRotation(Vector3.up, Vector3.down);
                flipX = true;
                flipY = true;
                break;
			case 3:
				rotationQuaternion = Quaternion.FromToRotation(Vector3.up, Vector3.left);
				break;
			default:
                break;
                
        }

		GameObject arrowInstance = Instantiate(
			arrowObjectToInstantiate,
            new Vector3(xPosition, yPosition, 0f), rotationQuaternion) as GameObject;
        arrowInstance.GetComponent<SpriteRenderer>().flipX = flipX;
		arrowInstance.GetComponent<SpriteRenderer>().flipY = flipY;

		arrowInstance.transform.SetParent(transform);
        createdArrows.Add(arrowInstance);

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
