using UnityEngine;
using System.Collections;

public class PlayerCam : MonoBehaviour {

	public Transform t;
	public GameObject target;
	public float damping = 1;
	private Vector3 offset;

	public float scrollSpeed = 1.0f;
	public float scrollArea = 20.0f;
	public float dragSpeed = 5.0f;

	// Update is called once per frame
	void Update () 
	{
		float mousePosX = Input.mousePosition.x; 
		float mousePosY = Input.mousePosition.y; 
		int scrollDistance = 5; 
		float scrollSpeed = 70;

		float d = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 100.0f;
		GameObject cam = GameObject.Find("Main Camera");
		cam.transform.Translate (transform.forward * d);

		
		if (mousePosX < scrollDistance) 
		{ 
			transform.Translate(Vector3.right * -scrollSpeed * Time.deltaTime); 
		} 
		
		if (mousePosX >= Screen.width - scrollDistance) 
		{ 
			transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime); 
		}
		
		if (mousePosY < scrollDistance) 
		{ 
			transform.Translate(transform.forward * -scrollSpeed * Time.deltaTime); 
		} 
		
		if (mousePosY >= Screen.height - scrollDistance) 
		{ 
			transform.Translate(transform.forward * scrollSpeed * Time.deltaTime); 
		}

	}
}
