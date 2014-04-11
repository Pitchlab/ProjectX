using UnityEngine;
using System.Collections;

public class SpawnLocation : MonoBehaviour {

	private GameObject enemy;

	public  float delay = 1.0f;
	private float timer = 0.0f;

	// Use this for initialization
	void Start () {
	
		spawn ();

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
			timer += delay;
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

		a.t.Translate (Random.Range(-50.0f, 50.0f), 0.0f, Random.Range(-50.0f, 50.0f)); 
		a.controller.setTurn(Random.value * 10.0f - 5.0f);
		a.controller.setAcc(Random.value * 10.0f);

		//GameObject.Instantiate(enemy, p, transform.rotation) as GameObject;
		//Actor a = goEnemy.GetComponent<Actor>();

		//Debug.Log ("Spawned Enemy");
	}
}
