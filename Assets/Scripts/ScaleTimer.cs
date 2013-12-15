using UnityEngine;
using System.Collections;

public class ScaleTimer : MonoBehaviour 
{
	public GameObject soundClip;
	bool doIt = false;
	float timeElapsed;
	Vector3 startScale;
	// Use this for initialization
	void Start () 
	{
		startScale = this.gameObject.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!doIt)
		{
			return;
		}
		timeElapsed += Time.deltaTime;
		Vector3 tP = startScale;
		tP.y += Mathf.Sin(Mathf.PI * (timeElapsed  * 2f)) * .7f;
		tP.x += Mathf.Sin(Mathf.PI * (timeElapsed  * 2f)) * .7f;
		this.gameObject.transform.localScale = tP;
		if (timeElapsed >= .5)
		{
			doIt = false;	
			this.gameObject.transform.localScale = startScale;
		}
	}
	
	public void LetsDoIt()
	{
		//this.audio.PlayOneShot(soundClip);
		timeElapsed = 0f;
		doIt = true;	
	}
}
