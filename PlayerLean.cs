using UnityEngine;
using System.Collections;

public class PlayerLean : MonoBehaviour 
{

	private bool leanLeft = false;
	private bool leanRight = false;
	private bool peeking = false;
	private Vector3 trnslate;
	public PlayerMove moveScript;
	public CameraLook cameraPlayer;
	public CameraLook cameraMain;

	public float leanLength = 20;
	public float peekHeight = 0.6f;


	void Update () 
	{
		if (moveScript.paused == false)
		{
			if(Input.GetKeyDown(KeyCode.Q)) 
			{
				Left();
			}

			if(Input.GetKeyDown(KeyCode.E)) 
			{
				Right();
			}

			if(Input.GetKeyUp(KeyCode.Q)) 
			{
				Back();
			}

			if(Input.GetKeyUp(KeyCode.E)) 
			{
				Back();
			}

			if(Input.GetKeyDown(KeyCode.Space)) 
			{
				Up();
			}

			if(Input.GetKeyUp(KeyCode.Space)) 
			{
				Back();
			}
		}
	}

	void Left() 
	{

		if(leanLeft == false) 
		{
			transform.rotation = transform.rotation * Quaternion.Euler(0, 0 ,leanLength);
			leanLeft = true;
			moveScript.isLeaning = true;
			cameraPlayer.isLeaning = true;
			cameraMain.isLeaning = true;
			
		}

	}

	void Right() 
	{

		if(leanRight == false) 
		{
			transform.rotation = transform.rotation * Quaternion.Euler(0,0,-leanLength);
			leanRight = true;
			moveScript.isLeaning = true;
			cameraPlayer.isLeaning = true;
			cameraMain.isLeaning = true;
		}

	}

	void Up()
	{
		if (peeking == false) 
		{
			trnslate = new Vector3 (0,peekHeight,0);
			transform.Translate (trnslate);
			GetComponent<Rigidbody> ().useGravity = false;
			peeking = true;
			moveScript.isLeaning = true;
			cameraPlayer.isLeaning = true;
			cameraMain.isLeaning = true;
		}
	}

	void Back() 
	{
		if (leanLeft == true) 
		{
			transform.rotation = transform.rotation * Quaternion.Euler (0, 0, -leanLength);
			leanLeft = false;
		}
		if (leanRight == true) 
		{
			transform.rotation = transform.rotation * Quaternion.Euler (0, 0, leanLength);
			leanRight = false;
		}
		if (peeking == true) 
		{
			trnslate = new Vector3 (0,-peekHeight,0);
			transform.Translate (trnslate);
			GetComponent<Rigidbody> ().useGravity = true;
			peeking = false;
		}
		//transform.rotation = Quaternion.Euler(transform.rotation.x,transform.rotation.y,0);
		moveScript.isLeaning = false;
		cameraPlayer.isLeaning = false;
		cameraMain.isLeaning = false;
	}
}
