using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class WordFormat : MonoBehaviour
{
	enum Language {english, welsh}
	
	[SerializeField] Language language;
	// Start is called before the first frame update
	
	public string FormatWord(string word)
	{
		switch(language)
		{
			case Language.english:
				string formattedWord = Regex.Replace(word, @"_(\w+)", "");
				formattedWord = char.ToUpper(formattedWord[0]) + formattedWord.Substring(1);
				return formattedWord;
			case Language.welsh:
				return "test";
		}
		return "test";
	}
}
