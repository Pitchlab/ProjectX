using UnityEngine;
using System.Collections;

// Locomotor base class
// TODO move flight stuff to FlightLocomotor
// 
public class Locomotor : Object {

	public Actor owner;

	// The max variables are to make the rotation framerate independent.
	// You could alternatively do the work in FixedUpdate,
	// but the controls might be less responsive there.
	
	// speed
	public float maxSpeed 		= 1.0f;
	public float minSpeed  		= 0.1f;
	public float acceleration 	= 0.1f;
	public float speed 			= 0.0f;
	
	// Tilt
	public float maxTilt 		= 180.0f; 	// Degrees / second
	public float tiltScale 		= 60.0f; 	// Degrees / unitInput * second
	public float tiltRange 		= 30.0f; 	// Degrees
	private float rotX 			= 0.0f; 	// Degrees
	
	// Turn
	public float maxTurn 		= 360.0f; 	// Degrees / second
	public float turnScale  	= 120.0f; 	// Degrees / unitInput * second
	public float turnRange 		= 360.0f; 	// Degrees
	private float rotY 			= 0.0f; 	// Degrees
	
	// Climb and descend back 
	public float maxClimb   	= 0.5f; 	// unitInput * second
	public float maxFall    	=-0.2f;
	public float climbRate  	= 0.1f;
	public float climbCurr  	= 0.0f;		// current climb rate
	public float altitude   	= 0.0f;		// the rate applied to the transform
	public float climbScale 	= 100.0f;	// 
	public float ceiling    	= 10.0f;	// units
	public float deck       	= 2.0f;		// units
	public float fallRate   	= 0.5f;		// fall down to deck
	
	// Bank
	public float maxBank 		= 90.0f; 	//Degrees/second
	public float bankScale 		= 120.0f; 	//Degrees/unitInput*second
	public float returnSpeed  	= 40.0f;	//Degrees/second
	public float bankRange  	= 20.0f; 	//Degrees
	private float rotZ 			= 0.0f; 	//Degrees
	
	// Start information
	private Quaternion originalRot = Quaternion.identity;

	// Use this for initialization
	public Locomotor(Actor actor) 
	{
		owner = actor;
	}
	
	// Update the locomotor controls
	//
	public void Locomote () {

		// Turn and bank
		//
		turn(owner.controller.trn);
		
		// Rotate the actor
		owner.t.localRotation = Quaternion.Euler(0,rotY,0)*originalRot;
		
		// Rotate the Orienter - bank and tilt independent of the base frame
		owner.orienter.transform.localRotation = Quaternion.Euler(rotX,0,rotZ)*originalRot;
		
		// Speed and acceleration
		//
		accelerate(owner.controller.acc);
		
		// vertical movement
		// like a boost going up
		//
		if (owner.controller.alt > 0.0f) {
			climb(climbRate);
		} else
		{
			fall();
		}
		
		// Move the ship
		//
		owner.t.Translate(0, altitude, speed);
	}

	// Actions
	//
	// Turning, Banking
	//
	public void turn(float amount) {
		
		// Bank
		//
		if(!Mathf.Approximately(amount, 0.0f)) {
			rotZ = clampAngle(rotZ-clampAngle(amount*bankScale,-maxBank,maxBank)*Time.deltaTime,-bankRange, bankRange);
		}
		else if(rotZ > 0.0f) {
			rotZ = clampAngle(rotZ-Time.deltaTime*returnSpeed,0.0f,bankRange);
		}
		else {
			rotZ = clampAngle(rotZ+Time.deltaTime*returnSpeed,-bankRange,0.0f);
		}
		
		// Turn
		rotY = clampAngle(rotY+clampAngle(amount*turnScale,-maxTurn,maxTurn)*Time.deltaTime,-turnRange,turnRange);
		
		// Tilt
		// TODO fix link to the climb rate in smooth way
		//
		rotX = 0.0f; //ClampAngle(rotX-ClampAngle(climbCurr*tiltScale,-maxTilt,maxTilt)*Time.deltaTime,-tiltRange,tiltRange);
	}
	
	public void accelerate(float acc) {
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
	
	// Boost is given, boost up towards the ceiling
	//
	public void climb(float rate) {
		
		float y = owner.t.position.y;
		
		climbCurr += climbRate; 
		if (climbCurr > maxClimb)
		{
			climbCurr = maxClimb;
		}
		
		float v = climbCurr * climbScale * Time.deltaTime;
		
		// make sure that the plane can't go above the ceiling
		//
		if (y + v > ceiling) {
			v = 0.0f;
			climbCurr = 0.0f;
		}
		
		altitude = v;
	}
	
	// if there is no boost, gradually fall down to the deck
	//
	public void fall() {
		float y = owner.t.position.y;
		
		climbCurr -= fallRate; 
		if (climbCurr < maxFall)
		{
			climbCurr = maxFall;
		}
		
		float v = climbCurr * climbScale * Time.deltaTime;
		
		// make sure that the plane can't go above the ceiling
		//
		if (y + v < deck) {
			v = 0.0f;
			climbCurr = 0.0f;
		}
		
		altitude = v;
	}

	// Modified to work when you get angles outside of -/+720 
	//
	static float clampAngle (float angle, float min, float max) {
		while (angle < -360.0f) angle += 360.0f;
		while (angle > 360.0f) angle -= 360.0f;
		return Mathf.Clamp(angle, min, max);
	}
}
