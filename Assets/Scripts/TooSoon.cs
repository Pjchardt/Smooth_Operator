using UnityEngine;
using System.Collections;

public class TooSoon : MonoBehaviour {
	
	bool doFlash;
	float timeElapsed = 0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timeElapsed += Time.deltaTime;
		
		if (timeElapsed > 3)
		{
			this.gameObject.SetActive(false);
		}
	}
	
	public void Flash()
	{
		doFlash = true;
		timeElapsed = 0f;
	}
}
