using UnityEngine;
using System.Collections;

// --------------------------------------------------------------------
// MountPoint
// --------------------------------------------------------------------
// The MountPoint is an empty GameObject that can be used to mount items
// to. The MountPoint will accept certain items. It can also be labeled
// so that the interface can link to it.
//
public class MountPoint : MonoBehaviour {

	// --------------------------------------------------------------------
	// Characteristics
	// --------------------------------------------------------------------
	[SerializeField]
	private bool isInitialized 	= 	false;
	
	// --------------------------------------------------------------------
	// Initialize
	// --------------------------------------------------------------------
	// Use this for initialization
	void Start () 
	{
		init();
	}

	public void init()
	{
		if (isInitialized) return;

		// important stuff here...
		//

		isInitialized = true;
	}

	// --------------------------------------------------------------------
	// Setters & Getters
	// --------------------------------------------------------------------
	//
	// get the mounted item - or null
	//
	public MountableItem getMountedItem()
	{
		return gameObject.GetComponent<MountableItem>();
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
	public bool mount(MountableItem item)
	{
		dismount ();

		item.transform.parent = gameObject.transform;

		return true;
	}

	// dismount the mounted item
	//
	public bool dismount()
	{
		MountableItem item = getMountedItem();
		if (item != null)
		{
			Destroy(item);

			return true;
		}
		return false;
	}
}
