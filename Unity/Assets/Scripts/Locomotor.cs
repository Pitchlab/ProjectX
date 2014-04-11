using UnityEngine;
using System.Collections;

// Locomotor base class
// TODO move flight stuff to FlightLocomotor
// 
public class Locomotor : Entity 
{
	// The max variables are to make the rotation framerate independent.
	// You could alternatively do the work in FixedUpdate,
	// but the controls might be less responsive there.

	// Acceleration & Speed
	//
	public float maxSpeed 		= 0.5f;
	public float minSpeed  		= 0.25f;
	public float acceleration 	= 0.2f;
	public float speed 			= 0.0f;
	public float accRate 		= 0.0f;		// current acceleration
	public float accFriction 	= 0.1f;		// reduce acceleration when no boost is pressed

	// Turn
	public float maxTurn 		= 360.0f; 	// Degrees / second
	public float turnScale  	= 120.0f; 	// Degrees / unitInput * second
	public float turnRange 		= 360.0f; 	// Degrees
	
	// Bank
	public float maxBank 		= 90.0f; 	//Degrees/second
	public float bankScale 		= 120.0f; 	//Degrees/unitInput*second
	public float returnSpeed  	= 40.0f;	//Degrees/second
	public float bankRange  	= 20.0f; 	//Degrees

	[SerializeField]
	protected float rotX 		= 0.0f; 	// Degrees
	[SerializeField]
	protected float rotY 		= 0.0f; 	// Degrees
	[SerializeField]
	protected float rotZ 		= 0.0f; 	//Degrees
	
	// Start information
	protected Quaternion originalRot = Quaternion.identity;
	
	// Update the locomotor controls
	//
	public virtual void Locomote () {

		// Turn and bank
		//
		turn(owner.controller.getTurn());
		
		// Rotate the actor
		owner.t.localRotation = Quaternion.Euler(0,rotY,0)*originalRot;

		// Rotate the Orienter - bank and tilt independent of the base frame
		owner.orienter.transform.localRotation = Quaternion.Euler(rotX,0,rotZ)*originalRot;
		
		// Speed and acceleration
		//
		accelerate(owner.controller.getAcc());

		// Move the ship
		//
		owner.t.Translate(0, 0, speed);
	}

	// Actions
	//
	// Turning, Banking
	//
	public virtual void turn(float bearing) {
		
		// Bank
		//
		if(!Mathf.Approximately(bearing, 0.0f)) {
			rotZ = clampAngle(rotZ-clampAngle(bearing*bankScale,-maxBank,maxBank)*Time.deltaTime,-bankRange, bankRange);
		}
		else if(rotZ > 0.0f) {
			rotZ = clampAngle(rotZ-Time.deltaTime*returnSpeed,0.0f,bankRange);
		}
		else {
			rotZ = clampAngle(rotZ+Time.deltaTime*returnSpeed,-bankRange,0.0f);
		}

		// Turn
		// HACK clampAngle seems to have artifacts? use bearing directly
		//
		rotY = clampAngle(rotY+clampAngle(bearing*turnScale,-maxTurn,maxTurn)*Time.deltaTime,-turnRange,turnRange);
		
		// Tilt
		// TODO fix link to the climb rate in smooth way
		//
		rotX = 0.0f; //ClampAngle(rotX-ClampAngle(climbCurr*tiltScale,-maxTilt,maxTilt)*Time.deltaTime,-tiltRange,tiltRange);
	}
	
	public virtual void accelerate(float acc) 
	{
		speed += acc;
		
		if (speed > maxSpeed)
		{
			speed = maxSpeed;
		}
		else if (speed < minSpeed)
		{
			speed = minSpeed;
		}
	}

	// Modified to work when you get angles outside of -/+720 
	//
	public static float clampAngle (float angle, float min, float max) 
	{
		while (angle < -360.0f) angle += 360.0f;
		while (angle > 360.0f) angle -= 360.0f;
		return Mathf.Clamp(angle, min, max);
	}
}
