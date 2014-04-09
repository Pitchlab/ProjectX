using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Actor : MonoBehaviour {

	// --------------------------------------------------------------------
	// Basic stuff
	// --------------------------------------------------------------------
	// initialized?
	//
	[SerializeField]
	private bool initialized = false;

	// --------------------------------------------------------------------
	// Game Object & World Related
	// --------------------------------------------------------------------
	// The basic transform, cached.
	//
	public Transform 	t;

	// this is a locator that we use to have an independent frame 
	// for rotation (bank, tilt) 
	//
	public GameObject orienter;	

	// the mesh
	//
	public string prefabName = "Spacepod"; // default mesh
	public GameObject mesh;

	// --------------------------------------------------------------------
	// Targeting & Factions
	// --------------------------------------------------------------------
	//
	// list ow known targets. The Actor is responsible for maintaining
	// this list. Sensors will work with this.
	//
	public List<Target> targetList = new List<Target>();

	// the currently targeted location (if any)
	// we'll put a sphere there for debug
	// TODO make invisible
	//
	public GameObject targetMarker;

	// faction id
	// 
	[SerializeField]
	private int faction;

	// --------------------------------------------------------------------
	// Stats
	// --------------------------------------------------------------------
	//
	// stats
	// TODO make an object for this with modifiers etc
	//
	public float maxHp = 100.0f;
	public float hp    = 100.0f;
	private Player lastHit;

	// --------------------------------------------------------------------
	// Mountpoints
	// --------------------------------------------------------------------
	// Mountpoints for weapons, items etc. are added below this
	//
	// TODO define what kinds of items they can accept.
	// 
	public GameObject mountPoints;
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
	}

	// --------------------------------------------------------------------
	// Update Code
	// --------------------------------------------------------------------
	// Update is called once per frame
	//
	void Update () {

		if (!initialized) return;

		// update target list
		//
		updateTargets();

		// update the controller
		//
		controller.updateControls();

		// update the locomotor
		//
		locomotor.Locomote();

		// DEBUG
		//
		//Vector3 newDirection = new Vector3(targetMarker.transform.position.x - t.position.x, targetMarker.transform.position.y - t.position.y, targetMarker.transform.position.z - t.position.z);
		//Debug.DrawRay (transform.position, newDirection, Color.green);
	}

	// --------------------------------------------------------------------
	// Initialization
	// --------------------------------------------------------------------
	//
	// This builds the structure we need so make sure it is called
	//
	public void init()
	{
		if (initialized) return;

		// name
		name = "Actor";

		// tag
		gameObject.tag = "Actor";

		// setup clear spawn point
		t = transform;
		t.position = new Vector3(0,0,0);

		// init orienter and parent
		//
		orienter = new GameObject();
		orienter.transform.parent = this.transform;
		orienter.name = "Orienter";
		
		// target marker
		// 
		targetMarker = new GameObject("Marker");

		// damage collider
		//
		SphereCollider s = gameObject.AddComponent<SphereCollider>();
		s.radius = 5.0f;
		s.isTrigger = false;

		// rigid body with damage collider
		Rigidbody r = gameObject.AddComponent<Rigidbody>();
		r.useGravity = false;
		//r.mass = 100000000;
		r.isKinematic = true;
		
		// add default mesh
		// 
		initMesh();

		// stats
		//
		hp = maxHp;

		// add MountPoints
		//
		addMountPoint(new Vector3(), Quaternion.identity, "MountPoint");

		// add a sensor
		//
		GameObject goMountableSensor = new GameObject();
		MountableSensor m = goMountableSensor.AddComponent<MountableSensor>();
		m.init();
		m.setOwner(this);
		mount(m, "MountPoint");

		// init perceptor, controller, locomotor
		//
		//initPerceptor();
		initController();
		initLocomotor();

		initialized = true;
	}

	// init perceptor - override for specific perceptor
	//
	public virtual void initPerceptor()
	{
		//perceptor = new AIPerceptor(this);
	}

	// init controller - override for specific controller
	//
	public virtual void initController()
	{
		controller = new AIController(this);
	}

	// init locomotor - override for specific locomotor
	//
	public virtual void initLocomotor()
	{
		locomotor = new Locomotor(this);
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
	// Mountpoints
	// --------------------------------------------------------------------
	//
	// Add a new mountpoint at the given offset
	// Should only be donw by the factory - no need to remove any mountpoints 
	// at this time.
	//
	public void addMountPoint(Vector3 offset, Quaternion rotation, string name)
	{
		GameObject goMountPoint = new GameObject();
		MountPoint mountPoint = goMountPoint.AddComponent<MountPoint>();
		mountPoint.transform.position = offset;
		mountPoint.transform.rotation = rotation;
		mountPoint.transform.parent = orienter.transform;
		mountPoint.transform.name = name;

		mountPointList.Add(mountPoint);
	}

	// mount item
	//
	public bool mount(MountableItem item, string mountPointName)
	{
		// get the mount point
		//
		MountPoint m = getMountPointByName(mountPointName);

		if (m != null)
		{
			m.mount(item);
			return true;
		}

		Debug.Log ("WARNING: Attempted to mount item " + item.name + " to MountPoint " + mountPointName + " - Mountpoint was not found.");
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
	private MountPoint getMountPointByName(string name)
	{

		MountPoint m = mountPointList.Find( ni => ni.name == name);

		return m;
	}

	// --------------------------------------------------------------------
	// Sensing
	// --------------------------------------------------------------------
	//
	public void addTarget(Collider col)
	{
		Debug.Log("Adding: " + col.transform.gameObject);

		// HACK using tags is unsafe
		if (col.tag == "Actor")
		{
			Actor a = col.transform.gameObject.GetComponent<Actor>();
			Target t = new Target(a);
			targetList.Add (t);
		}
		else if (col.tag == "Projectile")
		{
			Projectile p = col.transform.gameObject.GetComponent<Projectile>();
			Target t = new Target(p);
			targetList.Add (t);
		}
		else
		{
			Target t = new Target(col.transform.position);
			targetList.Add (t);
		}

	}

	public void removeTarget(Collider col)
	{
		Debug.Log("Removing: " + col.transform.gameObject);

		foreach (Target t in targetList)
		{
			if (t.targetIsActor())
			{
				Actor a = t.getTargetActor();

				if (col.transform.gameObject.GetComponent<Actor>() == a)
				{
					targetList.Remove(t);
					break;
				}
			}
			else if (t.targetIsProjectile())
			{
				Projectile p = t.getTargetProjectile();
				
				if (col.transform.gameObject.GetComponent<Projectile>() == p)
				{
					targetList.Remove(t);
					break;
				}
			}
		}
	}

	private void updateTargets()
	{
		List<Target> killList = new List<Target>();

		foreach (Target tgt in targetList)
		{
			tgt.update();

			// HACK should Behaviour int MountableSensor somehow? 
			//
			if (tgt.getDistance(t.position) > 100.0f)
			{
				tgt.die ();
			}

			if (!tgt.isAlive())
			{
				killList.Add(tgt);
			}
			else
			{
				// DEBUG
				Vector3 newDirection = new Vector3(tgt.getTargetLocation().x - t.position.x, tgt.getTargetLocation().y - t.position.y, tgt.getTargetLocation().z - t.position.z);
				Debug.DrawRay (transform.position, newDirection, Color.red);
			}
		}

		foreach (Target tgt in killList)
		{
			targetList.Remove(tgt);
		}

		killList.Clear();
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
	}

	// --------------------------------------------------------------------
	// Death
	// --------------------------------------------------------------------
	//
	public void die()
	{
		Instantiate(Resources.Load("Bang"), t.position, t.rotation);
		Destroy(gameObject);

		// remove target sphere
		//
		Destroy(targetMarker);
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
		Debug.Log ("Collision!" + col.ToString());
	
		// check what hit us
		//
		GameObject go = col.gameObject;

		if (col.gameObject.tag != "Projectile") return;

		// Projectile? 
		Projectile p = go.GetComponent<Projectile>();
		if (p != null)
		{
			p.owner.addScore(10);
			lastHit = p.owner;
		}

		// attribute points
		// 
		damage (10.0f);
	}
}
