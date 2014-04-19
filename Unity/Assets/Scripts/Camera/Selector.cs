using UnityEngine;
using System.Collections;

public class Selector : MonoBehaviour 
{

	// select
	//
	public Texture2D selectionHighlight = null;
	public static Rect selection = new Rect();
	private Vector3 startClick = -Vector3.one;

	
	// Update is called once per frame
	void Update () 
	{
		checkCamera();
	}
	
	// selection
	//
	private void checkCamera()
	{
		if (Input.GetMouseButtonDown(0))
		{
			startClick = Input.mousePosition;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (selection.width < 0)
			{
				selection.x += selection.width;
				selection.width = -selection.width;
			}
			
			if (selection.height < 0)
			{
				selection.y += selection.height;
				selection.height = -selection.height;
			}
			
			startClick = -Vector3.one;
		}
		
		if (Input.GetMouseButton(0))
		{
			selection = new Rect(startClick.x, convertToScreenY(startClick.y), Input.mousePosition.x - startClick.x, convertToScreenY(Input.mousePosition.y) - convertToScreenY(startClick.y));
		}
	}
	
	public static float convertToScreenY(float y)
	{
		return Screen.height-y;
	}
	
	private void OnGUI()
	{		
		if (startClick != -Vector3.one)
		{
			GUI.color = new Color(1,1,1,0.5f);
			GUI.DrawTexture(selection, selectionHighlight);
		}
	}
}
