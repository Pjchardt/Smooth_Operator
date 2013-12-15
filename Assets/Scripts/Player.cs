using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public enum PlayerState {Nothing, Attacking, Defending, Faking, InputDelay, AttackHit, Killed, JumpingBack, JumpingBackBothAttacked};
	public PlayerState currentState;
	public int playerNum = 0;
	
	public Material Nothing;
	public Material Attacking;
	public Material Defending;
	public Material Faking;
	public Material InputDelay;
	public Material AttackHit;
	public Material Killed;
	public Material Dead;
	public Material JumpingBack;
	
	public float inputDelay = .45f;
	public float BlockDelay = .35f;
	public float stateTime = .2f;
	
	private float timeElapsed = 0f;
	private bool blocked = false;
	private bool faked = false;
	private bool attacked = false;
	private bool newRound = false;
	
	public AudioClip Block;
	public AudioClip Fake;
	public AudioClip HitSound;
	
	private Renderer rendererRef;
	
	public GameObject OtherPlayer;
	private Vector3 startPosition;
	private Vector3 theirStartPosition;
	private Vector3 jumpingBackBothPosition;
	
	public GameObject Head;
	private GameObject headHandle = null;
	
	public GameObject FlashQuad;
	
	int points;
	
	// Use this for initialization
	void Start () 
	{
		rendererRef = this.gameObject.transform.renderer;
		rendererRef.material = Nothing;
		
		startPosition = this.gameObject.transform.position;
		theirStartPosition = OtherPlayer.transform.position;
		
		blocked = false;
		faked = false;
		attacked = false;
		newRound = false;
		
		points = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (GameManager.GameOver || !GameManager.Ready)
		{
			if (!GameManager.GameOver && newRound)
			{
				QueryInputBeforeRound();
				//Debug.Log("Hit");
			}
			
			if (currentState == PlayerState.JumpingBack)
			{
				UpdateJumpingBack();	
			}
			return;	
		}
		
		switch(currentState)
		{
			case PlayerState.Nothing:
			QueryInput();
			break;
			case PlayerState.Attacking:
			UpdateAttack();
			break;
			case PlayerState.Defending:
			UpdateDefend();
			break;
			case PlayerState.Faking:
			UpdateFake();
			break;
			case PlayerState.InputDelay:
			UpdateDelay();
			break;
			case PlayerState.AttackHit:
			UpdateAttackHit();
			break;
			case PlayerState.Killed:
			UpdateKilled();
			break;
			case PlayerState.JumpingBack:
			UpdateJumpingBack();
			break;
			case PlayerState.JumpingBackBothAttacked:
			UpdateJumpingBackBothAttacked();
			break;
		}
		
	}
	
	private void QueryInput()
	{
		bool input = false;
		
		if (playerNum == 0)
		{
			if (Input.GetKeyDown(KeyCode.S) && !blocked)	
			{
				rendererRef.material = Defending;
				currentState = PlayerState.Defending;
				blocked = true;
				this.gameObject.audio.PlayOneShot(Block);
				input = true;
			}
			if (Input.GetKeyDown(KeyCode.D) && !faked)
			{
				rendererRef.material = Faking;
				currentState = PlayerState.Faking;
				this.gameObject.audio.PlayOneShot(Fake);
				faked = true;
				input = true;
			}
			if (Input.GetKeyDown(KeyCode.F) && !attacked)
			{
				rendererRef.material = Attacking;
				currentState = PlayerState.Attacking;
				this.gameObject.audio.PlayOneShot(Fake);
				attacked = true;
				input = true;
			}
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.RightArrow) && !blocked)	
			{
				rendererRef.material = Defending;
				currentState = PlayerState.Defending;
				this.gameObject.audio.PlayOneShot(Block);
				blocked = true;
				input = true;
			}
			if (Input.GetKeyDown(KeyCode.UpArrow) && !faked)
			{
				rendererRef.material = Faking;
				currentState = PlayerState.Faking;
				this.gameObject.audio.PlayOneShot(Fake);
				faked = true;
				input = true;
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow) && !attacked)
			{
				rendererRef.material = Attacking;
				currentState = PlayerState.Attacking;
				this.gameObject.audio.PlayOneShot(Fake);
				attacked = true;
				input = true;
			}
		}
		
		timeElapsed = 0f;
	}
		
	private void QueryInputBeforeRound()
	{
		bool input = false;
		
		if (playerNum == 0)
		{
			if (Input.GetKeyDown(KeyCode.S) && !blocked)	
			{
				input = true;
			}
			if (Input.GetKeyDown(KeyCode.D) && !faked)
			{
				input = true;
			}
			if (Input.GetKeyDown(KeyCode.F) && !attacked)
			{
				input = true;
			}
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.RightArrow))	
			{
				input = true;
			}
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				input = true;
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				input = true;
			}
		}
		
		if (input)
		{
			//pressed button before round, kill player
			this.Hit();
			FlashQuad.SetActive(true);
			FlashQuad.GetComponent<TooSoon>().Flash();
			GameObject.Find("Main Camera").GetComponent<GameManager>().StopAllCoroutines((playerNum+1)%2);
		}
	}
	
	private void UpdateAttack()
	{
		timeElapsed += Time.fixedDeltaTime;
		this.gameObject.transform.position = Vector3.Lerp(startPosition, theirStartPosition, timeElapsed * (1f/stateTime));
		
		//check ig player has collided with other player or passed through other player and resolve
		
		if (timeElapsed >= stateTime)
		{
			this.gameObject.transform.position = theirStartPosition;
			//Check if attack has landed
			PlayerState otherPlayerState = OtherPlayer.GetComponent<Player>().currentState;
			
			if (otherPlayerState != PlayerState.Attacking && otherPlayerState != PlayerState.Defending)
			{
				//hit
				//Debug.Log("Hit");
				rendererRef.material = AttackHit;	
				currentState = PlayerState.AttackHit;
				this.gameObject.audio.PlayOneShot(HitSound);
				GameObject.Find("Main Camera").GetComponent<GameManager>().BulletTime(playerNum);
				OtherPlayer.GetComponent<Player>().Hit();
				StartCoroutine(WaitToJumpBack());
				timeElapsed = 0f;
			}
			else
			{
				//this.gameObject.transform.position = startPosition;
				//StartInputDelay();
				rendererRef.material = JumpingBack;
				currentState = PlayerState.JumpingBack;
				timeElapsed = 0f;
			}
		}
	}
	
	void UpdateJumpingBack()
	{
		timeElapsed += Time.fixedDeltaTime;
		this.gameObject.transform.position = Vector3.Lerp(theirStartPosition, startPosition, timeElapsed * (1f/stateTime));
		Vector3 tP = this.gameObject.transform.position;
		tP.y = startPosition.y;
		tP.y += Mathf.Sin(Mathf.PI * (timeElapsed  * (1f/stateTime))) * 2f;
		this.gameObject.transform.position = tP;
		
		//check ig player has collided with other player or passed through other player and resolve
		
		if (timeElapsed >= stateTime)
		{
			this.gameObject.transform.position = startPosition;
			//Check if attack has landed
			if (GameManager.GameOver)
			{
				currentState = PlayerState.Nothing;
				rendererRef.material = Nothing;
			}
			else
			{
				StartInputDelay();
			}
			
		}
	}
	
	void UpdateJumpingBackBothAttacked()
	{
		timeElapsed += Time.deltaTime;
		this.gameObject.transform.position = Vector3.Lerp(jumpingBackBothPosition, startPosition, timeElapsed * (1f/stateTime));
		Vector3 tP = this.gameObject.transform.position;
		tP.y = startPosition.y;
		tP.y += Mathf.Sin(Mathf.PI * (timeElapsed  * (1f/stateTime))) * 2f;
		this.gameObject.transform.position = tP;
		
		//check ig player has collided with other player or passed through other player and resolve
		
		if (timeElapsed >= stateTime)
		{
			this.gameObject.transform.position = startPosition;
			//Check if attack has landed
			rendererRef.material = Dead;
			currentState = PlayerState.Killed;	
			//need to call game over on manager but only once!
			
			
		}
	}
	
	public void StartJumpingBackBothAttacked()
	{
		this.gameObject.audio.PlayOneShot(HitSound);
		jumpingBackBothPosition = this.gameObject.transform.position;
		currentState = PlayerState.JumpingBackBothAttacked;
		timeElapsed = 0f;
	}
	
	private void UpdateDefend()
	{
		timeElapsed += Time.fixedDeltaTime;
		
		if (timeElapsed >= BlockDelay)
		{
			StartInputDelay();
		}
	}
	
	private void UpdateFake()
	{
		timeElapsed += Time.fixedDeltaTime;
		
		Vector3 target = (theirStartPosition - startPosition);
		target = startPosition + target.normalized * (target.magnitude * .25f);
		
		if (timeElapsed < stateTime * .5f)
		{
			this.gameObject.transform.position = Vector3.Lerp(startPosition, target, timeElapsed * (1f/(stateTime * .5f)));
		}
		else
		{
			this.gameObject.transform.position = Vector3.Lerp(target, startPosition, timeElapsed * (1f/(stateTime * .5f)));
		}
		
		timeElapsed += Time.fixedDeltaTime;
		
		if (timeElapsed >= stateTime)
		{
			StartInputDelay();
		}
	}
	
	private void UpdateDelay()
	{
		timeElapsed += Time.fixedDeltaTime;
		
		if (timeElapsed >= inputDelay)
		{
			rendererRef.material = Nothing;	
			timeElapsed = 0f;
			currentState = PlayerState.Nothing;
		}
	}
	
	private void UpdateAttackHit()
	{
	}
	
	private void UpdateKilled()
	{
	}
	
	public void Hit()
	{
		rendererRef.material = Killed;
		currentState = PlayerState.Killed;	
		
		int scale = 1;
		//create head
		if (playerNum == 1)
		{
			scale = -1;
		}
		headHandle = (GameObject)Instantiate(Head);
		headHandle.GetComponent<Head_Script>().DoCoolStuff(this.gameObject.transform.FindChild("HeadTransform").position, scale); 
		
		StartCoroutine(WaitToDie());
	}
	
	private void StartInputDelay()
	{
		rendererRef.material = InputDelay;	
		currentState = PlayerState.InputDelay;
		timeElapsed = 0f;
	}
	
	IEnumerator WaitToDie()
	{
		yield return new WaitForSeconds(.3f);
		
		rendererRef.material = Dead;
	}
	
	IEnumerator WaitToJumpBack()
	{
		yield return new WaitForSeconds(.3f);
		
		rendererRef.material = JumpingBack;
		currentState = PlayerState.JumpingBack;
		timeElapsed = 0f;
	}
	
	public int UpdatePoints()
	{
		points++;
		
		GameObject temp = GameObject.Find("P" + (playerNum + 1).ToString() + "_Points");
		GameObject stopReadingMyCode = temp.transform.FindChild("Point" + points.ToString()).gameObject;
		stopReadingMyCode.SetActive(true); //just kiding, you can read my code. but it is a mess!
		
		//check if player won three games
		if (points >= 3)
		{
			GameObject.Find("Main Camera").GetComponent<GameManager>().Finished(playerNum);
		}
		
		return points;
	}
	
	public void ResetPlayer()
	{
		rendererRef.material = Nothing;
		currentState = PlayerState.Nothing;
		newRound = false;
		if (headHandle != null)
		{
			Destroy(headHandle);
			headHandle = null;
		}
		
		blocked = false;
		faked = false;
		attacked = false;
	}
	
	public void NewRound()
	{
		newRound = true;	
	}
}
