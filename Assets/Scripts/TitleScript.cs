using UnityEngine;
using System.Collections;

public class TitleScript : MonoBehaviour 
{
	public GameObject load;
	Quaternion startRotation;
	float timeElapsed;
	// Use this for initialization
	void Start () 
	{
	 	timeElapsed = 0f;
		startRotation = load.transform.rotation;
		Screen.showCursor = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKey(KeyCode.F) && Input.GetKey(KeyCode.LeftArrow))
		{
			timeElapsed += Time.deltaTime;
			load.transform.RotateAround(load.transform.position, load.transform.forward, 360f * Time.deltaTime * 2f);
			
			if (timeElapsed >= .5)
			{
				Application.LoadLevel("Game");
			}
		}
		else
		{
			timeElapsed = 0f;
			load.transform.rotation = startRotation;
		}
		if (Input.GetKey(KeyCode.H))
		{
			Application.LoadLevel("More");
		}
		
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}
