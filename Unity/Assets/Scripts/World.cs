using UnityEngine;

// --------------------------------------------------------------------
// WORLD
// --------------------------------------------------------------------
// This class governs the Game World and acts as a factory for all of
// the items created. 
//
public class World : MonoSingleton<World>
{


	// awake code in here
	//
	protected override void Init() 
	{
			
	}

	// spawn the player 
	//

	// spawn a bullet
	//
	public static GameObject spawnBullet()
	{
		return null;
	}

	// spawn a specific actor
	//
	static Actor spawnActor(string type) 
	{
		return null;
	}
}