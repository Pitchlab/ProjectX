using UnityEngine;
using System.Collections;

public class PlayerController : Controller 
{
	// --------------------------------------------------------------------
	// Initialize
	// --------------------------------------------------------------------
	//
	public override void init()
	{
		if (isInitialized) return;
		
		base.init ();
		
		// important stuff here...
		//
		name = "PlayerController";
		
		isInitialized = true;
	}

	// --------------------------------------------------------------------
	// Update the input
	// --------------------------------------------------------------------
	//
	public override void updateControls ()
	{
		base.updateControls ();

		float deltaX = Input.GetAxis("Horizontal");
		float deltaY = Input.GetAxis("Vertical");


		// Turn and bank
		//
		setTurn(deltaX);

		// Speed and acceleration
		//
		setAcc(deltaY);

		// Spacebar fires laser
		//
		if (Input.GetKey("space")) {
			fireWeapons();
		}
	}

	// give any weapons mounted to the 
	// player the command to fire
	protected void fireWeapons()
	{
		owner.fireWeapons();
	}
}
