using UnityEngine;
using System.Collections;

public class ProjectileEmitter : MonoBehaviour {
	

	public string meshName;					// the name of the projectile
	
	[SerializeField]
	private GameObject 	projectileFab;		// placeholder for instantiation

	public Actor 		owner;
	
	// launch a projectile
	//
	public void Fire() 
	{
		if (owner == null) return;

		// shoot projectile
		//
		Object o = Resources.Load(meshName);
		GameObject proj = Instantiate(o, transform.position, transform.rotation) as GameObject;  

		proj.GetComponent<Projectile>().owner = owner;
	}
}
