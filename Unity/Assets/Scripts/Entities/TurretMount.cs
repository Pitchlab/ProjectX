using UnityEngine;
using System.Collections;

// --------------------------------------------------------------------
// TurretMount
// --------------------------------------------------------------------
// The Base that is used to place a turret stand-alone in the world. 
// 
public class TurretMount : Actor 
{
	protected Turret turret;

	public override void init ()
	{
		if (isInitialized) return;

		prefabName = "Turrets/Turret_Base";

		base.init ();

		// construct the turret itself
		//
		GameObject t = new GameObject();

		// parent to this object
		t.transform.parent = gameObject.transform;

		turret = t.AddComponent<Turret>();
		turret.setOwner(this);
		turret.init();

		isInitialized = true;
	}

	// has no orienter
	//
	public override void buildOrienter ()
	{
	}
		
	// has no controller
	//
	public override void initController ()
	{
	}
	
	// has no locomotor
	//
	public override void initLocomotor ()
	{
	}

	// has no sensors
	//
	public override void buildSensors ()
	{
	}

	// has no weapons
	//
	public override void buildWeapons ()
	{
	}

	// fix mesh
	//
	public override void buildMesh ()
	{
		// instantiate from prefab and parent to orient
		//
		GameObject mesh = Instantiate(Resources.Load(prefabName), t.position, t.rotation) as GameObject; 
		mesh.transform.parent = gameObject.transform;
	}
}
