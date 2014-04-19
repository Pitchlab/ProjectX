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

		prefabName = "Player/SpaceShip";
		base.init ();

		this.name = "Player";

		setFaction(1);

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
		base.doCollision(col);

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
