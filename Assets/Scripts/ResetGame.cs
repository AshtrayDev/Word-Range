using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGame : MonoBehaviour
{
	PlayerScore score;
	PlayerTimer timer;
	PlayerSettings settings;
	LoadLanguage language;
	
	void Start()
	{
		score = FindObjectOfType<PlayerScore>();
		timer = FindObjectOfType<PlayerTimer>();
		settings = FindObjectOfType<PlayerSettings>();
		language = FindObjectOfType<LoadLanguage>();
		print(language);
	}
	public void Reset()
	{
		language.AddUpcomingWords(30);
		score.SetScore(0);
		timer.ResetTimer();
		settings.SetActiveEndScreen(false);
	}
}
