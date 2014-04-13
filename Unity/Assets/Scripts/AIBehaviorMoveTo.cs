using UnityEngine;
using System.Collections;

public class AIBehaviorMoveTo : AIBehavior 
{
	[SerializeField]
	private float minDistance = 10.0f;

	public AIBehaviorMoveTo(AIController c) : base(c)
	{
		priority = 100.0f;
	}
	
	// move has a basic value of 100.0f 
	//
	public override float evaluatePriority ()
	{
		if (controller.owner.hasWaypoints())
		{
			return priority;
		}

		return 0.0f;
	}

	public override void execute ()
	{
		base.execute ();

		if (!controller.owner.hasWaypoints()) return;

		Vector3 tgtLoc = controller.owner.getNextWaypoint();

		// check if we are close to the targetLocation
		// HACK finding new location is temporary here
		//
		if (Vector3.Distance(controller.owner.t.position, tgtLoc) < minDistance)
		{
			controller.owner.removeFirstWaypoint();
		}
		else
		{
			// check if we need to turn left or right
			// 
			// the vector that we want to measure an angle from
			Vector3 referenceForward = controller.owner.t.forward; /* some vector that is not Vector3.up */
			
			// the vector perpendicular to referenceForward (90 degrees clockwise)
			// (used to determine if angle is positive or negative)
			Vector3 referenceRight= Vector3.Cross(Vector3.up, referenceForward);
			
			// the vector of interest
			//

			Vector3 newDirection = new Vector3(tgtLoc.x - controller.owner.t.position.x, tgtLoc.y - controller.owner.t.position.y, tgtLoc.z - controller.owner.t.position.z);
			
			// Get the angle in degrees between 0 and 180
			float angle = Vector3.Angle(newDirection, referenceForward);
			
			// Determine if the degree value should be negative.  Here, a positive value
			// from the dot product means that our vector is on the right of the reference vector   
			// whereas a negative value means we're on the left.
			float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));
			
			controller.bearing = sign * angle;
			
			controller.setTurn(controller.bearing);
			
			Debug.DrawRay (controller.owner.t.position, newDirection, Color.green);
		}
	}
}
