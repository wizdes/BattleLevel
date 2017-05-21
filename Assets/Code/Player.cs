using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject {

    public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
	public int pointsPerFood = 10;              //Number of points to add to player food points when picking up a food object.
	public int pointsPerSoda = 20;              //Number of points to add to player food points when picking up a soda object.
	public int wallDamage = 1;                  //How much damage a player does to a wall when chopping it.

    private Animator animator;
    private int food;

	// Use this for initialization
	public override void Start () {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        base.Start();
	}

    private void OnDisable(){
        GameManager.instance.playerFoodPoints = food;
    }

    protected override void AttemptMove<T>(int xDir, int yDir){
        food--;
        base.AttemptMove<T>(xDir, yDir);

    }
	
	// Update is called once per frame
	void Update () {
		//If it's not the player's turn, exit the function.
        if (this.isMoving) return;

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
            AttemptMove<Component>(horizontal, vertical);
		}        
	}

    protected override void OnCantMove<T>(T component)
    {
        
    }


}
