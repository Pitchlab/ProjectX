using UnityEngine;
using System.Collections;


// --------------------------------------------------------------------
// MountableSensor
// --------------------------------------------------------------------
// This is a gameObject with its own collision trigger that is used to
// detect enemies and objectives in range. It will need access to the 
// owner's target list.
//
public class MountableSensor : Entity, IMountable {

	[SerializeField]
	private float radius = 50.0f;		// sensor radius

	// --------------------------------------------------------------------
	// Initialize
	// --------------------------------------------------------------------
	//
	public override void init()
	{
		if (isInitialized) return;

		base.init();

		name = "MountableSensor";

		// damage collider
		//
		SphereCollider s = gameObject.AddComponent<SphereCollider>();
		s.radius = radius;
		s.isTrigger = true;
		s.tag = "Detector";

		isInitialized = true;
	}

	// --------------------------------------------------------------------
	// Update
	// --------------------------------------------------------------------
	// Update is called once per frame
	//
	void Update () 
	{
		if (!isInitialized) return;
	}

	// --------------------------------------------------------------------
	// Sensing
	// --------------------------------------------------------------------
	// OnTriggerEnter is used to see any incoming items
	//
	void OnTriggerEnter(Collider col)
	{
		// tell the owner
		//
		//Debug.Log ("Sensor sees " + col.tag);
		if (col.gameObject.GetComponent<Entity>() is ITargetable)
		{
			//Debug.Log (col.transform.name + "(" + col.tag + ")" + " entered my field of view");

			if (owner != null)
			{
				owner.addTarget(col.gameObject.GetComponent<Entity>() as ITargetable);
			}
		}
	}
	
	void OnTriggerExit(Collider col)
	{
		if (col.gameObject.GetComponent<Entity>() is ITargetable)
		{
			if (owner != null)
			{
				//Debug.Log (col.transform.name + "(" + col.tag + ")" + " left my field of view");
				owner.removeTarget(col.gameObject.GetComponent<Entity>() as ITargetable);
			}
		}
	}
}
