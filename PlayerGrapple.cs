using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrapple : MonoBehaviour 
{
	//Set states for whether we are mid-grapple or mid-aim. Also the force to apply for our swing and max grapple distance.
	[SerializeField] private bool isGrappling = false;
	//private bool isAiming = false;
	public float grappleLength = 10.0f;
	public float grappleStrength = 8.0f;
	public float dampen = 3.0f;

	//Prep our input names.
	private float rsVert;
	private float rsHoriz;
	private float xboxLT;
	private float xboxRT;
	private bool xboxB;

	//We deal with a whole bunch of angle stuff here.
	//private float aimAngle = 0;
	private Vector3 aimVector;
	private float aimTimeTarget = 1.0f;
	private float toAim = 0.0f;
	private bool canGrapple = true;

	//Grapple aim renderer!
	public LineRenderer render;
	public GameObject lineRender;
	RaycastHit hit;
	public Gradient registeredGrad;
	public Gradient unregisteredGrad;

	//Now the grapple rope itself.
	public LineRenderer ropeRender;
	public GameObject ropeLineRender;
	private bool latchActive = false;
	public PlayerMove moveScript;
	public Pause pauseScript;

	public AbilityManager abiMan;

	//Audio
	public AudioSource sound;
	public AudioClip succTether;
	public AudioClip failTether;
	public AudioClip release;



	//      MUSH DETECTED
	[SerializeField] private bool mushDetected = false;

	void Start () 
	{
		//ropeRender.enabled = true;
		//render.enabled = true;

		abiMan = GameObject.FindGameObjectWithTag("GameController").GetComponent<AbilityManager>();
	}
	

	void Update () 
	{
		//Set up all of our inputs that we need, so we don't need to keep using this command.
		rsVert = Input.GetAxisRaw ("XboxRSVertical") * -1;
		rsHoriz = Input.GetAxisRaw ("XboxRSHorizontal");
		xboxLT = Input.GetAxisRaw ("XboxLT");
		xboxRT = Input.GetAxisRaw ("XboxRT");
		xboxB = Input.GetButton ("XboxB");

		//If the player respawns...
		if (GameObject.FindWithTag("Player") == false || moveScript == null)
		{
			moveScript = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
		}

		if (pauseScript == null)
		{
			pauseScript = GameObject.FindWithTag("GameController").GetComponent<Pause>();
		}

		//After amount of time defined in "aimTimeTarget," we can grapple again.
		if (Time.time > toAim)
		{
			canGrapple = true;
		}

		//If our right stick is moved at all and we are not grappling, we grab our angle.
		if ((rsVert != 0 || rsHoriz != 0) && !isGrappling)
		{
			//isAiming = true;
			//aimAngle = Mathf.Atan2(rsHoriz, rsVert) * Mathf.Rad2Deg;
			aimVector = new Vector3(rsHoriz, rsVert, 0);
			GrappleAim();

			//Since we're aiming, let's also perform our Raycast. MushD set to false when raycast is not being performed,
			//and when the tag is not "Tether."
			if (Physics.Raycast(transform.position, aimVector, out hit, grappleLength))
			{
				if (hit.transform.gameObject.tag == "Tether")
				{
					mushDetected = true;
					render.colorGradient = registeredGrad;

				}
				else
				{
					mushDetected = false;
					render.colorGradient = unregisteredGrad;
				}
			}
			else
			{
				render.colorGradient = unregisteredGrad;
				mushDetected = false;
			}

			//When trigger is pulled and we can grapple, set the timer to grapple again, and Grapple().
			if (xboxRT > 0 && canGrapple == true)
			{
				canGrapple = false;
				toAim = aimTimeTarget + Time.time;
				abiMan.grappleBar = aimTimeTarget + Time.time;
				Grapple();
				lineRender.SetActive(false);

				if (mushDetected)
				{
					sound.clip = succTether;
				}
				else if (!mushDetected)
				{
					sound.clip = failTether;
				}
				sound.Play();
			}
		}
		else
		{
			//If the right stick isn't being touched, all these are false! Stop drawing that line!
			//isAiming = false;
			lineRender.SetActive(false);
			render.colorGradient = unregisteredGrad;
			if (!isGrappling)
			{
				mushDetected = false;
			}
		}

		if (isGrappling)
		{
			mushDetected = true;
			Grapple();
		}

	}

	void Grapple()
	{
		//Name the springjoint initially.
		SpringJoint spring = gameObject.GetComponent<SpringJoint>();

		//If we grapple and mushDetected, then we render the grapple, set its points, and set some bools.
		//If that's the case and we press B, we add a force toward the grapple location.
		if (mushDetected)
		{
			ropeLineRender.SetActive(true);
			ropeRender.SetPosition(1, gameObject.transform.position);
			ropeRender.SetPosition(0, hit.point);
			ropeLineRender.transform.position = hit.point;
			isGrappling = true;
			moveScript.grappling = true;
			moveScript.launched = true;

			//If we haven't done this yet, we fiddle with our springjoint.
			if (latchActive == false)
			{
				spring.connectedAnchor = hit.point;
				spring.spring = grappleStrength;
				spring.damper = dampen;
				latchActive = true;
			}

			if (xboxB)
			{
				gameObject.GetComponent<Rigidbody>().AddForce((hit.point - gameObject.transform.position) * 24f, ForceMode.Force);
			}
		}



		//Once the trigger is released, we kill the grapple, essentially.
		if ((xboxRT <= 0 || xboxLT > 0) && pauseScript.paused == false)
		{
			moveScript.grappling = false;
			isGrappling = false;
			ropeLineRender.SetActive(false);
			spring.spring = 0f;
			spring.damper = 0.0f;
			latchActive = false;

			sound.clip = release;
			sound.Play();
		}

	}

	void GrappleAim()
	{
		 
		if (canGrapple == true)
		{
			//If we are able to grapple, we render our line. Point 0 is set to the player, and point 1 is either
			//where our Raycast lands, or wherever we're aiming.
			lineRender.SetActive(true);
			render.SetPosition(0, gameObject.transform.position);
			if (Physics.Raycast(transform.position, aimVector, out hit, grappleLength))
			{
				render.SetPosition(1, hit.point);
				if (mushDetected)
				{
					//change color, here
				}
			}
			else 
			{
				render.SetPosition(1, (aimVector * 8) + gameObject.transform.position);
			}

		}

	}
}
