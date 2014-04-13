using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// --------------------------------------------------------------------
// Entity
// --------------------------------------------------------------------
// Basic object to use for any custom items in the game
//
public class Entity : MonoBehaviour, INotifier {

	// --------------------------------------------------------------------
	// Initialization
	// --------------------------------------------------------------------
	[SerializeField]
	protected bool isInitialized = false;	

	[SerializeField]
	protected bool alive = true;

	// --------------------------------------------------------------------
	// INotifier related
	// --------------------------------------------------------------------
	static private Dictionary<string, Delegate> callbackTable = new Dictionary<string, Delegate>();

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
	public Actor owner; 				// who owns this

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
	// alive
	//
	public virtual bool isAlive()
	{
		return alive;
	}

	// owner
	//
	public virtual void setOwner(Actor a)
	{
		owner = a;
	}

	// --------------------------------------------------------------------
	// Death
	// --------------------------------------------------------------------
	//
	public virtual void die()
	{
		alive = false;

		// mark for destruction
		//
		Destroy(this.gameObject);
	}

	// notify others that we are destroyed... 
	// TODO should make this more specific.
	//
	void OnDestroy()
	{
		//Notifier.broadcast(new Notification(this, "ENTITY.DESTROY", null));
		broadcast(new Notification(this, "ENTITY.DESTROY", null));
	}

	// --------------------------------------------------------------------
	// INotifier
	// --------------------------------------------------------------------
	public void addListener(string type, NotificationCallback c)
	{
		if (!callbackTable.ContainsKey(type)) 
		{
			callbackTable.Add(type, null);
		}
		
		// this is added if we have differnt formats for the callback function... 
		//
		Delegate d = callbackTable[type];
		if (d != null && d.GetType() != c.GetType()) 
		{
			throw new Notifier.ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", type, d.GetType().Name, c.GetType().Name));
		}
		
		callbackTable[type] = (NotificationCallback)callbackTable[type] + c;
	}
	
	public void removeListener(string type, NotificationCallback c)
	{
		if (callbackTable.ContainsKey(type)) 
		{
			Delegate d = callbackTable[type];
			
			if (d == null) 
			{
				throw new Notifier.ListenerException(string.Format("Attempting to remove listener with for event type {0} but current listener is null.", type));
			} else if (d.GetType() != c.GetType()) 
			{
				throw new Notifier.ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", type, d.GetType().Name, c.GetType().Name));
			}
		} 
		else 
		{
			throw new Notifier.ListenerException(string.Format("Attempting to remove listener for type {0} but Messenger doesn't know about this event type.", type));
		}
		
		callbackTable[type] = (NotificationCallback)callbackTable[type] - c;
		
		
		if (callbackTable[type] == null) 
		{
			callbackTable.Remove(type);
		}
	}
	
	public void broadcast(Notification n)
	{
		Delegate d;
		if (callbackTable.TryGetValue(n.type, out d)) 
		{
			NotificationCallback callback = d as NotificationCallback;
			if (callback != null) 
			{
				callback(n);
			} 
			else 
			{
				throw new Notifier.BroadcastException(string.Format("Attempting to broadcast event type {0} but there is no callback registered.", n.type));
			}
		}
	}
}
