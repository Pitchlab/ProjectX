using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	protected Transform 	t;
	public 	float 			speed 		= 400.0f;
	public  float      	 	lifeTime 	= 10.0f; 	// how long will it stay alive? 	
	private float			age 		= 0.0f;


	public Actor 			owner;

	// Use this for initialization
	void Start () {
	
		// supposedly helps caching
		//
		t = transform;

		gameObject.tag = "Projectile";
	}

	// Update is called once per frame
	void Update () 
	{
		checkAge();
		updateProjectile ();
	}

	protected virtual void checkAge()
	{
		age+=Time.deltaTime;
		
		if (age > lifeTime) 
		{
			die();
		}
	}

	protected virtual void updateProjectile()
	{
		move();
	}

	protected virtual void move()
	{
		t.Translate (Vector3.forward * speed * Time.deltaTime);
	}

	void OnTriggerEnter(Collider col)
	{
		if (owner == null) return;

		if (col.tag != "Detector")
		{
			if ((col.gameObject.GetComponent<Actor>()) is ITargetable)
			{
				Actor actor = col.gameObject.GetComponent<Actor>();
				if (actor != owner)
				{
					actor.damage(10.0f);

					die ();
				}
			}
		}
	}

	protected void die()
	{
		Instantiate(Resources.Load("Bang"), t.position, t.rotation); 
		DestroyObject(this.gameObject);
	}
}
