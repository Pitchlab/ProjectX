using UnityEngine;
using System.Collections;

public class SpawnLocation : MonoBehaviour {

	private GameObject enemy;

	public  float delay = 1.0f;
	private float timer = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

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
	public void spawn() {

		GameObject goEnemy = new GameObject();
		Actor a = goEnemy.AddComponent<Actor>();

		a.init();

		a.controller.trn = Random.value * 10.0f - 5.0f;
		a.controller.acc = Random.value * 10.0f;

		//GameObject.Instantiate(enemy, p, transform.rotation) as GameObject;
		//Actor a = goEnemy.GetComponent<Actor>();

		//Debug.Log ("Spawned Enemy");
	}
}
