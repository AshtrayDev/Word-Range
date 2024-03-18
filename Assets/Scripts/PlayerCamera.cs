using System;
using System.Collections;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

	[Range(0.1f, 9f)][SerializeField] float sensitivity = 2f;
	[Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;
	
	bool movementEnabled;

	Vector2 rotation = Vector2.zero;

		void Start()
		{
			StartCameraMovement();
		}
		// Update is called once per frame
		void Update()
		{
			if(movementEnabled)
			{
				rotation.x = rotation.x + Input.GetAxis("Mouse X") * sensitivity;
				rotation.y = rotation.y + Input.GetAxis("Mouse Y") * sensitivity;
				rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
				Quaternion xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
				Quaternion yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

				transform.localRotation = xQuat * yQuat;
			}

		}
		
		public void StopCameraMovement()
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			movementEnabled = false;
		}
		
		public void StartCameraMovement()
		{
			Cursor.visible = false; 
			Cursor.lockState = CursorLockMode.Locked;
			movementEnabled = true;
		}
	}
		
