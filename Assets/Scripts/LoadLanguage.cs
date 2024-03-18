using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;

public class LoadLanguage : MonoBehaviour
{
	WordFormat wordFormat;
	public string embeddingsFilePath = "Assets/word_embeddings.csv";

	public Dictionary<string, float[]> wordEmbeddings = new Dictionary<string, float[]>();
	public Dictionary<string, Dictionary<string, float>> upcomingWords = new Dictionary<string, Dictionary<string, float>>();
	List<string> mainWords = new List<string>();
	[SerializeField] public KDTree kDTree;
	
	public Dictionary<string, float> targetWords = new Dictionary<string, float>();
	string mainWord;
	string winningWord;
	int wordNumber = 0;

	void Awake()
	{
		LoadEmbeddings();
		kDTree.BuildTree(wordEmbeddings);
		wordFormat = GetComponent<WordFormat>();
	}
	
	void Start()
	{
		AddUpcomingWords(30);
	}

	void LoadEmbeddings()
	{
		// Read lines from CSV file
		string[] lines = File.ReadAllLines(embeddingsFilePath);
		// Skip header line if present
		int startIndex = lines[0].Contains("Word,Embedding") ? 1 : 0;

		// Parse each line and add to dictionary
		for (int i = startIndex; i < lines.Length; i++)
		{
			string line = lines[i];
			string[] parts = line.Split(',');
			string word = parts[0];
			
			// Exclude words
			if (word.Contains("_PROPN") || word.Contains("::") || word.Contains("_NUM") || word.Contains("www.") || word.Contains("~"))
				continue; // Skip the current word

			float[] embeddingValues = ParseEmbeddingValues(parts);
			wordEmbeddings[word] = embeddingValues;
		}
	}

	float[] ParseEmbeddingValues(string[] parts)
	{
		float[] values = new float[parts.Length - 1];
		for (int i = 1; i < parts.Length; i++)
		{
			float value;
			if (float.TryParse(parts[i], out value))
			{
				values[i - 1] = value;
			}
			else
			{
				Debug.LogError("Invalid value format: " + parts[i]);
			}
		}
		return values;
	}
	
	
	string GetRandomWord(Dictionary<string, float[]> data)
	{
		int index = UnityEngine.Random.Range(0, data.Count);
		return data.Keys.ElementAt(index);
	}
	
	public string GiveTargetWord(bool isWinningWord)
	{

		if(isWinningWord)
		{
			winningWord = upcomingWords[mainWord].Keys.ElementAt(0);
			upcomingWords[mainWord].Remove(winningWord);
			return winningWord;
		}
		
		
		int wordNum = UnityEngine.Random.Range(0, upcomingWords[mainWord].Count-1);
		print("Ran number: " + wordNum);
		print("Count-1: " + (upcomingWords[mainWord].Count-1));
		string word = upcomingWords[mainWord].Keys.ElementAt(wordNum);
		upcomingWords[mainWord].Remove(word);
		return word;
	}
	
	public string GetMainWord()
	{
		return mainWord;
	}
	
	public void SetNewMainWord()
	{
		mainWord = GetRandomWord(wordEmbeddings);
		mainWords.Add(mainWord);
		Dictionary<string, float[]> neighbors = kDTree.FindNearestNeighbors(mainWord, 20, wordEmbeddings);
		Dictionary<string, float[]> NoDupes = new Dictionary<string, float[]>();
		
		//Clears all duplicates
		for (int i = 0; i < neighbors.Count; i++)
		{
			try
			{
				NoDupes.Add(wordFormat.FormatWord(neighbors.Keys.ElementAt(i)),neighbors[neighbors.Keys.ElementAt(i)]);
			}
			catch (Exception e)
			{
				Debug.LogWarning(e);
			}
		}
		print("NEIGHBOURS COUNT: " + neighbors.Count);
		print("NODUPES COUNT: " + NoDupes.Count);
		neighbors = NoDupes;
		targetWords = SortWordsBySimilarity(neighbors);
	}
	
	public void AddUpcomingWords(int count)
	{
		for (int i = 0; i < count; i++)
		{
			bool acceptableDistance = false;
			while(acceptableDistance == false)
			{
				SetNewMainWord();
				float distance = targetWords.Values.ElementAt(0);
				if(distance < 2f)
				{
					print("acceptable");
					print(distance);
					acceptableDistance = true;
				}
				
				else
				{
					print(distance);
					print("not acceptable");
					mainWords.RemoveAt(mainWords.Count-1);
				}
			}
			upcomingWords.Add(mainWord, targetWords);
		}
	}
	
	public void NextWord()
	{
		wordNumber++;
		mainWord = upcomingWords.Keys.ElementAt(wordNumber);
		
		foreach(var word in upcomingWords[mainWord])
		{
			print(word.Key + ":" + word.Value);
		}
	}
	
	public Dictionary<string, float> SortWordsBySimilarity(Dictionary<string, float[]> words)
	{
		// Calculate distance scores for each word
		Dictionary<string, float> distanceScores = new Dictionary<string, float>();
		float[] mainWordEmbedding = wordEmbeddings[mainWords[wordNumber]];
		foreach (var word in words)
		{
			float[] wordEmbedding = word.Value;
			float distance = CalculateDistance(mainWordEmbedding, wordEmbedding);
			distanceScores[word.Key] = distance;
		}
		Dictionary<string, float> sortedDistanceScores = distanceScores.OrderBy(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
		return sortedDistanceScores;
	}
	
	private float CalculateDistance(float[] embedding1, float[] embedding2)
	{
		float sum = 0;
		for (int i = 0; i < embedding1.Length; i++)
			sum += (embedding1[i] - embedding2[i]) * (embedding1[i] - embedding2[i]);
		return (float)Math.Sqrt(sum);
	}
}