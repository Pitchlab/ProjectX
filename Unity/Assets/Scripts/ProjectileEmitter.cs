using UnityEngine;
using System.Collections;

public class ProjectileEmitter : MonoBehaviour {
	
	public GameObject 	projectileFab;
	public Player 		owner;

	// Use this for initialization
	void Start () {
		// Spawn point - position = -3,-3,-1
		//transform.position = new Vector3(0,0,0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// launch a projectile
	//
	public void Fire() {

		// find direction (forward)
		//
		Vector3 p = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		
		// shoot projectile
		//
		//Instantiate(projectileFab, p, transform.rotation);

		//World.spawnBullet(this.gameObject);

		GameObject projectileGameObject = GameObject.Instantiate(projectileFab, p, transform.rotation) as GameObject;
		Projectile projectile = projectileGameObject.GetComponent<Projectile>();

		projectile.owner = owner;
	}
}
