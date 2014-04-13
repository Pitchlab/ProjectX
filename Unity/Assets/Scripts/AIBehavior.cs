using UnityEngine;
using System.Collections;

// This is the idle behavior.
//
public class AIBehavior 
{

	public AIController controller;

	[SerializeField]
	protected float priority = 100.0f;

	// constructor
	//
	public AIBehavior(AIController c) 
	{
		setController(c);
	}

	public void setController(AIController c)
	{
		controller = c;
	}

	// AI Behavior will need to have the following:
	//
	// - Priority setting to resolve conflicting behaviors
	// - A Heuristic: is this behavior executable now? 
	//
	// - Access to sensory systems 
	// - An executable control method that issues orders

	// always return basic number
	//
	public virtual float evaluatePriority()
	{
		return priority;
	}

	public virtual void execute()
	{
	}
}
