using UnityEngine;
using System.Collections;

public class TankController : MonoBehaviour 
{

	public float moveSpeed;
	public float turnSpeed;
	public int playerNumber;
	public float fireRate = 1.5f;
	public GameObject Missile;
	public GameObject explosion;
	public Transform missileSpawnLocation1;
	public Transform missileSpawnLocation2;
	public Animator tankOneAnim;
	public Animator tankTwoAnim;
	public AudioSource shot; 


	private float nextShoot = 0.0f;
	private float moveInputValue;
	private float turnInputValue;
	private Rigidbody2D rigbody;
	private string moveAxisName;
	private string turnAxisName;
	private string shootInputName;
	private bool inPit;

	void Awake () 
	{
		rigbody = GetComponent<UnityEngine.Rigidbody2D> ();
		inPit = false;
	}

	private void OnEnable ()
	{
		rigbody.isKinematic = false;
		moveInputValue = 0f;
		turnInputValue = 0f;
	}

	private void OnDisable ()
	{
		rigbody.isKinematic = true;
	}

	void Start ()
	{
		if (playerNumber == 1)
		{
			moveAxisName = "Vertical1";
			turnAxisName = "Horizontal1";
			shootInputName = "Fire1";
		}

		if (playerNumber == 2)
		{
			moveAxisName = "Vertical2";
			turnAxisName = "Horizontal2";
			shootInputName = "Fire2";
		}
	}

	void Update () 
	{
		moveInputValue = Input.GetAxis (moveAxisName);
		turnInputValue = Input.GetAxis (turnAxisName);
		if (inPit == true) 
		{
			Instantiate (explosion, transform.position, transform.rotation);

			Destroy (gameObject);
		}
	}

	void FixedUpdate ()
	{
		Move ();
		Turn ();
		Shoot ();
		Anim ();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Finish") 
		{
			inPit = true;
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Waterfall") 
		{
			transform.Translate (0f, -0.01f, 0f, Space.World);
		}
	}

	private void Anim ()
	{
		if (playerNumber == 1) 
		{
			if (moveInputValue != 0 || turnInputValue != 0) 
			{
				tankOneAnim.SetBool ("Move", true);
			}
			if (moveInputValue == 0 || turnInputValue == 0) 
			{
				tankOneAnim.SetBool ("Move", false);
			}
		}

		if (playerNumber == 2) 
		{
			if (moveInputValue != 0 || turnInputValue != 0) 
			{
				tankTwoAnim.SetBool ("Move", true);
			}
			if (moveInputValue == 0 || turnInputValue == 0) 
			{
				tankTwoAnim.SetBool ("Move", false);
			}
		}
	}

	private void Move ()
	{
		Vector2 movement = transform.up * moveInputValue * moveSpeed * Time.deltaTime;
		rigbody.MovePosition (rigbody.position + movement);
	}

	private void Turn ()
	{
		transform.Rotate (Vector3.forward  * turnSpeed * turnInputValue);
	}

	private void Shoot ()
	{
		if (Input.GetButton (shootInputName) && Time.time > nextShoot) 
		{
			nextShoot = Time.time + fireRate;
			if (playerNumber == 1) 
			{
				shot.Play ();
				Instantiate (Missile, missileSpawnLocation1.position, missileSpawnLocation1.rotation);
			}

			if (playerNumber == 2) 
			{
				shot.Play ();
				Instantiate (Missile, missileSpawnLocation2.position, missileSpawnLocation2.rotation);
			}
		}
	}
}
