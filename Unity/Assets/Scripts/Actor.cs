using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// --------------------------------------------------------------------
// Actor
// --------------------------------------------------------------------
// any 'sentient' and active game object that actually should be targetable
// and recognized as such
//
public class Actor : Entity, ITargetable {

	// this is a locator that we use to have an independent frame 
	// for rotation (bank, tilt) 
	//
	public GameObject orienter;	

	// the mesh
	//
	protected string prefabName = "Spacepod"; // default mesh
	protected GameObject mesh;

	// --------------------------------------------------------------------
	// Targeting & Factions
	// --------------------------------------------------------------------
	//
	// list ow known targets. The Actor is responsible for maintaining
	// this list. Sensors will work with this.
	//
	public List<ITargetable> targetList = new List<ITargetable>();

	// waypoints for orders 
	// TODO should these be here? or in behaviors? 
	// TODO add path planning vectors in there
	//
	protected List<Vector3> waypointList = new List<Vector3>();

	// faction id
	// 
	[SerializeField]
	protected int faction;

	// --------------------------------------------------------------------
	// Stats
	// --------------------------------------------------------------------
	//
	// stats
	// TODO make an object for this with modifiers etc
	//
	public float maxHp = 100.0f;
	public float hp    = 100.0f;

	protected Actor lastHit;
	public int score = 0;

	// --------------------------------------------------------------------
	// Mountpoints
	// --------------------------------------------------------------------
	// Mountpoints for weapons, items etc. are added below this
	//
	// TODO define what kinds of items they can accept.
	// 
	public List<MountPoint> mountPointList = new List<MountPoint>();
	
	// --------------------------------------------------------------------
	// Controller Code
	// --------------------------------------------------------------------
	// Controller, this is the code that
	// handles input and output
	// can be Player or an AI controller
	//
	public Controller controller;

	// --------------------------------------------------------------------
	// Locomotor Code
	// --------------------------------------------------------------------
	// Locomotor, this is the code that
	// actually performs any movement
	//
	public Locomotor locomotor;

	// --------------------------------------------------------------------
	// Default Code
	// --------------------------------------------------------------------
	// Use this for initialization
	//
	void Start () {
		
		init ();
		//Notifier.addListener("ENTITY.DESTROY", onTargetDied);
	}

	// --------------------------------------------------------------------
	// Update Code
	// --------------------------------------------------------------------
	// Update is called once per frame
	//
	void Update () {

		if (!isInitialized) return;

		// update target list
		//
		updateTargets();

		// update the controller
		//
		controller.updateControls();

		// update the locomotor
		//
		locomotor.Locomote();
	}

	// --------------------------------------------------------------------
	// Initialization
	// --------------------------------------------------------------------
	//
	// This builds the structure we need so make sure it is called
	//
	public override void init()
	{
		if (isInitialized) return;

		base.init();

		// name
		name = "Actor";

		// tag
		this.gameObject.tag = "Actor";

		// setup clear spawn point
		t = transform;
		t.position = new Vector3(0,0,0);

		// init orienter and parent
		//
		orienter = new GameObject();
		orienter.transform.parent = this.transform;
		orienter.name = "Orienter";

		// damage collider
		//
		SphereCollider s = gameObject.AddComponent<SphereCollider>();
		s.radius = 5.0f;
		s.isTrigger = false;

		// rigid body with damage collider
		Rigidbody r = gameObject.AddComponent<Rigidbody>();
		if (r != null)
		{
			r.useGravity = false;
			r.isKinematic = true;
		}

		// sensor
		//
		MountPoint m = addMountPoint(new Vector3(), Quaternion.identity, "MountPointSensor");
		MountableSensor sns = m.gameObject.AddComponent<MountableSensor>();
		sns.setOwner(this);
		sns.init();
		mount(sns, "MountPointSensor");

		// add weapon MountPoints
		//
		m = addMountPoint(new Vector3(), Quaternion.identity, "MountPointWeapon");
		
		// add a weapon
		//
		LaserWeapon w = m.gameObject.AddComponent<LaserWeapon>();
		w.setOwner(this);
		w.init();
		mount(w, "MountPointWeapon");

		
		// add default mesh
		// 
		initMesh();

		// stats
		//
		hp = maxHp;

		isInitialized = true;

		initController ();
		initLocomotor ();
	}

	// init controller - override for specific controller
	//
	public virtual void initController()
	{
		controller = gameObject.AddComponent<AIController>();
		controller.setOwner(this);
		controller.init();
	}

	// init locomotor - override for specific locomotor
	//
	public virtual void initLocomotor()
	{
		locomotor = gameObject.AddComponent<Locomotor>();
		locomotor.setOwner(this);
		locomotor.init();
	}
	
	// initialize with mesh - override for specifics 
	//
	public virtual void initMesh()
	{
		// instantiate from prefab and parent to orient
		//
		mesh = Instantiate(Resources.Load(prefabName), t.position, t.rotation) as GameObject; 
		mesh.transform.parent = orienter.transform;
	}

	// --------------------------------------------------------------------
	// Getters and Setters
	// --------------------------------------------------------------------
	//
	public int getFaction() 
	{
		return faction;
	}

	public void setFaction(int f)
	{
		faction = f;

		// dispatch message
		//
		broadcast(new Notification(this, "ACTOR.SETFACTION", f));
	}

	// --------------------------------------------------------------------
	// ITargetable
	// --------------------------------------------------------------------
	//
	public Vector3 getPosition()
	{
		if (alive) return t.position;

		return new Vector3();
	}

	// --------------------------------------------------------------------
	// Mountpoints
	// --------------------------------------------------------------------
	//
	// Add a new mountpoint at the given offset
	// Should only be donw by the factory - no need to remove any mountpoints 
	// at this time.
	//
	public MountPoint addMountPoint(Vector3 offset, Quaternion rotation, string name)
	{
		GameObject goMountPoint = new GameObject();
		MountPoint mountPoint = goMountPoint.AddComponent<MountPoint>();
		mountPoint.transform.position = offset;
		mountPoint.transform.rotation = rotation;
		mountPoint.transform.parent = orienter.transform;
		mountPoint.init ();
		mountPoint.name = name;
		mountPointList.Add(mountPoint);

		return mountPoint;
	}

	// mount item
	//
	public bool mount(IMountable item, string mountPointName)
	{
		// get the mount point
		//
		MountPoint m = getMountPointByName(mountPointName);

		if (m != null)
		{
			m.mount(item);
			return true;
		}

		Debug.Log ("WARNING: Attempted to mount item " + (item as Entity).name + " to MountPoint " + mountPointName + " - Mountpoint was not found.");
		return false;
	}

	// dismount item
	//
	public bool dismount(string mountPointName)
	{
		// get the mount point
		//
		MountPoint m = getMountPointByName(mountPointName);
		
		if (m != null)
		{
			m.dismount();
			return true;
		}
		Debug.Log ("WARNING: Attempted to dismount from MountPoint " + mountPointName + " - Mountpoint was not found.");
		return false;
	}


	// find mount point by name
	//
	protected MountPoint getMountPointByName(string name)
	{
		foreach (MountPoint m in mountPointList)
		{
			if (m != null)
			{
				if (m.name == name)
				{
					return m;
				}
			}
		}

		return null;
	}

	// --------------------------------------------------------------------
	// Sensing
	// --------------------------------------------------------------------
	//
	public void addTarget(ITargetable tgt)
	{
		targetList.Add (tgt);

		(tgt as Entity).addListener("ENTITY.DESTROY", onTargetDied);

		// dispatch message
		//
		broadcast(new Notification(this, "ACTOR.ADDTARGET", tgt));
	}

	public void removeTarget(ITargetable tgt)
	{
		foreach (ITargetable t in targetList)
		{
			if (t == tgt)
			{
				targetList.Remove(t);
				break;
			}
		}

		// dispatch message
		//
		broadcast(new Notification(this, "ACTOR.LOSTTARGET", tgt));
	}

	private void updateTargets()
	{
		List<ITargetable> killList = new List<ITargetable>();

		foreach (ITargetable tgt in targetList)
		{
			// HACK should Behaviour int MountableSensor somehow? 
			//
			if ((tgt == null) || (Vector3.Distance(tgt.getPosition(), t.position) > 100.0f))
			{
				killList.Add(tgt);
			}
			else
			{
				// DEBUG
				Vector3 newDirection = new Vector3(tgt.getPosition().x - t.position.x, tgt.getPosition().y - t.position.y, tgt.getPosition().z - t.position.z);
				Debug.DrawRay (transform.position, newDirection, Color.red);
			}
		}

		foreach (ITargetable tgt in killList)
		{
			targetList.Remove(tgt);
		}

		killList.Clear();
	}

	public ITargetable getTarget()
	{
		foreach (ITargetable t in targetList)
		{
			if ((t != null) && (t is Actor) && ((t as Actor).getFaction() != faction))
			{
				return t as ITargetable;
			}
		}

		return null;
	}

	// message handler for ENTITY.DESTROY
	//
	private void onTargetDied(Notification n) 
	{
		removeTarget(n.sender as ITargetable);
		(n.sender as Entity).removeListener("ENTITY.DESTROY", onTargetDied);
	}

	// --------------------------------------------------------------------
	// Waypoints
	// --------------------------------------------------------------------
	//
	public virtual void addWaypoint(Vector3 w)
	{
		waypointList.Add(w);
	}

	public virtual void removeWaypoint(Vector3 w)
	{
		waypointList.Remove(w);
	}

	public virtual void removeFirstWaypoint()
	{
		if (waypointList.Count > 0)
		{
			waypointList.Remove(waypointList[0]);
		}
	}

	public bool hasWaypoints()
	{
		// HACK create some new random waypoints...
		//
		if (waypointList.Count == 0)
		{

			Debug.Log ("Adding random locations...");
			addWaypoint(Target.getRandomTargetLocation().getTargetLocation());
			addWaypoint(Target.getRandomTargetLocation().getTargetLocation());
			addWaypoint(Target.getRandomTargetLocation().getTargetLocation());
			addWaypoint(Target.getRandomTargetLocation().getTargetLocation());
		}

		return (waypointList.Count > 0);
	}

	public Vector3 getNextWaypoint()
	{
		if (hasWaypoints())
		{
			return waypointList[0];
		}
		else
		{
			return new Vector3();
			Debug.LogWarning("Warning: No valid waypoint returned!");
		}
	}


	// --------------------------------------------------------------------
	// Fire weapons
	// --------------------------------------------------------------------
	// TODO make specific? 
	//
	public virtual void fireWeapons()
	{
		// get any weapons...
		//
		Weapon[] wpns = gameObject.GetComponentsInChildren<Weapon>();

		foreach (Weapon w in wpns)
		{
			w.fire();
		}

		// dispatch message
		//
		broadcast(new Notification(this, "ACTOR.FIRE", null));
	}

	// --------------------------------------------------------------------
	// Modify Stats
	// --------------------------------------------------------------------
	//
	public void damage(float dmg)
	{
		hp -= dmg;
		if (hp < 0.0f)
		{
			if (lastHit != null)
			{
				lastHit.addScore(100);
			}
			die();
		}

		// dispatch message
		//
		broadcast(new Notification(this, "ACTOR.DAMAGED", dmg));
	}

	// --------------------------------------------------------------------
	// Death
	// --------------------------------------------------------------------
	//
	public override void die()
	{
		base.die();

		Instantiate(Resources.Load("Bang"), t.position, t.rotation);

		Destroy(this.gameObject);
	}
		
	// --------------------------------------------------------------------
	// Score
	// --------------------------------------------------------------------
	//
	public void addScore(int points) 
	{
		score += points;

		// dispatch message
		//
		broadcast(new Notification(this, "ACTOR.SCORE", points));
	}


	// --------------------------------------------------------------------
	// Collision Detection
	// --------------------------------------------------------------------
	//
	// collision
	// TODO make dependent of the hit and what hit me (damage settings etc)
	//
	void OnCollisionEnter(Collision col)
	{
		doCollision(col);

	}

	// this method can be overridden to do custom
	// collision
	//
	protected virtual void doCollision(Collision col)
	{
		// dispatch message
		//
		broadcast(new Notification(this, "ACTOR.COLLISION", null));
	}
}
