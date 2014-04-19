using UnityEngine;
using System.Collections;

// --------------------------------------------------------------------
// Controller
// --------------------------------------------------------------------
// handles the input of speed, turn, acceleration and firing for an actor
//
public class Controller : Entity {

	[SerializeField]
	protected float trn = 0.0f;		// turning movement
	[SerializeField]
	protected float acc = 0.0f;		// acceleration

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
		name = "Controller";
		
		isInitialized = true;
	}

	// --------------------------------------------------------------------
	// Getters and setters
	// --------------------------------------------------------------------
	public float getTurn()
	{
		return trn;
	}

	public void setTurn(float t)
	{
		trn = t;
	}

	public float getAcc()
	{
		return acc;
	}
	
	public void setAcc(float a)
	{
		acc = a;
	}

	// --------------------------------------------------------------------
	// Update the input
	// --------------------------------------------------------------------
	//
	// update the controls and update trn, acc, alt.
	// 
	public virtual void updateControls() {

	}
}
