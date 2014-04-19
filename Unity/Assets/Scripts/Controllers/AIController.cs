using UnityEngine;
using System.Collections.Generic;

// TODO
//
// Create Orders in the system to follow
// The AIController can issue orders 
// How they are followed is a matter of the controller
// ?- Do I need to split this up? 
// ?- Move order execution to the regular controller so it is inherited? 
//
public class AIController : Controller {

	public float bearing;

	[SerializeField] 
	protected List<AIBehavior> behaviors = new List<AIBehavior>();
	
	protected AIBehavior selectedBehavior = null;

	public string chosenBehavior = "";

	public override void init ()
	{
		if (isInitialized) return;
		base.init ();

		// add basic behaviors
		//
		AIBehaviorAttack a = new AIBehaviorAttack(this);
		behaviors.Add(a);
		behaviors.Add(new AIBehaviorMoveTo(this));

		isInitialized = true;
	}

	// update the controls and update trn, acc, alt.
	// 
	public override void updateControls() 
	{
		// figure out which behavior to use
		// TODO evaluate less than every update...
		// TODO use events?
		//
		selectedBehavior = null;
		chosenBehavior = "";
		float priority = 0.0f;

		foreach (AIBehavior b in behaviors)
		{
			if (b.evaluatePriority() > priority)
			{

				priority = b.evaluatePriority();
				selectedBehavior = b;
				chosenBehavior = b.GetType().ToString();
			}
		}

		// execute a behavior
		//
		if (selectedBehavior != null)
		{
			selectedBehavior.execute();
		}
		else
		{
			Debug.LogWarning("Warning: No valid AIBehavior!");
		}
	}
}
