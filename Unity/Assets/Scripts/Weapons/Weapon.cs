using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : Entity, IMountable {

	// list of emitters
	//
	protected List<ProjectileEmitter> emitterList = new List<ProjectileEmitter>();
	
	[SerializeField]
	private Target target;
	
	public float coolDown = 0.1f;
	[SerializeField]
	protected float coolDownCounter;

	// --------------------------------------------------------------------
	// Initialize
	// --------------------------------------------------------------------
	// Overrides entity init
	//
	public override void init() 
	{
		if (isInitialized) return;

		base.init();

		name = "Weapon";
		
		// stuff ...


		isInitialized = true;
	}

	// --------------------------------------------------------------------
	// Targeting
	// --------------------------------------------------------------------
	// set the target
	//
	public void setTarget(Target t) 
	{
		target = t;
	}

	// clear the target
	//
	public void clearTarget() 
	{
		target = null;
	}

	// --------------------------------------------------------------------
	// Fire Weapons
	// --------------------------------------------------------------------
	// Fire weapons as specified
	//
	public virtual void fire ()
	{
		if (canFire())
		{
			foreach(ProjectileEmitter p in emitterList)
			{
				p.Fire();
			}
			resetCooldown();
		}
	}

	// --------------------------------------------------------------------
	// Cooldown
	// --------------------------------------------------------------------
	// 
	public bool canFire() 
	{
		return coolDownCounter == 0.0f;
	}

	public void resetCooldown()
	{
		coolDownCounter = coolDown;
	}

	void Update() 
	{
		coolDownCounter -= Time.deltaTime;
		if (coolDownCounter < 0.0f)
		{
			coolDownCounter = 0.0f;
		}
	}

	// --------------------------------------------------------------------
	// Setup emitter
	// --------------------------------------------------------------------
	// 
	public ProjectileEmitter addEmitter(Vector3 offset, Quaternion rotation, string name, string meshName)
	{
		if (owner == null)
		{
			Debug.Log ("Warning! adding emitter without setting owner first...");
			return null;
		}

		// add particle emitter
		//
		GameObject goEmitter = new GameObject();
		ProjectileEmitter e = goEmitter.AddComponent<ProjectileEmitter>();
		e.transform.position = offset;
		e.transform.rotation = rotation;
		e.transform.parent = this.transform;
		e.transform.name = name;
		e.meshName = meshName;
		e.owner = owner;
		emitterList.Add(e);

		return e;
	}
}
