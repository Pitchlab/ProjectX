using UnityEngine;
using System.Collections;

// A gun turret (must be attached to another actor)
//
public class Turret : Actor 
{
	// the gun turret and the movable guns
	//
	public string turretPrefabName = "Turrets/Turret_Top";
	public string gunsPrefabName = "Turrets/Turret_Guns";
	
	// the offset from base origin to the rotation point of 
	// the turret top
	//
	public Vector3 turretOffset = new Vector3();

	// the offset from the turret rotation point to the elevator
	// rotation point
	public Vector3 gunOffset = new Vector3(0.0f, 3.6f, 0.0f);

	// this is the thing that rotates the gun barrels up/down
	//
	//public GameObject elevator;

	// make sure that we also have an orienter and an elevator.
	//
	public override void buildOrienter ()
	{
		base.buildOrienter ();

		// move the center correctly
		//
		orienter.transform.Translate(gunOffset);
	}

	// big sensor range
	//
	public override void buildSensors ()
	{
		// sensor
		//
		MountPoint m = addMountPoint(new Vector3(), Quaternion.identity, "MountPointSensor");
		MountableSensor sns = m.gameObject.AddComponent<MountableSensor>();
		sns.setOwner(this);
		sns.init();
		sns.setRadius(500.0f);
		mount(sns, "MountPointSensor");
	}

	// build the mesh in three parts
	//
	public override void buildMesh ()
	{
		// the turret - place at root transform
		//
		GameObject turretMesh = Instantiate(Resources.Load(turretPrefabName), gameObject.transform.position, gameObject.transform.rotation) as GameObject; 
		turretMesh.transform.parent = gameObject.transform;

		// the guns - place at orienter
		//
		GameObject gunsMesh = Instantiate(Resources.Load(gunsPrefabName), orienter.transform.position, orienter.transform.rotation) as GameObject; 
		gunsMesh.transform.parent = orienter.transform;
	}
	
	// gun locomotor can only rotate and aim
	//
	public override void initLocomotor ()
	{
		locomotor = gameObject.AddComponent<TurretLocomotor>();
		locomotor.setOwner(this);
		locomotor.init();
	}

	// ai controller
	//
	public override void initController ()
	{
		controller = gameObject.AddComponent<AIController>();
		controller.setOwner(this);
		controller.init();
	}
}
