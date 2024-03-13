using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
	[SerializeField] int baseTimeScore;
	[SerializeField] int baseSizeScore;
	[SerializeField] int baseSpeedScore;
	[SerializeField] float maxTimePenalty;
	[SerializeField] int wrongTargetScoreLoss;
	
	int score;
	
	PlayerSettings settings;
	
	// Start is called before the first frame update
	void Start()
	{
		settings = GetComponent<PlayerSettings>();
	}
	
	public void AddScore(float timeToHit, float size, float speed)
	{
		int timeBonus = Mathf.RoundToInt(baseTimeScore / Mathf.Clamp(timeToHit, 0, maxTimePenalty));
		int sizeBonus = Mathf.RoundToInt(baseSizeScore / size);
		int speedBonus = Mathf.RoundToInt(baseSpeedScore * speed);
		
		int totalScore = timeBonus + sizeBonus + speedBonus;
		SetScore(score + totalScore);
	}
	
	public void RemoveScore()
	{
		SetScore(score-wrongTargetScoreLoss);
	}
	
	public void SetScore(int newScore)
	{
		score = newScore;
		settings.UpdateScoreText(score);
	}
	
	public int GetScore()
	{
		return score;
	}
	
}
