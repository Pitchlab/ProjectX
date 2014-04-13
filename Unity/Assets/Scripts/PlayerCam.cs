using UnityEngine;
using System.Collections;

public class PlayerCam : MonoBehaviour {

	public GameObject target;
	public float damping = 1;
	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = target.transform.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate() {

		// smooth angle change 
		if (target != null)
		{
			float currentAngle = transform.eulerAngles.y;
			float desiredAngle = target.transform.eulerAngles.y;
			float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * damping);
			Quaternion rotation = Quaternion.Euler(0, angle, 0); //Quaternion.identity; //

			transform.position = target.transform.position - offset;// - (rotation * offset);
			//transform.LookAt(target.transform);
		}
	}
}
