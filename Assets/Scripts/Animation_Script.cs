using UnityEngine;
using System.Collections;

public class Animation_Script : MonoBehaviour 
{
	public int numFrames = 1;
	float offset;
	private int currentFrame;
	// Use this for initialization
	void Start () 
	{
		offset = 1f/(float)numFrames;
		currentFrame = 10;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public void NewFrame(int i)
	{
		if (currentFrame != i)
		{
			this.gameObject.renderer.material.SetTextureOffset("_MainTex", new Vector2(1f-(offset * (i+1)), 0));
			this.gameObject.GetComponent<ScaleTimer>().LetsDoIt();
			currentFrame = i;
		}
	}
}
