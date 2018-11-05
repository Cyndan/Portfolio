using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkeletonDialogue : MonoBehaviour 
{
	// Use this for initialization
	// Grab the textbox so it can be edited. Allow this script to be edited by others.
	// Grab the little sound thing for menus. 
	// Set a delay between key pushes.

	// You can use "PlayerControl.instance.blackout.enabled/disabled to make
	// the screen go black, save textboxes and talksprites.

	public Text textbox;
	public static SkeletonDialogue instance;
	public AudioSource blip;
	public float delay = 0.25f;

	private int stage = 0;
	private bool start = false;
	private bool canGo = true;

	private bool renderSelf;
	private bool renderPlayer;

	void Start()
	{
		instance = this;
	}

	void Update()
	{
		if (PlayerControl.instance.talking == false) 
		{
			renderPlayer = false;
			renderSelf = false;
		}

		if (start == true) 
		{
			PlayerControl.instance.canAct = false;

			Dialogue ();
		}

		if (canGo == false) 
		{
			StartCoroutine (Proceed ());
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log ("Collision!");
		if (other.gameObject.tag == "Interact") 
		{
			PlayerControl.instance.canAct = false;

			start = true;
		}
	}

	IEnumerator Proceed ()
	{
		if (canGo == false) 
		{
			yield return new WaitForSeconds (delay);
			canGo = true;
		}
	}


	void Dialogue ()
	{
		//Retrieve player input, but only when this function is being called.
		bool interactButton = Input.GetButtonDown ("Interact");
		float moveHorizontal = Input.GetAxisRaw ("Horizontal");
		float moveVertical = Input.GetAxisRaw ("Vertical");
		PlayerControl.instance.talking = true;
		PlayerControl.instance.canAct = false;

		//This function operates by proceeding to the following "stage" each time
		//the interact button is pressed. Movement buttons are assigned to each
		//dialogue option. "ButtonDown" prevents text from advancing every frame,
		//and renderNPC/Player shows or hides the detailed sprites.

		// This blurb controls the talksprites; in this example, when renderPlayer
		// is true, the CameraControl script displays the player talk sprite.

		if (renderPlayer == true) 
		{
			CameraControl.instance.playerTalk = true;
		} else 
		{
			CameraControl.instance.playerTalk = false;
		}
		if (renderSelf == true) 
		{
			CameraControl.instance.meulinTalk = true;
		} else 
		{
			CameraControl.instance.meulinTalk = false;
		}

		if (stage == 0) {
			//renderSelf is used to place the NPC's sprite on screen.
			//renderPlayer does the same for the player.
			renderSelf = true;
			textbox.text = "";
			if (interactButton == true && canGo == true) {
				//canGo is what allows us to prevent the player from flipping through dialogue instantly.
				//blip.Play makes a small little sound.
				//stage is a variable that sets a point in the cutscene to display on screen.
				canGo = false;
				blip.Play ();
				stage = 1;
				renderSelf = false;
			}

		}

			if (stage == 1) 
			{
				textbox.text = "^: \r\nv: \r\n<: \r\n>: ";
				// "\r\n" is code for a linebreak mid-string.
				// This chunk is used when players are given a choice.

				if (moveVertical > 0) 
				{
					canGo = false;
					blip.Play ();
					stage = 100;
				}
				if (moveVertical < 0) 
				{
					canGo = false;
					blip.Play ();
					stage = 200;
				}
				if (moveHorizontal < 0) 
				{
					canGo = false;
					blip.Play ();
					stage = 300;
				}
				if (moveHorizontal > 0) 
				{
					canGo = false;
					blip.Play ();
					stage = 400;
				}
			}

			if (stage == 100) 
			{
				renderPlayer = true;
				textbox.text = "";
				if (interactButton == true && canGo == true) 
				{
					canGo = false;
					blip.Play ();
					renderPlayer = false;
					stage = 101;
				}
			}

			if (stage == 101) 
			{
				renderSelf = true;
				textbox.text = "This is dialogue that displays when the NPC finishes talking.";
				if (interactButton == true  && canGo == true) 
				{
					canGo = false; 
					blip.Play ();
					stage = 102;
					renderSelf = false;
					PlayerControl.instance.talking = false;
					PlayerControl.instance.canAct = true;
					start = false;
				}
			}

			if (stage == 102) 
			{
				renderSelf = true;
				textbox.text = "This is dialogue that displays if the NPC is talked to again.";
				if (interactButton == true  && canGo == true) 
				{
					canGo = false; 
					blip.Play ();
					PlayerControl.instance.talking = false;
					PlayerControl.instance.canAct = true;
					start = false;
					renderSelf = false;
				}
			}

		}


}
