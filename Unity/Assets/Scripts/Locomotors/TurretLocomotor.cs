using UnityEngine;
using System.Collections;

public class TurretLocomotor : Locomotor 
{

	// no moving... do nothing! 
	//
	public override void move ()
	{
	}

	// turn the guns and the orienter 
	//
	public override void turn (float bearing)
	{
		// Turn
		// HACK clampAngle seems to have artifacts? use bearing directly
		//
		rotY = clampAngle(rotY+clampAngle(bearing*turnScale,-maxTurn,maxTurn)*Time.deltaTime,-turnRange,turnRange);

		// Bank (Aim)
		// TODO rotate the orienter to aim vertically!
		//
		(owner as Turret).orienter.transform.localRotation = Quaternion.Euler(0,0,0.5f)*originalRot;

		// Rotate the orienter to aim the gun
		//
		owner.transform.localRotation = Quaternion.Euler(0,rotY,0)*originalRot;
	}
}
