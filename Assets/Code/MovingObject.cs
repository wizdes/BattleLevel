using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.01f;

    public bool isMoving;
    private float inverseMoveTime;
    private Rigidbody2D rb2D;

	// Use this for initialization
	public virtual void Start () {
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
        this.isMoving = false;
	}

    protected bool Move(int xDir, int yDir){
        Vector2 start = rb2D.position;
        this.isMoving = true;

        Vector2 end = start + new Vector2(xDir, yDir);

        StartCoroutine(SmoothMovement(xDir, yDir));

        return true;
    }

	//Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
	protected IEnumerator SmoothMovement(int xDir, int yDir)
	{
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

        this.isMoving = false;
	}

    //The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
	//AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
	protected virtual void AttemptMove<T>(int xDir, int yDir)
		where T : Component
	{
		//Set canMove to true if Move was successful, false if failed.
		Move(xDir, yDir);
	}	
}
