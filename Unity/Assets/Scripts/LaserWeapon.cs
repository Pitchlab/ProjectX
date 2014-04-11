using UnityEngine;
using System.Collections;

public class LaserWeapon : Weapon 
{
	
	public override void init ()
	{
		if (isInitialized) return;

		base.init ();

		coolDown = 3.0f;

		name = "LaserWeapon";

		addEmitter(new Vector3(-1.5f, -0.4f, 2.5f), Quaternion.identity, "left",  "HomingMissileFab");
		addEmitter(new Vector3( 1.5f, -0.4f, 2.5f), Quaternion.identity, "right", "HomingMissileFab");

		isInitialized = true;
	}
}
