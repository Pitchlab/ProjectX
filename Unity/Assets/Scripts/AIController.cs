using UnityEngine;
using System.Collections.Generic;

// TODO
//
// Create Orders in the system to follow
// The AIController can issue orders 
// How they are followed is a matter of the controller
// ?- Do I need to split this up? 
// ?- Move order execution to the regular controller so it is inherited? 
//
public class AIController : Controller {
	
	public float minDistance = 10.0f;

	[SerializeField] 
	protected float bearing;
	
	// update the controls and update trn, acc, alt.
	// 
	public override void updateControls() 
	{

		float d = Vector3.Distance(owner.t.position, owner.targetMarker.transform.position);

		// check if we are close to the targetLocation
		if (d < minDistance)
		{
			//Target t = Target.getRandomTargetLocation();
			//owner.targetList.Add(t);			
			owner.targetMarker.transform.position =  Target.getRandomTargetLocation().getTargetLocation();
		}
		else 
		{
			// check if we need to turn left or right
			// 
			// the vector that we want to measure an angle from
			Vector3 referenceForward = owner.t.forward; /* some vector that is not Vector3.up */
				
			// the vector perpendicular to referenceForward (90 degrees clockwise)
			// (used to determine if angle is positive or negative)
			Vector3 referenceRight= Vector3.Cross(Vector3.up, referenceForward);
			
			// the vector of interest

			//Vector3 newDirection = new Vector3(owner.targetMarker.transform.position.x - owner.t.position.x, owner.targetMarker.transform.position.y - owner.t.position.y, owner.targetMarker.transform.position.z - owner.t.position.z);

			// find first Actor in list
			// HACK testing 
			// TODO create a 'selected target' - the one currently targeted. This can be location or Actor.
			// The TargetList will only contain actors and projectiles. There we can remove the need for Locations totally.
			//
			foreach (Actor a in owner.targetList)
			{
				//if (tgt.isAlive())
				//{
					Vector3 tgtLoc = a.t.position;
					Vector3 newDirection = new Vector3(tgtLoc.x - owner.t.position.x, tgtLoc.y - owner.t.position.y, tgtLoc.z - owner.t.position.z);

					// Get the angle in degrees between 0 and 180
					float angle = Vector3.Angle(newDirection, referenceForward);
					
					// Determine if the degree value should be negative.  Here, a positive value
					// from the dot product means that our vector is on the right of the reference vector   
					// whereas a negative value means we're on the left.
					float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));
					
					bearing = sign * angle;

					setTurn(bearing);

					Debug.DrawRay (owner.t.position, newDirection, Color.yellow);

					if (Vector3.Distance (a.t.position, owner.t.position) < 50.0f)
					{
						// HACK
						//
					    owner.fireWeapons();
					}

					return;
				//}
			}
		}


	}
}
