using UnityEngine;
using System.Collections;

public class FianlWinner : MonoBehaviour 
{
	public Material POne;
	public Material PTwo;
	
	// Use this for initialization
	void Start () {
	
	}
	
	public void ShowWinnerText(int player)
	{
		if (player == 0)
		{
			this.gameObject.renderer.material = POne;
		}
		else if (player == 1)
		{
			this.gameObject.renderer.material = PTwo;
		}
		else
		{
			//There are only two players!?!	
		}
	}
}
