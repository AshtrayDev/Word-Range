using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerTimer : MonoBehaviour
{
	[SerializeField] int timerMin;
	[SerializeField] int timerSeconds;
	
	int minutes;
	int seconds;
	
	PlayerSettings settings;
	PlayerCamera cam;
	
	
	// Start is called before the first frame update
	void Start()
	{
		settings = FindObjectOfType<PlayerSettings>();
		cam = FindObjectOfType<PlayerCamera>();
		ResetTimer();
	}

	IEnumerator StartTimer()
	{
		while(true)
		{
			WaitForSeconds wait = new WaitForSeconds(1);
			if(seconds <= 0)
			{
				if(minutes <= 0)
				{
					cam.StopCameraMovement();
					settings.SetActiveEndScreen(true);
					StopCoroutine(StartTimer());
				}
				else
				{
					minutes--;
					seconds = 59;
				}
			}
			else
			{
				seconds--;
			}
			settings.UpdateTimerText(minutes, seconds);
			yield return wait;
		}
	}
	
	public void ResetTimer()
	{
		cam.StartCameraMovement();
		StopCoroutine(StartTimer());
		minutes = timerMin;
		seconds = timerSeconds;
		StartCoroutine(StartTimer());
	}
}
