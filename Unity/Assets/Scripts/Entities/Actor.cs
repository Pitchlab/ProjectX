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
	protected string prefabName = "NPC/dart"; // default mesh
	protected GameObject mesh;

	protected GameObject selectMesh;

	// selected 
	//
	protected bool isSelected = false;
		
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

		// HACK - create selection marker quad
		//		
		Debug.Log ("aaa");
		
		Mesh quad = new Mesh();
		
		Vector3[] vertices = new Vector3[]
		{
			new Vector3( 1, 1,  0),
			new Vector3( 1, -1, 0),
			new Vector3(-1, 1, 0),
			new Vector3(-1, -1, 0),
		};
		
		Vector2[] uv = new Vector2[]
		{
			new Vector2(1, 1),
			new Vector2(1, 0),
			new Vector2(0, 1),
			new Vector2(0, 0),
		};
		
		int[] triangles = new int[]
		{
			0, 1, 2,
			2, 1, 3,
		};
		
		quad.vertices = vertices;
		quad.uv = uv;
		quad.triangles = triangles;
		quad.RecalculateNormals();
		
		// Create object
		selectMesh = (GameObject) new GameObject(
			"Circle", 
			typeof(MeshRenderer), // Required to render
			typeof(MeshFilter)    // Required to have a mesh
			);
		selectMesh.GetComponent<MeshFilter>().mesh = quad;
		
		// Set texture
		var tex = (Texture) Resources.Load ("Textures/Helpers/circle");
		selectMesh.renderer.material.mainTexture = tex;
		
		// Set shader for this sprite; unlit supporting transparency
		// If we dont do this the sprite seems 'dark' when drawn. 
		var shader = Shader.Find ("Unlit/Transparent");
		selectMesh.renderer.material.shader = shader;
		
		// Set position
		//item.transform.position = new Vector3(1, 1, 1);
		selectMesh.transform.position = t.position;
		selectMesh.transform.Translate(0.0f, 0.0f, -0.5f);
		selectMesh.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
		selectMesh.transform.Rotate(new Vector3(90.0f,0f,0f));
		selectMesh.transform.parent = t;
	}

	// --------------------------------------------------------------------
	// Update Code
	// --------------------------------------------------------------------
	// Update is called once per frame
	//
	void Update () {

		if (!isInitialized) return;

		// HACK check if selected
		// 
		if (Input.GetMouseButtonUp(0))
		{
			Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
			camPos.y = Selector.convertToScreenY(camPos.y);
			isSelected = Selector.selection.Contains(camPos);
		}

		// HACK faction color and select color
		//
		Color c = Color.gray;
		if (faction == 1)
		{
			c = Color.red;

			//if (isSelected)
			//{
			//	c = Color.magenta;
			//}
		}
		else if (faction == 2)
		{
			c = Color.blue;

			//if (isSelected)
			//{
			//	c = Color.cyan;
			//}
		}

		foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
		{
			r.material.color = c;
		}

		// update target list
		//
		updateTargets();

		// update the controller
		//
		if (controller != null) controller.updateControls();

		// update the locomotor
		//
		if (locomotor != null) locomotor.Locomote();

		// HACK SELECTING: show circle
		//
		selectMesh.renderer.enabled = isSelected;
		
			
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

		buildOrienter();

		buildCollision();

		buildMountPoints();

		buildSensors();

		buildWeapons();

		buildMesh();

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
	
	// --------------------------------------------------------------------
	// Build the object
	// --------------------------------------------------------------------
	//

	// Orienter
	//
	public virtual void buildOrienter()
	{
		// init orienter and parent
		//
		orienter = new GameObject();
		orienter.transform.parent = this.transform;
		orienter.name = "Orienter";
	}

	// Collision
	//
	public virtual void buildCollision()
	{
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
	}

	// Mount points
	//
	public virtual void buildMountPoints()
	{
		addMountPoint(new Vector3(), Quaternion.identity, "MountPointSensor");
		addMountPoint(new Vector3(), Quaternion.identity, "MountPointWeapon");
	}

	// Sensor array
	//
	public virtual void buildSensors()
	{
		GameObject g = new GameObject();
		MountableSensor s = g.AddComponent<MountableSensor>();
		mount(s, "MountPointSensor");
		s.setOwner(this);
		s.init();
	}

	// Weapon array
	//
	public virtual void buildWeapons()
	{
		// add a weapon
		//
		GameObject g = new GameObject();
		LaserWeapon w = g.AddComponent<LaserWeapon>();
		mount(w, "MountPointWeapon");
		w.setOwner(this);
		w.init();
	}

	// initialize with mesh - override for specifics 
	//
	public virtual void buildMesh()
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
		if (owner != null) return owner.faction;

		return faction;
	}

	public void setFaction(int f)
	{
		faction = f;

		// dispatch message
		//
		broadcast(new Notification(this, "ACTOR.SETFACTION", f));
	}

	public virtual void selectUnit()
	{
		isSelected = true;

	}

	public virtual void deselectUnit()
	{
		isSelected = false;
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
	public virtual MountPoint addMountPoint(Vector3 offset, Quaternion rotation, string name)
	{
		GameObject goMountPoint = new GameObject();
		MountPoint mountPoint = goMountPoint.AddComponent<MountPoint>();
		mountPoint.transform.position = offset;
		mountPoint.transform.rotation = rotation;
		mountPoint.transform.parent = orienter.transform;
		mountPoint.init ();
		mountPoint.setLabel (name);
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
				if (m.getLabel() == name)
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

		Instantiate(Resources.Load("FX/Bang"), t.position, t.rotation);

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
