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
	private Actor 		tgtActor;			// if a specific actor is targeted
	
	[SerializeField]
	private Projectile 	tgtProjectile;		// if a specific projectile is targeted

	[SerializeField]
	private Vector3 	tgtLocation;		// if a target location is always set
	
	[SerializeField]
	private float 		age = 0.0f;			// deltaTime

	[SerializeField]
	private float 		maxAge = 100.0f;	// deltaTime

	[SerializeField]
	private bool 		alive = false;		// is the target still viable (after timeout it 'dies' and can be removed)

	// --------------------------------------------------------------------
	// CONSTRUCTORS 
	// --------------------------------------------------------------------
	// Constructor: set actor directly
	//
	public Target(Actor a)
	{
		setTarget(a);
	}

	// Constructor: set actor directly with max age
	//
	public Target(Actor a, int maxAge)
	{
		setTarget(a);
		setMaxAge(maxAge);
	}

	// Constructor: set projectile directly
	//
	public Target(Projectile p)
	{
		setTarget(p);
	}

	// Constructor: set projectile directly with max age
	//
	public Target(Projectile p, int maxAge)
	{
		setTarget(p);
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

	// --------------------------------------------------------------------
	// SETTERS & GETTERS
	// --------------------------------------------------------------------
	// set location, if there was an actor, forget that
	// 
	public void setTarget(Vector3 l)
	{
		tgtLocation = l;
		tgtActor = null;
		resetAge();
	}

	// set actor, if there was a location, forget that
	//
	public void setTarget(Actor a)
	{
		tgtActor = a;
		tgtLocation = a.transform.position;
		resetAge();
	}

	// set projectile, if there was a location, forget that
	//
	public void setTarget(Projectile p)
	{
		tgtProjectile = p;
		tgtLocation = p.transform.position;
		resetAge();
	}

	public bool targetIsActor()
	{
		return (tgtActor != null);
	}

	public bool targetIsProjectile()
	{
		return (tgtProjectile != null);
	}

	public Vector3 getTargetLocation()
	{
		return tgtLocation;
	}

	public Actor getTargetActor()
	{
		return tgtActor;
	}

	public Projectile getTargetProjectile()
	{
		return tgtProjectile;
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

	// get the distance to the target location or actor
	//
	public float getDistance(Vector3 pos)
	{
		if (tgtActor != null)
		{
			return Vector3.Distance (tgtActor.transform.position, pos);
		}
		else 
		{
			return Vector3.Distance (tgtLocation, pos);
		}
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

			if (targetIsActor())
			{
				tgtLocation = tgtActor.transform.position;
			}
			else if (targetIsProjectile())
			{
				tgtLocation = tgtProjectile.transform.position;
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

	// degrade the marker
	// from a live actor mark a position and reset age
	// from a position set age to -1
	// 
	public void degrade() 
	{
		if (targetIsActor() || targetIsProjectile ())
		{
			tgtActor = null;
			tgtProjectile = null;
			resetAge();
		}
		else
		{
			die ();
		}
	}
}
