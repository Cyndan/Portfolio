using UnityEngine;
using System.Collections;

public class MissileBehavior : MonoBehaviour 
{
	public float missileSpeed;
	public int bounceAllowed;
	public GameObject explosion;
	public AudioSource bump;

	private int shotsOut;
	private int bounces = 0;

	void Start () 
	{
		GetComponent<Rigidbody2D> ().velocity = transform.up * missileSpeed;
	}
	

	void Update () 
	{
	
	}
		
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Player2") 
		{
			Instantiate (explosion, other.transform.position, other.transform.rotation);
			// gameController.GameOver ();

			Destroy (other.gameObject);
			Destroy (gameObject);

		}
		if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Waterfall") 
		{
			bounces = bounces + 1;
			bump.Play ();
			if (bounces >= 4) {
				Destroy (gameObject);
			}
		}
	}
}
