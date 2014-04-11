using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private Transform 	myTransform;

	// simple gun mount positions
	// assumed to hold BulletEmitters
	// 
	public ProjectileEmitter WpnLeft;
	public ProjectileEmitter WpnRight;

	// this is a locator that we use to have an independent frame 
	// for rotation (bank, tilt) 
	//
	public GameObject Orienter;	

	public bool keyboardControls = true;
	public bool mouseControls = false;


	public int score = 0;

	// The max variables are to make the rotation framerate independent.
	// You could alternatively do the work in FixedUpdate,
	// but the controls might be less responsive there.

	// speed
	public float maxSpeed = 1.0f;
	public float minSpeed  = 0.1f;
	public float speed = 0.0f;

	// speed boost
	public float acceleration = 1.0f;	// add to acceleration when boost pressed
	public float accRate = 0.0f;		// current acceleration
	public float accFriction = 0.1f;	// reduce acceleration when no boost is pressed

	// Tilt
	public float maxTilt = 180.0f; 		// Degrees / second
	public float tiltScale = 60.0f; 	// Degrees / unitInput * second
	public float tiltRange = 30.0f; 	// Degrees
	private float rotX = 0.0f; 			// Degrees
	
	// Turn
	public float maxTurn = 360.0f; 		// Degrees / second
	public float turnScale  = 120.0f; 	// Degrees / unitInput * second
	public float turnRange = 360.0f; 	// Degrees
	private float rotY = 0.0f; 			// Degrees

	// Climb and descend back 
	public float maxClimb   = 0.5f; 	// unitInput * second
	public float maxFall    =-0.2f;
	public float climbRate  = 0.1f;
	public float climbCurr  = 0.0f;		// current climb rate
	public float altitude   = 0.0f;		// the rate applied to the transform
	public float climbScale = 100.0f;	// 
	public float ceiling    = 10.0f;	// units
	public float deck       = 2.0f;		// units
	public float fallRate   = 0.5f;		// fall down to deck
	
	// Bank
	public float maxBank = 90.0f; //Degrees/second
	public float bankScale = 120.0f; //Degrees/unitInput*second
	public float returnSpeed  = 40.0f;//Degrees/second
	public float bankRange  = 20.0f; //Degrees
	private float rotZ = 0.0f; //Degrees
	
	// Input
	private float mouseScale = 1.0f; //Gs of acceleration/pixel
	private float deltaX = 0.0f; //Units of input
	private float deltaY = 0.0f; //Units of input
	
	// Start information
	private Quaternion originalRot = Quaternion.identity;

	// Use this for initialization
	void Start () {

		// supposedly helps caching
		myTransform = transform;

		// Spawn point - position = -3,-3,-1
		myTransform.position = new Vector3(0,0,0);

		WpnLeft.owner  = this;
		WpnRight.owner = this;

		Debug.Log ("Init Player");
	}
	
	// Update is called once per frame
	void Update () {



		deltaX = Input.GetAxis("Horizontal");
		deltaY = Input.GetAxis("Vertical");


		// Turn and bank
		//
		Turn(deltaX);

		// Rotate the ship
		myTransform.localRotation = Quaternion.Euler(0,rotY,0)*originalRot;

		// Rotate the Orienter
		Orienter.transform.localRotation = Quaternion.Euler(rotX,0,rotZ)*originalRot;

		// Speed and acceleration
		//
		Accelerate(deltaY);

		// vertical movement
		// like a boost going up
		//
		if (Input.GetKey(KeyCode.LeftShift)) {
			Climb(climbRate);
		} else
		{
			Fall();
		}

		// Move the ship
		// TODO: split up in the game object that only turns in y, the mesh then tilts and banks separately
		//
		
		myTransform.Translate(0, altitude, speed);
		// Spacebar fires laser
		//
		if (Input.GetKey("space")) {
			FireWeapons();
		}


	}

	// Actions
	//
	// Turning, Banking
	//
	public void Turn(float amount) {

		// Bank
		//
		if(!Mathf.Approximately(amount, 0.0f))
			rotZ = ClampAngle(rotZ-ClampAngle(amount*bankScale,-maxBank,maxBank)*Time.deltaTime,-bankRange, bankRange);
		else if(rotZ > 0.0f)
			rotZ = ClampAngle(rotZ-Time.deltaTime*returnSpeed,0.0f,bankRange);
		else rotZ = ClampAngle(rotZ+Time.deltaTime*returnSpeed,-bankRange,0.0f);
		
		// Turn
		rotY = ClampAngle(rotY+ClampAngle(amount*turnScale,-maxTurn,maxTurn)*Time.deltaTime,-turnRange,turnRange);
		

	}

	public void Accelerate(float acc) {

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

		// Tilt
		// TODO fix link to the climb rate in smooth way
		//
		//rotX = ClampAngle(rotX-ClampAngle(-1*accRate*tiltScale,-maxTilt/2.0f,maxTilt)*Time.deltaTime,-tiltRange,tiltRange);
		// HACK - linked the (fixed named!) wings to accelerator

		GameObject w = GameObject.Find("wleft");
		w.transform.localRotation = Quaternion.Euler(0,-acc*15, 0);

		w = GameObject.Find("uleft");
		w.transform.localRotation = Quaternion.Euler(0,0, -acc*15);
		
		w = GameObject.Find("tleft");
		w.transform.localRotation = Quaternion.Euler(0,0, acc*15);

		// right
		//
		w = GameObject.Find("wright");
		w.transform.localRotation = Quaternion.Euler(0,acc*15, 0);

		w = GameObject.Find("uright");
		w.transform.localRotation = Quaternion.Euler(0,0, -acc*15);

		w = GameObject.Find("tright");
		w.transform.localRotation = Quaternion.Euler(0,0, acc*15);


	}

	// Boost is given, boost up towards the ceiling
	//
	public void Climb(float rate) {

		float y = myTransform.position.y;

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
	public void Fall() {
		float y = myTransform.position.y;
		
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

	// fire weapon 
	public void FireWeapons() {
		WpnLeft.Fire();
		WpnRight.Fire();
	}


	// Modified to work when you get angles outside of -/+720 
	//
	static float ClampAngle (float angle, float min, float max) {
		while (angle < -360.0f) angle += 360.0f;
		while (angle > 360.0f) angle -= 360.0f;
		return Mathf.Clamp(angle, min, max);
	}

	public void addScore(int points) 
	{
		score += points;
	}

	void OnTriggerEnter(Collider col)
	{
		Debug.Log ("Bang!!!");
	}

	void OnCollision(Collision col)
	{
		Debug.Log ("Banger!!!");
	}
}
