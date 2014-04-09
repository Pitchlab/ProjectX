using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	private Transform 	t;
	public 	int 		speed = 100;
	public  int         maxUpdates = 100; 

	private int			updates = 0;

	public Player owner;

	// Use this for initialization
	void Start () {
	
		// supposedly helps caching
		t = transform;

		gameObject.tag = "Projectile";
	}

	// Update is called once per frame
	void Update () {

		updates++;
	
		// move up
		// 
		t.Translate (Vector3.forward * speed * Time.deltaTime);

		if (updates > maxUpdates) {
			DestroyObject(this.gameObject);
		}
	}

	void OnTriggerEnter(Collider col)
	{

		if (col.tag != "Detector")
		{
			Debug.Log (col.tag);
			Instantiate(Resources.Load("Bang"), t.position, t.rotation); 
			DestroyObject(this.gameObject);
		}
	}
}
