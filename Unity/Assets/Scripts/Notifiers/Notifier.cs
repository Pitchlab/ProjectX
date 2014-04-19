using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTIFIER
//
// MESSAGE BROADCASTING WITH A SINGLETON
// 
// Format for the Event Type string: <TYPENAME>.<LABEL> e.g. ENTITY.DESTROY
//

// Delegate for event listener
// 
public delegate void NotificationCallback(Notification n);

public class Notification
{
	public object sender;
	public string type;
	public object data;
	
	public Notification(object s, string t, object d)
	{
		sender = s;
		data = d;
		type = t;
	}
}

// Interface to enable other classes (i.e. Entity) to act as an event dispatcher.
// This greatly reduces the amount of messaging and checking required. Any targets
// in an entity list can be listened to, so that they can send information on their
// behavior.
//
public interface INotifier
{
	void addListener(string type, NotificationCallback c);
	void removeListener(string type, NotificationCallback c);
	void broadcast(Notification n);
}

// Singleton class to handle Notifications
// 
static public class Notifier
{
	static private Dictionary<string, Delegate> callbackTable = new Dictionary<string, Delegate>();

	static public void addListener(string type, NotificationCallback c)
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
			throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", type, d.GetType().Name, c.GetType().Name));
		}

		callbackTable[type] = (NotificationCallback)callbackTable[type] + c;
	}

	static public void removeListener(string type, NotificationCallback c)
	{
		if (callbackTable.ContainsKey(type)) 
		{
			Delegate d = callbackTable[type];
			
			if (d == null) 
			{
				throw new ListenerException(string.Format("Attempting to remove listener with for event type {0} but current listener is null.", type));
			} else if (d.GetType() != c.GetType()) 
			{
				throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", type, d.GetType().Name, c.GetType().Name));
			}
		} 
		else 
		{
			throw new ListenerException(string.Format("Attempting to remove listener for type {0} but Messenger doesn't know about this event type.", type));
		}

		callbackTable[type] = (NotificationCallback)callbackTable[type] - c;


		if (callbackTable[type] == null) 
		{
			callbackTable.Remove(type);
		}
	}

	static public void broadcast(Notification n)
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
				throw new BroadcastException(string.Format("Attempting to broadcast event type {0} but there is no callback registered.", n.type));
			}
		}
	}

	public class BroadcastException : Exception 
	{
		public BroadcastException(string msg) : base(msg) 
		{
		}
	}
	
	public class ListenerException : Exception 
	{
		public ListenerException(string msg) : base(msg) 
		{
		}
	}
}



