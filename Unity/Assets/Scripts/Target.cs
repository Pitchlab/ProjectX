using UnityEngine;
using System.Collections;

// A Target in the world
// 
// Can be either a game object or a location, never both
// Use the age to determine if contact should be lost
// 
// TODO target sub objects? 
// 
//
public class Target : Object 
{

	[SerializeField]
	private ITargetable 	target;				// if a specific entity is targeted
	
	[SerializeField]
	private Vector3 		tgtLocation;		// if a target location is always set
	
	[SerializeField]
	private float 			age = 0.0f;			// deltaTime

	[SerializeField]
	private float 			maxAge = 100.0f;	// deltaTime

	[SerializeField]
	private bool 			alive = false;		// is the target still viable (after timeout it 'dies' and can be removed)

	// --------------------------------------------------------------------
	// CONSTRUCTORS 
	// --------------------------------------------------------------------
	// Constructor: set actor directly
	//
	public Target(ITargetable t)
	{
		setTarget(t);
	}

	// Constructor: set actor directly with max age
	//
	public Target(ITargetable t, int maxAge)
	{
		setTarget(t);
		setMaxAge(maxAge);
	}
	
	// Constructor: set location directly
	//
	public Target(Vector3 l)
	{
		setTarget(l);
	}

	// Constructor: set location directly with max age
	//
	public Target(Vector3 l,  int maxAge)
	{
		setTarget(l);
		setMaxAge(maxAge);
	}

	// get a new random location
	//
	public static Target getRandomTargetLocation()
	{
		return new Target(new Vector3(Random.value * 400.0f - 200.0f,0.0f,Random.value * 4000.0f - 200.0f));
	}
	
	// --------------------------------------------------------------------
	// SETTERS & GETTERS
	// --------------------------------------------------------------------
	// set location, if there was an actor, forget that
	// 
	public void setTarget(Vector3 l)
	{
		tgtLocation = l;
		target = null;
		resetAge();
	}

	// set actor, if there was a location, forget that
	//
	public void setTarget(ITargetable t)
	{
		target = t;
		tgtLocation = (t as Entity).transform.position;
		resetAge();
	}
	
	public Vector3 getTargetLocation()
	{
		return tgtLocation;
	}

	public ITargetable getTarget()
	{
		return target;
	}

	// Returns true if alive
	//
	public bool isAlive()
	{
		return alive;
	}

	// set the max age
	private void setMaxAge(float max)
	{
		maxAge = max;
	}

	// --------------------------------------------------------------------
	// LOGIC
	// --------------------------------------------------------------------
	//
	// Update the age of the character - do this whenever you can
	// if no argument is given, assume update of 1. Otherwise use 
	// deltatime.
	// The location is updated as well.
	//
	public void update()
	{
		update (1.0f);
	}

	public void update(float deltatime)
	{
		if (age > -1.0f)
		{
			age += deltatime;

			if ((target as Entity) != null)
			{
				tgtLocation = (target as Entity).transform.position;
			}
		}
		
		if (age > maxAge)
		{
			die();
		}
	}

	private void resetAge()
	{
		age = 0.0f;
		alive = true;
	}

	public void die() 
	{
		alive = false;
	}
}
