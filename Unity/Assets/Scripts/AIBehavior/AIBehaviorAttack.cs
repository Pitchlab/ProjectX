using UnityEngine;
using System.Collections;

public class AIBehaviorAttack : AIBehavior {

	public AIBehaviorAttack(AIController c) : base(c)
	{
		priority = 500.0f;
	}

	// if there is an enemy, return high number, otherwise zero.
	//
	public override float evaluatePriority ()
	{
		foreach (Actor a in controller.owner.targetList)
		{
			if ((a != null) && (a.getFaction() != controller.owner.getFaction()))
			{
				return priority;
			}
		}

		return 0.0f;
	}

	public override void execute ()
	{
		base.execute ();

		// fly towards the target
		// check if we need to turn left or right
		// the vector that we want to measure an angle from
		//
		Vector3 referenceForward = controller.owner.t.forward; /* some vector that is not Vector3.up */
		
		// the vector perpendicular to referenceForward (90 degrees clockwise)
		// (used to determine if angle is positive or negative)
		//
		Vector3 referenceRight= Vector3.Cross(Vector3.up, referenceForward);
		
		// the vector of interest
		
		// find first Actor in list
		// HACK testing 
		// TODO create a 'selected target' - the one currently targeted.
		//
		foreach (Actor a in controller.owner.targetList)
		{
			if ((a != null) && (a.getFaction() != controller.owner.getFaction()))
			{
				Vector3 tgtLoc = a.t.position;
				Vector3 newDirection = new Vector3(tgtLoc.x - controller.owner.t.position.x, tgtLoc.y - controller.owner.t.position.y, tgtLoc.z - controller.owner.t.position.z);
				
				// Get the angle in degrees between 0 and 180
				float angle = Vector3.Angle(newDirection, referenceForward);
				
				// Determine if the degree value should be negative.  Here, a positive value
				// from the dot product means that our vector is on the right of the reference vector   
				// whereas a negative value means we're on the left.
				float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));
				
				controller.bearing = sign * angle;
				
				controller.setTurn(controller.bearing);
				
				Debug.DrawRay (controller.owner.t.position, newDirection, Color.yellow);
				
				if (Vector3.Distance (a.t.position, controller.owner.t.position) < 50.0f)
				{
					// HACK fire as often as possible
					//
					controller.owner.fireWeapons();
				}
				
				return;
			}
		}
	}
}
