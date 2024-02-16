using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class Target : MonoBehaviour
{

	enum MoveType{still, random}
	
	[SerializeField] Vector2 direction = new Vector2(0,0);
	[SerializeField] float timeBetweenStrafe;
	[SerializeField] MoveType moveType;
	[SerializeField] float speed;
	
	// Start is called before the first frame update
	void Start()
	{
		if(moveType == MoveType.random)
		{
			StartCoroutine(RandomMove());
		}
	}

	// Update is called once per frame
	void Update()
	{
		Move();
	}
	
	public void TargetHit()
	{
		Destroy(gameObject);
	}
	
	IEnumerator RandomMove()
	{
		while (true)
		{
			WaitForSeconds wait = new WaitForSeconds(timeBetweenStrafe);
			direction = new Vector2(Random.Range(-1f,1f),Random.Range(-1f,1f));
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
		print(other);
		direction = -direction;
	}
	
}
