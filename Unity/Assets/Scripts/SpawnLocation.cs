using UnityEngine;
using System.Collections;

public class SpawnLocation : MonoBehaviour 
{
	
	public  float delay = 1.0f;
	private float timer = 0.0f;

	public int faction = 0;

	// Use this for initialization
	void Start () 
	{
		spawn ();
	}
	
	// Update is called once per frame
	void Update () 
	{

		//return;
		if (timer > 0) {
			timer -= Time.deltaTime;
		}
		else {
			timer = delay + Random.Range(1.5f, 3.0f);
			spawn ();
		}
	}

	// spawn a ship 
	// TODO add arguments
	//
	public void spawn() 
	{
		GameObject goEnemy = new GameObject();
		Actor a = goEnemy.AddComponent<Actor>();

		a.init();
		a.setFaction(faction);

		goEnemy.transform.Translate(gameObject.transform.position);

		for (int i=0; i<4; i++)
		{
			a.addWaypoint(GameObject.Find("s" + faction.ToString() + i.ToString()).transform.position);
		}
	}
}
