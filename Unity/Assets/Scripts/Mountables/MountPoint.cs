using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// --------------------------------------------------------------------
// MountPoint
// --------------------------------------------------------------------
// The MountPoint is an empty GameObject that can be used to mount items
// to. The MountPoint will accept certain items. It can also be labeled
// so that the interface can link to it.
//
public class MountPoint : Entity 
{

	public override void init()
	{
		if (isInitialized) return;
		
		base.init ();
		
		// important stuff here...
		//
		name = "MountPoint";
		
		isInitialized = true;
	}

	// --------------------------------------------------------------------
	// Setters & Getters
	// --------------------------------------------------------------------
	//
	// get the mounted item - or null
	//
	public IMountable getMountedItem()
	{
		Entity[] list = gameObject.GetComponents<Entity>();

		foreach (Entity e in list)
		{
			if (e is IMountable) return e as IMountable;
		}

		return null;
	}

	// --------------------------------------------------------------------
	// Update
	// --------------------------------------------------------------------
	// Update is called once per frame
	void Update () {
	
	}

	// --------------------------------------------------------------------
	// Mount and Dismount
	// --------------------------------------------------------------------
	// 
	// mount a new item (can only be done if the mount point has not been used)
	// otherwise destroy the item
	//
	public bool mount(IMountable item)
	{
		// dismount any previous items
		//
		dismount ();

		(item as Entity).transform.parent = gameObject.transform;

		return true;
	}

	// dismount the mounted item
	//
	public bool dismount()
	{
		IMountable item = getMountedItem();
		if (item != null)
		{
			Destroy(item as Entity);

			return true;
		}
		return false;
	}
}
