using UnityEngine;
using System.Collections;


// --------------------------------------------------------------------
// MountableSensor
// --------------------------------------------------------------------
// This is a gameObject with its own collision trigger that is used to
// detect enemies and objectives in range. It will need access to the 
// owner's target list.
//
public class MountableSensor : MountableItem {

	[SerializeField]
	private bool isInitialized = false;	

	[SerializeField]
	private Actor owner; 				// who owns this

	[SerializeField]
	private float radius = 50.0f;		// sensor radius

	// Use this for initialization
	//
	void Start () 
	{
		init ();
	}

	public void init()
	{
		if (isInitialized) return;

		name = "MountableSensor";

		// damage collider
		//
		SphereCollider s = gameObject.AddComponent<SphereCollider>();
		s.radius = radius;
		s.isTrigger = true;
		s.tag = "Detector";

		isInitialized = true;
	}

	public void setOwner(Actor a)
	{
		owner = a;
	}

	// Update is called once per frame
	//
	void Update () 
	{
		if (!isInitialized) return;
	}

	void OnTriggerEnter(Collider col)
	{
		// tell the owner
		//
		if (owner != null)
		{
			if ((col.tag == "Actor") || (col.tag == "Projectile"))
			{
				Debug.Log (col.transform.name + "(" + col.tag + ")" + " entered my field of view");
				owner.addTarget(col);
			}
		}
	}
	
	void OnTriggerExit(Collider col)
	{
		if (owner != null)
		{
			if ((col.tag == "Actor") || (col.tag == "Projectile"))
			{
				Debug.Log (col.transform.name + "(" + col.tag + ")" + " left my field of view");
				owner.removeTarget(col);
			}
		}
	}
}
