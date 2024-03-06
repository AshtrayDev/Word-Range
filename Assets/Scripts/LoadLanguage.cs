using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LoadLanguage : MonoBehaviour
{
	WordFormat wordFormat;
	public string embeddingsFilePath = "Assets/word_embeddings.csv";

	public Dictionary<string, float[]> wordEmbeddings = new Dictionary<string, float[]>();
	public Dictionary<string, int> wordToIndex;
	
	List<string> targetWords;
	string mainWord;
	string winningWord;

	void Awake()
	{
		LoadEmbeddings();
		AssignIndices();
		wordFormat = GetComponent<WordFormat>();
		SetNewMainWord();
	}
	
	void Start()
	{
		//Clears all duplicates
		for (int i = 0; i < targetWords.Count; i++)
		{
			targetWords[i] = wordFormat.FormatWord(targetWords[i]);
			List<string> NoDupes = targetWords.Distinct().ToList();
			targetWords = NoDupes;
		}
		

		
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

	List<string> FindClosestWords(string targetWord, int numWords)
	{
		if (wordEmbeddings.ContainsKey(targetWord))
		{
			// Get embedding of target word
			float[] targetEmbedding = wordEmbeddings[targetWord];

			// Create list to store distances
			List<Tuple<string, float>> distances = new List<Tuple<string, float>>();

			// Calculate distance from target word to all other words
			foreach (var kvp in wordEmbeddings)
			{
				if (kvp.Key != targetWord)
				{
					float distance = VectorDistance(targetEmbedding, kvp.Value);
					distances.Add(new Tuple<string, float>(kvp.Key, distance));
				}
			}

			// Sort distances in ascending order
			distances.Sort((x, y) => x.Item2.CompareTo(y.Item2));

			// Print the closest words
			//Debug.Log("Closest words to '" + targetWord + "':");
			List<string> strings = new List<string>();
			for (int i = 0; i < Mathf.Min(numWords, distances.Count); i++)
			{
				//Debug.Log(distances[i].Item1 + " (Distance: " + distances[i].Item2 + ")");
				strings.Add(distances[i].Item1);
			}
			return strings;
		}
		else
		{
			Debug.LogError("Word '" + targetWord + "' not found in embeddings.");
			return null;
		}
	}

	float VectorDistance(float[] vector1, float[] vector2)
	{
		if (vector1.Length != vector2.Length)
		{
			throw new ArgumentException("Vector dimensions must match");
		}

		float sum = 0f;
		for (int i = 0; i < vector1.Length; i++)
		{
			sum += Mathf.Pow(vector1[i] - vector2[i], 2);
		}
		return Mathf.Sqrt(sum);
	}
	
	void AssignIndices()
	{
		wordToIndex = new Dictionary<string, int>();

		// Convert keys of the word embeddings dictionary into an array
		string[] words = new string[wordEmbeddings.Count];
		wordEmbeddings.Keys.CopyTo(words, 0);

		// Assign an integer index to each word
		for (int i = 0; i < words.Length; i++)
		{
			wordToIndex[words[i]] = i;
		}
	}
	
	string GetRandomWord()
	{
		// Check if the dictionary is empty
		if (wordEmbeddings.Count == 0)
		{
			Debug.LogWarning("Dictionary is empty.");
			return null;
		}

		// Generate a random index within the range of the dictionary size
		int randomIndex = UnityEngine.Random.Range(0, wordEmbeddings.Count);

		// Get the word at the random index
		string[] words = new string[wordEmbeddings.Keys.Count];
		wordEmbeddings.Keys.CopyTo(words, 0);
		string randomWord = words[randomIndex];

		return randomWord;
	}
	
	public string GiveTargetWord(bool isWinningWord)
	{
		if(isWinningWord)
		{
			return winningWord;
		}
		
		int wordNum = UnityEngine.Random.Range(0, targetWords.Count-1);
		string word = targetWords[wordNum];
		targetWords.RemoveAt(wordNum);
		word = wordFormat.FormatWord(word);
		return word;
	}
	
	public string GetMainWord()
	{
		return wordFormat.FormatWord(mainWord);
	}
	
	public void SetNewMainWord()
	{
		mainWord = GetRandomWord();
		targetWords = FindClosestWords(mainWord, 20);
		winningWord = targetWords[0];
		targetWords.Remove(winningWord);
	}
}
