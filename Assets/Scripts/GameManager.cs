using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	public static bool GameOver;
	public static bool Ready;
		
	public float GameTime;
	private float currentGameTime;
		
	public GameObject Timer;
	public GameObject FightImage;
	public GameObject RoundImage;
	int round;
	
	public GameObject Winner;
	public Material Player1Victory;
	public Material Player2Victory;
	public Material Tie;
	
	public GameObject Player1;
	public GameObject Player2;
	private Player Player1Player;
	private Player Player2Player;
	
	public GameObject FinalWinnerText;
	
	public GameObject TooMuchBloodEffect;
	
	public AudioClip WinSound;
	public AudioClip FailSound;
	
	public float timeScale = 2f;
	
	// Use this for initialization
	void Start () 
	{
		Time.timeScale = timeScale;
		round = 1;
		GameOver = false;
		Ready = false;
		currentGameTime = GameTime;
		StartCoroutine("WaitToStart");
		Player1Player = Player1.GetComponent<Player>();
		Player2Player = Player2.GetComponent<Player>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
		
		if (GameManager.GameOver || !GameManager.Ready)
		{
			return;	
		}
		
		currentGameTime -= Time.fixedDeltaTime;
		
		//update timer graphic
		int toInt = Mathf.CeilToInt(currentGameTime);
		Timer.GetComponent<Animation_Script>().NewFrame(toInt);
		
		//check if both players are attacking
		if (Player1Player.currentState == Player.PlayerState.Attacking && Player2Player.currentState == Player.PlayerState.Attacking)
		{
			//check if they are hitting each other 
			if (Player1.transform.position.x > Player2.transform.position.x)
			{
				StartCoroutine(DOUBLEBULLETTIME());
			}
		}
		
		if (currentGameTime <= 0)
		{
			//Gamer Over
			//pop off everyones head
			//then do game over
			this.gameObject.transform.FindChild("WinSoundObject").gameObject.audio.PlayOneShot(FailSound);
			Player1Player.Hit();
			Player2Player.Hit();
			StartGameOver(2);
		}
	}
	
	IEnumerator DelayAfterBothAttacked()
	{
		yield return new WaitForSeconds(1f);
		StartGameOver(2);
	}
	
	public void StartGameOver(int winner)
	{
		int Points = 0;
		if (winner == 0)
		{
			Winner.renderer.material = Player1Victory;
			Points = Player1.GetComponent<Player>().UpdatePoints();
			//play winner sound
			this.gameObject.transform.FindChild("WinSoundObject").gameObject.audio.PlayOneShot(WinSound);
		}
		else if (winner == 1)
		{
			Winner.renderer.material = Player2Victory;
			Points = Player2.GetComponent<Player>().UpdatePoints();
			//play winner sound
			this.gameObject.transform.FindChild("WinSoundObject").gameObject.audio.PlayOneShot(WinSound);
		}
		else if (winner == 2)
		{
			Winner.renderer.material = Tie;
			this.gameObject.transform.FindChild("WinSoundObject").gameObject.audio.PlayOneShot(FailSound);
		}
		this.audio.Stop();
		
		//if tie say tie and kill both players
		Winner.SetActive(true);
		GameOver = true;
		Ready = false;
		//show game over text
		
		if (Points < 3)
		{
			StartCoroutine(WaitToRestart());
		}
		//restart
	}
	
	IEnumerator WaitToRestart()
	{
		yield return new WaitForSeconds(4f);
		
		Winner.SetActive(false);
		GameOver = false;
		Ready = false;
		currentGameTime = GameTime;
		round++;
		timeScale *= 1.1f;
		if (timeScale > 3)
		{
			timeScale = 3f;
		}
		StartCoroutine("WaitToStart");
	}
	
	IEnumerator WaitToStart()
	{
		yield return new WaitForSeconds(1f);
		
		//show round
		
		//then wait variable time
		
		//then start with birds flying off
		//FightImage.SetActive(true);
		RoundImage.SetActive(true);
		RoundImage.GetComponent<RoundScript>().DoRound(round);
		
		Time.timeScale = timeScale;
		
		Player1.GetComponent<Player>().ResetPlayer();
		Player2.GetComponent<Player>().ResetPlayer();
		
		Player1Player.NewRound();
		Player2Player.NewRound();
		
		//call next routine
		StartCoroutine("WaitToFight");
		
	}
	
	public void BulletTime(int winner)
	{
		StartCoroutine(BulletTimeDelay(winner));
	}
	
	IEnumerator BulletTimeDelay(int winner)
	{
		//Debug.Log("BulletTime");
		Time.timeScale = .01f;
		
		yield return new WaitForSeconds(.01f);
		
		Time.timeScale = timeScale;
		
		StartGameOver(winner);
	}
	
	public void Finished(int winner)
	{
		//someone has won, do stuff and return to title screen, oh that means i need to make a title screen
		StartCoroutine(FinishedStuff(winner));
	}
	
	IEnumerator WaitToFight()
	{
		yield return new WaitForSeconds(1.8f);
		FightImage.SetActive(true);
		FightImage.GetComponent<Fight_Script>().DoFight();
		StartCoroutine("PlaySong");
		Ready = true;
		//Debug.Log(Time.timeScale);
	}
	
	IEnumerator PlaySong()
	{
		yield return new WaitForSeconds(.6f);
		this.audio.Play();
	}
	
	IEnumerator DOUBLEBULLETTIME()
	{
		GameObject temp = (GameObject)Instantiate(TooMuchBloodEffect);
		Vector3 stuffToUse = Player2.transform.position - Player1.transform.position;
		Vector3 tempPos = Player1.transform.position + (stuffToUse.magnitude * stuffToUse.normalized * .5f); 
		temp.transform.position = tempPos;
		Player1.transform.position = new Vector3(tempPos.x - 1f, tempPos.y, tempPos.z);;
		Player2.transform.position = new Vector3(tempPos.x + 1f, tempPos.y, tempPos.z);
		Player1Player.StartJumpingBackBothAttacked();
		Player2Player.StartJumpingBackBothAttacked();
		
		Time.timeScale = .01f;
		
		yield return new WaitForSeconds(.01f);
		
		Time.timeScale = timeScale;
		
		temp.GetComponent<ParticleSystem>().Stop();
		
		StartCoroutine(DelayAfterBothAttacked());
	}
	
	IEnumerator FinishedStuff(int winner)
	{
		yield return new WaitForSeconds(3f);
		Winner.SetActive(false);
		FinalWinnerText.SetActive(true);
		FinalWinnerText.GetComponent<FianlWinner>().ShowWinnerText(winner);
		StartCoroutine(WaitToLoadTitleScreen());
	}
	
	IEnumerator WaitToLoadTitleScreen()
	{
		yield return new WaitForSeconds(3f);
		
		Application.LoadLevel("Title");
	}
	
	public void StopAllCoroutines(int winner)
	{
		StopAllCoroutines();
		
		StartGameOver(winner);
	}
}
