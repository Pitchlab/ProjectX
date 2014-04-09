using UnityEngine;
using System.Collections;

public class Controller : Object {

	public Actor owner;

	public float trn = 0.0f;		// turning movement
	public float acc = 0.0f;		// acceleration
	public float alt = 0.0f;		// climbing 

	// Use this for initialization
	public Controller (Actor actor) 
	{
		owner = actor;
	}

	// update the controls and update trn, acc, alt.
	// 
	public virtual void updateControls() {

	}
}
