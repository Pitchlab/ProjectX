using UnityEngine;
using System.Collections;

// --------------------------------------------------------------------
// Entity
// --------------------------------------------------------------------
// Basic object to use for any custom items in the game
//
public class Entity : MonoBehaviour {

	// --------------------------------------------------------------------
	// Initialization
	// --------------------------------------------------------------------
	[SerializeField]
	protected bool isInitialized = false;	

	// --------------------------------------------------------------------
	// Game Object & World Related
	// --------------------------------------------------------------------
	// The basic transform, cached.
	//
	public Transform 	t;

	// --------------------------------------------------------------------
	// Owner and Parent
	// --------------------------------------------------------------------
	[SerializeField]
	protected Actor owner; 				// who owns this
	[SerializeField]
	protected Entity parent;			// linked to any other entity?

	// --------------------------------------------------------------------
	// Initialization code
	// --------------------------------------------------------------------
	// Make sure the entity is initialized
	//
	void Start () 
	{
		init ();
	}

	// --------------------------------------------------------------------
	// Initialization code
	// --------------------------------------------------------------------
	// Make sure the entity is initialized correctly
	//
	public virtual void init()
	{
		if (isInitialized) return;

		name = "Entity";

		isInitialized = true;
	}

	// --------------------------------------------------------------------
	// Getters & Setters
	// --------------------------------------------------------------------
	// setters
	//
	public virtual void setOwner(Actor a)
	{
		owner = a;
	}

	// --------------------------------------------------------------------
	// Getters & Setters
	// --------------------------------------------------------------------
	// setters
	//
	public void setParent(Entity e)
	{
		parent = e;
	}
}
