using UnityEngine;
using System.Collections;

public class RoundScript : MonoBehaviour 
{
	public float Speed = 10f;
	bool fade = false;
	Vector3 startPosition;
	// Use this for initialization
	public AudioClip[] RoundSound;
	
	void Start () 
	{
		startPosition = this.gameObject.transform.position;
	}
	
	public void DoRound(int round)
	{
		//StartCoroutine(WaitToFade());	
		fade = true;
		this.gameObject.transform.FindChild("Round").gameObject.renderer.material.SetTextureOffset("_MainTex", new Vector2((round-1) * .2f, 0));
		if ((round-1) < 5)
		{
			this.gameObject.audio.PlayOneShot(RoundSound[round-1]);
		}
		else
		{
			this.gameObject.audio.PlayOneShot(RoundSound[5]);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (fade)
		{
			Vector3 temp = this.gameObject.transform.position;
			temp.x += Speed * Time.deltaTime;

			this.gameObject.transform.position = temp;
			
			if (temp.x <= -80)
			{
				temp = startPosition;
				fade = false;
				this.gameObject.SetActive(false);
			}
			this.gameObject.transform.position = temp;
		}
	}
}


