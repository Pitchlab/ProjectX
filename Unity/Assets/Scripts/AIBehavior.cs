using UnityEngine;
using System.Collections;

public class AIBehavior : Object {

	public AIController Controller;

	// constructor
	//
	public AIBehavior(AIController c) 
	{
		Controller = c;
	}

	// AI Behavior will need to have the following:
	//
	// - Priority setting to resolve conflicting behaviors
	// - A Heuristic: is this behavior executable now? 
	//
	// - Access to sensory systems 
	// - An executable control method that issues orders
}
