using UnityEngine;
using System.Collections;

public class Head_Script : MonoBehaviour 
{
	public Vector3 Gravity = new Vector3(0f, -10f, 0f);
	private Vector3 Impulse;
	
	bool DoStuff = true;
	private Transform FloorTransform;
	
	public GameObject ABunchOfBlood;
	private GameObject bloodToPause;
	private GameObject HeadImage;
	
	public AudioClip HeadSplat;
	
	// Use this for initialization
	void Start () 
	{
		HeadImage = this.gameObject.transform.FindChild("PlayerHead").gameObject;
		FloorTransform = GameObject.Find("Floor").transform;
	}
	
	// Update is called once per framew
	void Update () 
	{
		if (!DoStuff)
		{
			return;	
		}
		//rotate
		HeadImage.transform.RotateAround(HeadImage.transform.position, HeadImage.transform.forward, Time.deltaTime * 900f);
		//move
		this.gameObject.transform.position += Impulse * Time.deltaTime;
		
		if (this.gameObject.transform.position.y < FloorTransform.position.y)
		{
			if (Mathf.Abs(Impulse.y) <= 1f)
			{
				DoStuff = false;
				Vector3 someWittyVariableName = this.gameObject.transform.position;
				someWittyVariableName.y = FloorTransform.position.y;
				this.gameObject.transform.position = someWittyVariableName;
			}
			else
			{
				Impulse.y *= -1f;	
				this.gameObject.transform.position += Impulse * Time.deltaTime;
				Impulse.y *= .4f;	
				this.gameObject.audio.PlayOneShot(HeadSplat, Mathf.Clamp(Mathf.Abs(Impulse.y) / 15f, .1f, 1f));
			}
		}
		//check if hit ground
		Impulse += Gravity * Time.deltaTime;
	}
	
	public void DoCoolStuff(Vector3 pos, float scale)
	{
		Vector3 sT = this.gameObject.transform.localScale;
		sT.x *= scale;
		//this.gameObject.transform.localScale = sT;
		
		
		//determine rotation speed, for now constant
		
		Impulse = new Vector3(Random.Range(-2f, 2f), 50f, 0f);
		
		this.gameObject.transform.position = pos;
		Vector3 tPos = pos;
		tPos.y -= 2f;
		bloodToPause = (GameObject)Instantiate(ABunchOfBlood, tPos, this.gameObject.transform.rotation);
		
		DoStuff = true;
		
		StartCoroutine(WaitToPause());
	}
	
	IEnumerator WaitToPause()
	{
		
		yield return new WaitForSeconds(.03f);
		bloodToPause.GetComponent<ParticleSystem>().Stop();
	}
}
