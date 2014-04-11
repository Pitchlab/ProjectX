using UnityEngine;
using System.Collections;

public class Player : Actor {


		
	// --------------------------------------------------------------------
	// Initialization
	// --------------------------------------------------------------------
	//
	// This builds the structure we need so make sure it is called
	//
	public override void init()
	{
		if (isInitialized) return;

		prefabName = "SpaceShip";
		base.init ();

		// add weapon MountPoints
		//
		//MountPoint m = addMountPoint(new Vector3(), Quaternion.identity, "MountPointWeapon");
		
		// add a weapon
		//
		//LaserWeapon w = m.gameObject.AddComponent<LaserWeapon>();
		//w.init();
		//w.setOwner(this);
		//mount(w, "MountPointWeapon");
		
		// add a weapon
		//
		//WpnLeft.owner  = this;
		//WpnRight.owner = this;

		this.name = "Player";

		isInitialized = true;
	}

	// init controller - override for specific controller
	//
	public override void initController()
	{
		controller = gameObject.AddComponent<PlayerController>();
		controller.setOwner(this);
		controller.init();
	}

	// init locomotor - override for specific locomotor
	//
	public override void initLocomotor()
	{
		locomotor = gameObject.AddComponent<PlayerLocomotor>();
		locomotor.setOwner(this);
		locomotor.init();
	}
	
	// --------------------------------------------------------------------
	// Collision Detection
	// --------------------------------------------------------------------
	//
	// this method can be overridden to do custom
	// collision
	//
	protected override void doCollision(Collision col)
	{
		//Debug.Log ("Collision!" + col.ToString());
		
		// check what hit us
		//
		GameObject go = col.gameObject;
		
		if (col.gameObject.tag != "Projectile") return;
		
		// Projectile? 
		//
		Projectile p = go.GetComponent<Projectile>();
		if (p != null)
		{
			p.owner.addScore(10);
			lastHit = p.owner;
		}
		
		// attribute points
		// 
		damage (10.0f);
	}
}
