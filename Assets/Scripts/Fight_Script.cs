using UnityEngine;
using System.Collections;

public class Fight_Script : MonoBehaviour 
{
	public float Speed = 10f;
	bool fade = false;
	
	
	public AudioClip FightSound;
	
	void Start () 
	{
		
	}
	
	public void DoFight()
	{
		fade = true;
		Color temp = this.gameObject.renderer.material.color;
		temp.a = 1f;

		this.gameObject.renderer.material.color = temp;
		this.gameObject.audio.PlayOneShot(FightSound);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (fade)
		{
			
			
			Color temp = this.gameObject.renderer.material.color;
			temp.a -= Speed * Time.deltaTime;

			this.gameObject.renderer.material.color = temp;
			
			if (temp.a <= 0)
			{
				fade = false;
				this.gameObject.SetActive(false);
			}
		}
	}
}
