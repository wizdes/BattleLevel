using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;

    public bool isMoving = false;
    private float inverseMoveTime;
    private Rigidbody2D rb2D;

	// Use this for initialization
	public virtual void Start () {
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
	}

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit){
        Vector2 start = transform.position;
        this.isMoving = true;

        Vector2 end = start + new Vector2(xDir, yDir);

        hit = new RaycastHit2D();

        StartCoroutine(SmoothMovement(end));

        return true;
    }

	//Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
	protected IEnumerator SmoothMovement(Vector3 end)
	{
		//Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
		//Square magnitude is used instead of magnitude because it's computationally cheaper.
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		//While that distance is greater than a very small amount (Epsilon, almost zero):
		while (sqrRemainingDistance > float.Epsilon)
		{
			//Find a new position proportionally closer to the end, based on the moveTime
			Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

			//Call MovePosition on attached Rigidbody2D and move it to the calculated position.
			rb2D.MovePosition(newPostion);

			//Recalculate the remaining distance after moving.
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;

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
		//Hit will store whatever our linecast hits when Move is called.
		RaycastHit2D hit;

		//Set canMove to true if Move was successful, false if failed.
		bool canMove = Move(xDir, yDir, out hit);

		//Check if nothing was hit by linecast
		if (hit.transform == null)
			//If nothing was hit, return and don't execute further code.
			return;

		//Get a component reference to the component of type T attached to the object that was hit
		//T hitComponent = hit.transform.GetComponent<T>();

		//If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
		//if (!canMove && hitComponent != null)

			//Call the OnCantMove function and pass it hitComponent as a parameter.
			//OnCantMove(hitComponent);
	}


	//The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
	//OnCantMove will be overriden by functions in the inheriting classes.
	protected abstract void OnCantMove<T>(T component)
		where T : Component;
	
}
