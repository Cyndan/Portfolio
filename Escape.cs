using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Escape : MonoBehaviour 
{
	// Use this for initialization
	// Grab the textbox so it can be edited. Allow this script to be edited by others.
	// Grab the little sound thing for menus. 
	// Set a delay between key pushes.

	// You can use "PlayerControl.instance.blackout.enabled/disabled to make
	// the screen go black, save textboxes and talksprites.

	public Text textbox;
	public static Escape instance;
	public AudioSource blip;
	public AudioSource clang;
	public AudioSource quake;
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
		if (PlayerControl.instance.hasDisguise == true) 
		{
			Destroy (gameObject);
		}

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
		if (other.gameObject.tag == "Player") 
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
			CameraControl.instance.guardTalk = true;
		} else 
		{
			CameraControl.instance.guardTalk = false;
		}

		if (stage == 0) 
		{
			//renderSelf is used to place the NPC's sprite on screen.
			//renderPlayer does the same for the player.
			renderSelf = true;
			textbox.text = "There he is! Sieze him!";
			if (interactButton == true && canGo == true) 
			{
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
			//renderSelf is used to place the NPC's sprite on screen.
			//renderPlayer does the same for the player.
			renderPlayer = true;
			textbox.text = "Oh, for the love of nip...!";
			if (interactButton == true && canGo == true) 
			{
				//canGo is what allows us to prevent the player from flipping through dialogue instantly.
				//blip.Play makes a small little sound.
				//stage is a variable that sets a point in the cutscene to display on screen.
				canGo = false;
				blip.Play ();
				stage = 2;
				renderPlayer = false;
			}

		}

		if (stage == 2) 
		{ 
			textbox.text = "^: [Fight!]\r\nv: [<b>Wind...</b>]";
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

		}

		if (stage == 100) 
		{
			renderPlayer = true;
			textbox.text = "No more running away! I'll kill you all here! Just you fracking watch me!";
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
			textbox.text = "^: [Rely on your sword training!]\r\nv: [<b>Fire...</b>]";
			// "\r\n" is code for a linebreak mid-string.
			// This chunk is used when players are given a choice.

			if (moveVertical > 0) 
			{
				canGo = false;
				blip.Play ();
				clang.Play ();
				stage = 300;
			}
			if (moveVertical < 0) 
			{
				canGo = false;
				blip.Play ();
				stage = 400;
			}

		}

		if (stage == 300) 
		{
			//renderPlayer = true;
			PlayerControl.instance.blackout.enabled = true;
			textbox.text = "[Tomas lunges with his sword, driving it into the heart of the first soldier that sets upon him...but five soldiers are four too many. They pile upon the unarmored feline, and kill him swiftly to their song...]";
			if (interactButton == true && canGo == true) 
			{
				canGo = false;
				blip.Play ();
				renderPlayer = false;
				stage = 150;
			}
		}

		if (stage == 400) 
		{
			renderPlayer = true;
			textbox.text = "<i>Hellsong Firestorm!</i>";
			if (interactButton == true && canGo == true) 
			{
				canGo = false;
				blip.Play ();
				renderPlayer = false;
				stage = 401;
			}
		}

		if (stage == 401) 
		{
			//renderPlayer = true;
			textbox.text = "[Like a total nerd, Tomas shouts the name of his attack as he swathes his entire form in fire. He steps forward, the moist dock refusing to ignite. The soldiers in front of him take a step back...]";
			if (interactButton == true && canGo == true) 
			{
				canGo = false;
				blip.Play ();
				renderPlayer = false;
				stage = 402;
			}
		}

		if (stage == 402) 
		{
			//renderPlayer = true;
			textbox.text = "[With a lunge, his firey blade cuts through their armor with ease. One by one they fall, as if toys for the cat and nothing more. As they lay smoldering, Tomas sheathes his sword and the fire extinguishes itself.]";
			if (interactButton == true && canGo == true) 
			{
				canGo = false;
				blip.Play ();
				renderPlayer = false;
				stage = 403;
			}
		}

		if (stage == 403) 
		{
			//renderPlayer = true;
			textbox.text = "[A door blocks Tomas' path. Alas, will this be the end of his journey?!]";
			if (interactButton == true && canGo == true) 
			{
				canGo = false;
				blip.Play ();
				renderPlayer = false;
				stage = 404;
			}
		}

		if (stage == 404) 
		{ 
			textbox.text = "^: [Kick it the frack down.]\r\nv: [<b>FIRE! FIRE FIRE FIRE!</b>]";
			// "\r\n" is code for a linebreak mid-string.
			// This chunk is used when players are given a choice.

			if (moveVertical > 0) 
			{
				canGo = false;
				blip.Play ();
				clang.Play ();
				stage = 420;
			}
			if (moveVertical < 0) 
			{
				canGo = false;
				blip.Play ();
				stage = 440;
			}

		}

		if (stage == 420) 
		{
			//renderPlayer = true;
			textbox.text = "[A single kick. That's all it takes for the door to come crashing down.]";
			if (interactButton == true && canGo == true) 
			{
				canGo = false;
				blip.Play ();
				renderPlayer = false;
				stage = 203;
			}
		}

		if (stage == 440) 
		{
			renderPlayer = true;
			textbox.text = "<b>INFERNAL JUDGEMENT!</b>";
			if (interactButton == true && canGo == true) 
			{
				canGo = false;
				blip.Play ();
				renderPlayer = false;
				stage = 441;
			}
		}
		if (stage == 441) 
		{
			//renderPlayer = true;
			PlayerControl.instance.blackout.enabled = true;
			textbox.text = "[Tomas raises his arms, and a leviathan, fiery vortex swirls above his head. With a chuckle befitting a villain, Tomas throws his arms down, and the red tornado consumes the building...]";
			if (interactButton == true && canGo == true) 
			{
				canGo = false;
				blip.Play ();
				renderPlayer = false;
				stage = 442;
			}
		}

		if (stage == 442) 
		{
			//renderPlayer = true;
			textbox.text = "...and the escape pods within...and Tomas. Good going, kid.";
			if (interactButton == true && canGo == true) 
			{
				canGo = false;
				blip.Play ();
				renderPlayer = false;
				stage = 150;
			}
		}

		if (stage == 200) 
		{
			renderPlayer = true;
			textbox.text = "<i>Expeditious Dash!</i>";
			if (interactButton == true && canGo == true) 
			{
				canGo = false;
				blip.Play ();
				renderPlayer = false;
				stage = 201;
			}
		}

		if (stage == 201) 
		{
			//renderPlayer = true;
			textbox.text = "[Like a true dork, Tomas shouts the name of his spell as he dashes forward, either arm splayed out straight behind him. The soldiers raise their swords...]";
			if (interactButton == true && canGo == true) 
			{
				canGo = false;
				blip.Play ();
				renderPlayer = false;
				stage = 202;
			}
		}

		if (stage == 202) 
		{
			renderPlayer = true;
			textbox.text = "[Wind rushes behind Tomas, and his body fades, become ethereal as he phases through the guards at a supernatural speed. The gales rattle the soldier's helmets, stunning them briefly as Tomas phases through the approaching door.]";
			if (interactButton == true && canGo == true) 
			{
				canGo = false;
				blip.Play ();
				renderPlayer = false;
				stage = 203;
			}
		}

		if (stage == 203) 
		{
			//renderPlayer = true;
			PlayerControl.instance.blackout.enabled = true;
			textbox.text = "[Now inside, the escape pods are easy pickings. Tomas throws a switch, clambers aboard, and lowers the hatch. The glass seals him away as he takes off into the blue abyss.]";
			if (interactButton == true && canGo == true) 
			{
				canGo = false;
				blip.Play ();
				renderPlayer = false;
				stage = 105;
			}
		}

		if (stage == 105) 
		{
			//renderSelf is used to place the NPC's sprite on screen.
			//renderPlayer does the same for the player.
			//renderGuard = true;
			textbox.text = "^: [Never look back]\r\nv: [<b>Typhoon</b>]";
			if (moveVertical > 0) 
			{
				canGo = false;
				blip.Play ();
				stage = 110;
			}
			if (moveVertical < 0) 
			{
				canGo = false;
				blip.Play ();
				quake.Play ();
				stage = 120;
			}

		}

		if (stage == 110) 
		{
			//renderSelf is used to place the NPC's sprite on screen.
			//renderPlayer does the same for the player.
			PlayerControl.instance.blackout.enabled = true;
			//renderPlayer = true;
			textbox.text = "I never did look back as I left. But I survived. The story from there, however, is long forgotten, but perhaps to be continued...";
			if (interactButton == true && canGo == true) 
			{
				//canGo is what allows us to prevent the player from flipping through dialogue instantly.
				//blip.Play makes a small little sound.
				//stage is a variable that sets a point in the cutscene to display on screen.
				canGo = false;
				blip.Play ();
				stage = 150;
				renderPlayer = false;
			}

		}

		if (stage == 120) 
		{
			//renderSelf is used to place the NPC's sprite on screen.
			//renderPlayer does the same for the player.
			//renderPlayer = true;
			textbox.text = "[With a single, effortless thought, a piercing crack rings throughout the ocean. As Tomas looks back, the glass holding back the water from Underfell caves in on itself. Water pours in, crushing its inhabitants.";
			if (interactButton == true && canGo == true) {
				//canGo is what allows us to prevent the player from flipping through dialogue instantly.
				//blip.Play makes a small little sound.
				//stage is a variable that sets a point in the cutscene to display on screen.
				canGo = false;
				blip.Play ();
				stage = 121;
				renderPlayer = false;
			}
		}

		if (stage == 121) 
		{
			//renderSelf is used to place the NPC's sprite on screen.
			//renderPlayer does the same for the player.
			PlayerControl.instance.blackout.enabled = true;
			renderPlayer = true;
			textbox.text = "[As Tomas turns around, he hears a quieter crack. He looks up, and is met with a dazzling spiderweb of fissures in the glass of his pod.]";
			if (interactButton == true && canGo == true) 
			{
				//canGo is what allows us to prevent the player from flipping through dialogue instantly.
				//blip.Play makes a small little sound.
				//stage is a variable that sets a point in the cutscene to display on screen.
				canGo = false;
				blip.Play ();
				stage = 150;
				renderPlayer = false;
			}

		}

		if (stage == 150) 
		{
			//renderSelf is used to place the NPC's sprite on screen.
			//renderPlayer does the same for the player.
			//renderPlayer = true;
			textbox.text = "<b>\r\n         G A M E   O V E R</b>";
			if (interactButton == true && canGo == true) 
			{
				//canGo is what allows us to prevent the player from flipping through dialogue instantly.
				//blip.Play makes a small little sound.
				//stage is a variable that sets a point in the cutscene to display on screen.
				canGo = false;
				blip.Play ();
				stage = 150;
				renderPlayer = false;
			}

		}





		if (stage == 10001) 
		{
			renderSelf = true;
			textbox.text = "This is dialogue that displays when the NPC finishes talking.";
			if (interactButton == true  && canGo == true) 
			{
				canGo = false; 
				blip.Play ();
				stage = 10002;
				renderSelf = false;
				PlayerControl.instance.talking = false;
				PlayerControl.instance.canAct = true;
				start = false;
			}
		}

		if (stage == 10002) 
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
