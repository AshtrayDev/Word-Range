using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class TargetSpawner : MonoBehaviour
{

	[SerializeField] GameObject targetPrefab;
	[SerializeField] GameObject targetWinPrefab;
	[SerializeField] float minSize;
	[SerializeField] float maxSize;
	
	[SerializeField, Range(1,20)] float maxSpeed;
	[SerializeField] Vector2 minSpawnPos;
	[SerializeField] Vector2 maxSpawnPos;
	[SerializeField] float zPosition;
	[SerializeField] int targetsPerRound;
 	
	List<GameObject> targets = new List<GameObject>();
	
	LoadLanguage language;
	PlayerSettings settings;
	
	void Awake()
	{
		language = FindObjectOfType<LoadLanguage>();
		settings = FindAnyObjectByType<PlayerSettings>();
	}
	void Start()
	{
		AddNewTargets();
	}
	
	public GameObject SpawnTarget()
	{
		return Instantiate(targetPrefab, GetRandomPosition(), transform.rotation, transform);
	}
	
	public GameObject SpawnWinTarget()
	{
		return Instantiate(targetWinPrefab, GetRandomPosition(), transform.rotation, transform);
	}
	
	public void SetRandomStats(GameObject targetObject)
	{
		Target target = targetObject.GetComponent<Target>();
		SetRandomSpeed(target);
		SetRandomSize(targetObject);
	}
	
	public Vector3 GetRandomPosition()
	{
		float xPos = Random.Range(minSpawnPos.x, maxSpawnPos.x);
		float yPos = Random.Range(minSpawnPos.y, maxSpawnPos.y);
		
		return new Vector3(xPos, yPos, zPosition);
	}
	
	public void SetRandomSpeed(Target target)
	{
		target.SetSpeed(Random.Range(1, maxSpeed));
	}
	
	public void SetRandomSize(GameObject target)
	{
		float size = Random.Range(minSize, maxSize);
		target.transform.localScale = new Vector3(size, size, size);
	}
	
	public void AddNewTargets()
	{
		targets.Add(SpawnWinTarget());
		for (int i = 0; i < targetsPerRound-1; i++)
		{
			targets.Add(SpawnTarget());
		}
	}
	
	public void RemoveAllTargets()
	{
		foreach(GameObject target in targets)
		{
			Destroy(target);
		}
		targets.Clear();
	}
	
	public void WinningWordHit()
	{
		language.NextWord();
		RemoveAllTargets();
		AddNewTargets();
		settings.UpdateMainWordText();
	}
}
