using UnityEngine;
using System.Collections;

public class HomingMissile : Projectile 
{

	public float maxTurn			= 0.0001f;	// radians
	
	public float bearing;

	[SerializeField]
	private float rotY = 0.0f;

	[SerializeField]
	private ITargetable 		target;

	// Start information
	protected Quaternion originalRot = Quaternion.identity;
	
	// Use this for initialization
	void Start () 
	{
		speed = 100.0f;
		lifeTime = 10.0f;

		// supposedly helps caching
		t = transform;
		
		gameObject.tag = "Projectile";
	}

	protected override void updateProjectile ()
	{
		if (owner != null)
		{
			target = owner.getTarget();
		}

		// turn towards target (if there is one)
		//
		if (target != null)
		{
			// check if we need to turn left or right
			// 
			// the vector that we want to measure an angle from
			Vector3 referenceForward = t.forward; /* some vector that is not Vector3.up */
			
			// the vector perpendicular to referenceForward (90 degrees clockwise)
			// (used to determine if angle is positive or negative)
			Vector3 referenceRight= Vector3.Cross(t.up, referenceForward);
			Vector3 newDirection = new Vector3(target.getPosition().x - t.position.x, target.getPosition().y - t.position.y, target.getPosition().z - t.position.z);
			//newDirection.Normalize ();
			//newDirection *= 5.0f;

			// Get the angle in degrees between 0 and 180
			float angle = Vector3.Angle(newDirection, referenceForward);
			
			// Determine if the degree value should be negative.  Here, a positive value
			// from the dot product means that our vector is on the right of the reference vector   
			// whereas a negative value means we're on the left.
			float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));

			bearing = sign * angle;
			
			Debug.DrawRay (t.position, newDirection, Color.cyan);
			Debug.DrawRay (t.position, referenceForward, Color.red);

			Vector3 targetDir = target.getPosition()-t.position;
			Vector3 nd = Vector3.RotateTowards(t.forward, targetDir, maxTurn * Time.deltaTime * 0.05f, 0.0f);
			Debug.DrawRay(t.position, nd.normalized, Color.cyan);
			t.rotation = Quaternion.LookRotation(nd);

			// Turn
			//rotY = rotY + bearing/20.0f;	// / 100; //Locomotor.clampAngle(rotY + Locomotor.clampAngle(finalAngle*turnScale,-maxTurn,maxTurn)*Time.deltaTime,-turnRange,turnRange);
			//t.localRotation = Quaternion.Euler(0,rotY,0)*originalRot;
		}

		base.updateProjectile ();
	}

	// setters and getters 
	//
	public void setTarget(ITargetable t)
	{
		if (target != null)
		{
			(target as Entity).removeListener("ENTITY.DESTROY", onTargetDied);
		}

		target = t;

		(target as Entity).addListener("ENTITY.DESTROY", onTargetDied);
	}


	// message handler for ENTITY.DESTROY
	//
	private void onTargetDied(Notification n) 
	{
		target = null;
		(n.sender as Entity).removeListener("ENTITY.DESTROY", onTargetDied);

		die();
	}
}
