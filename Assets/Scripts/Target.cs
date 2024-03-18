using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Target : MonoBehaviour
{

	enum MoveType{still, random}
	
	[SerializeField] Vector2 direction = new Vector2(0,0);
	[SerializeField] float timeBetweenStrafe;
	[SerializeField] MoveType moveType;
	[SerializeField] float speed;
	
	[SerializeField] GameObject wordTextObject;
	
	public bool winningWord;
	
	float startTime;
	float endTime;
	
	TargetSpawner targetSpawner;
	WordFormat wordFormat;
	PlayerScore score;
	
	void Awake()
	{
		wordFormat = FindObjectOfType<WordFormat>();
	}

	// Start is called before the first frame update
	void Start()
	{
		if(moveType == MoveType.random)
		{
			StartCoroutine(RandomMove());
		}
		StartCoroutine(TryGetWord());
		targetSpawner = FindObjectOfType<TargetSpawner>();
		targetSpawner.SetRandomStats(gameObject);
		score = FindObjectOfType<PlayerScore>();
		startTime = Time.time;
	}

	// Update is called once per frame
	void Update()
	{
		Move();
	}
	
	public void TargetHit()
	{
		if(winningWord)
		{
			targetSpawner.WinningWordHit();
			float timeToHit = Time.time - startTime;
			score.AddScore(timeToHit, transform.localScale.x, speed);
		}
		else
		{
			score.RemoveScore();
		}
		
		Destroy(gameObject);
	}
	
	IEnumerator RandomMove()
	{
		while (true)
		{
			WaitForSeconds wait = new WaitForSeconds(timeBetweenStrafe);
			direction = new Vector2(UnityEngine.Random.Range(-1f,1f),UnityEngine.Random.Range(-1f,1f));
			yield return wait;
		}
	}
	
	IEnumerator TryGetWord()
	{
		string newWord = "";
		while(newWord == "")
		{
			try
			{
				newWord = GetNewWord();
				ChangeWord(newWord);
			}
			catch (Exception e)
			{
				Debug.LogWarning(e);
			}
			yield return new WaitForSeconds(0.1f);			
		}
	}
	
	void Move()
	{
		Vector3 currentPos = transform.position;
		Vector3 newPos;
		newPos = direction * speed * Time.deltaTime;
		newPos = newPos + currentPos;
		transform.position = newPos;
	}
	
	void OnTriggerEnter(Collider other)
	{
		direction = -direction;
	}
	
	void ChangeWord(string word)
	{
		TMP_Text tmp = wordTextObject.GetComponent<TMP_Text>();
		tmp.text = wordFormat.FormatWord(word);
	}
	
	string GetNewWord()
	{
		return FindObjectOfType<LoadLanguage>().GiveTargetWord(winningWord);
	}
	
	public void SetSpeed(float newSpeed)
	{
		speed = newSpeed;
	}
	
}
