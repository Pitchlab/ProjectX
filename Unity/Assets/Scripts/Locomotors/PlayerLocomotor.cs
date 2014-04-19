using UnityEngine;
using System.Collections;

public class PlayerLocomotor : Locomotor 
{
	// --------------------------------------------------------------------
	// Initialization
	// --------------------------------------------------------------------
	//
	public override void init()
	{
		if (isInitialized) return;

		base.init ();
		
		maxSpeed = 4.0f;
		minSpeed = 1.0f;

		maxBank 		= 400.0f;
		bankScale 		= 200.0f;
		bankRange 		= 70.0f;
		returnSpeed  	= 200.0f;

		
		isInitialized = true;
	}

	// --------------------------------------------------------------------
	// Actions
	// --------------------------------------------------------------------
	// Acceleration
	//
	public override void accelerate(float acc)
	{
		// adjust speed boost
		// 
		if (acc > 0.0f)
		{
			accRate = acceleration;
		}
		else 
		{
			accRate -= 2 * accFriction;
			if (accRate < -acceleration)
			{
				accRate = -acceleration;
			}
		}
		
		// compute new speed
		
		speed += accRate;
		
		if (speed > maxSpeed)
		{
			speed = maxSpeed;
		}
		else if (speed < minSpeed)
		{
			speed = minSpeed;
		}

		// HACK - linked the (fixed named!) wings to accelerator
		//
		GameObject w = GameObject.Find("Ship_Wings_Left Body Left");
		w.transform.localRotation = Quaternion.Euler(0,-acc*15, 0);
		
		w = GameObject.Find("Ship_Left_Under Wings_Left Body");
		w.transform.localRotation = Quaternion.Euler(0,0, -acc*15);
		
		w = GameObject.Find("Ship_Wings_Left Body Left_Top");
		w.transform.localRotation = Quaternion.Euler(0,0, acc*15);
		
		// right
		//
		w = GameObject.Find("Ship_Wings_Left Body Left1");
		w.transform.localRotation = Quaternion.Euler(0,acc*15, 0);
		
		w = GameObject.Find("Ship_Body Wings_Right Right_Top");
		w.transform.localRotation = Quaternion.Euler(0,0, -acc*15);
		
		w = GameObject.Find("Ship_Body Right_Under Wings_Right");
		w.transform.localRotation = Quaternion.Euler(0,0, acc*15);
	}
}
