using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{

	Camera cam;
	[SerializeField] bool isAutomatic = false;
	
	void Start()
	{
		cam = FindObjectOfType<Camera>();
	}
	// Update is called once per frame
	void Update()
	{
		DetectPlayerInput();
	}
	
	void DetectPlayerInput()
	{
		if(Input.GetButtonDown("Fire1"))
		{
			Shoot();
		}
	}
	
	void Shoot()
	{
		RaycastHit hit;
		if(Physics.Raycast(cam.transform.position, transform.TransformDirection(Vector3.forward), out hit))
		{
			if(hit.collider.tag == "Target")
			{
				Destroy(hit.collider.gameObject);
			}
		}
	
	}
}
