using UnityEngine;
using System.Collections;

public class ScrollingBanner : MonoBehaviour 
{
	public float Speed;
	float timeElapsed = 0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		timeElapsed += Time.deltaTime * Speed;
		this.gameObject.renderer.material.SetTextureOffset("_MainTex", new Vector2(timeElapsed, 0));
	}
}
