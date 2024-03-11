using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using UnityEditor.SceneManagement;
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
	
	TargetSpawner targetSpawner;
	WordFormat wordFormat;
	
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
		
		string newWord = GetNewWord();
		ChangeWord(newWord);
		targetSpawner = FindObjectOfType<TargetSpawner>();
		targetSpawner.SetRandomStats(gameObject);
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
