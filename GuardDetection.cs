using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardDetection : MonoBehaviour 
{
	public GameManager gameManager;
	public Transform head;
	public GameObject player;
	public GuardSpotlightDetection spotlight;
	public GameObject camera;
	public Transform camSpawn;
	public bool seen = false;
	private bool cameraIn = false;
	AudioSource alert;
	public GuardLife life;


	void Start ()
	{
		alert = GetComponent<AudioSource>();
	}

	void Update () 
	{
		if (player != null)
		{
			RaycastHit hit;
			Vector3 raycastDir = (player.transform.position - transform.position);
			Ray detectionRay = new Ray(head.position, raycastDir);
			if (Physics.Raycast(detectionRay, out hit, 30.0f))
			{
				
				if (hit.transform == player.transform && spotlight.seen == true && life.active == true)
				{
					gameManager.caught = true;
					alert.Play();

					seen = true;
					if (cameraIn == false)
					{
						Instantiate(camera, camSpawn);
						cameraIn = true;
					}
				}
				else
				{
					seen = false;
				}
			}
		}



	}
		
}
