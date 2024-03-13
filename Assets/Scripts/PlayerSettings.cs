using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerSettings : MonoBehaviour
{
	[SerializeField] GameObject crosshair1;
	[SerializeField] GameObject crosshair2;
	[SerializeField] GameObject crosshair3;
	[SerializeField] GameObject crosshair4;
	[SerializeField] GameObject dot;
	[SerializeField] GameObject mainWord;
	[SerializeField] GameObject score;
	[SerializeField] Color crosshairColour;
	[SerializeField, Range(0,200)] float offset = 6;
	[SerializeField, Range(0,5)] float thickness = 0.77f;
	[SerializeField, Range(0,5)] float length = 0.42f;
	[SerializeField] bool enableDot;
	[SerializeField] float dotSize;
	[SerializeField] Color targetColor;
	
	Image crosshair1Image;
	Image crosshair2Image;
	Image crosshair3Image;
	Image crosshair4Image;
	Image dotImage;
	TMP_Text mainWordText;
	TMP_Text scoreText;
	
	RectTransform crosshair1Rect;
	RectTransform crosshair2Rect;
	RectTransform crosshair3Rect;
	RectTransform crosshair4Rect;
	RectTransform dotRect;
	WordFormat wordFormat;
	
	void Awake()
	{
		crosshair1Image = crosshair1.GetComponent<Image>();
		crosshair2Image = crosshair2.GetComponent<Image>();
		crosshair3Image = crosshair3.GetComponent<Image>();
		crosshair4Image = crosshair4.GetComponent<Image>();
		dotImage = dot.GetComponent<Image>();
		
		crosshair1Rect = crosshair1.GetComponent<RectTransform>();
		crosshair2Rect = crosshair2.GetComponent<RectTransform>();
		crosshair3Rect = crosshair3.GetComponent<RectTransform>();
		crosshair4Rect = crosshair4.GetComponent<RectTransform>();
		dotRect = dot.GetComponent<RectTransform>();
		mainWordText = mainWord.GetComponent<TMP_Text>();
		scoreText = score.GetComponent<TMP_Text>();
		wordFormat = FindObjectOfType<WordFormat>();
	}
	
	void Start()
	{
		
	}
	
	void Update()
	{
		UpdateOffset();
		UpdateCrosshairColor();
		UpdateCrosshairSize();
		UpdateDot();
	}
	
	void UpdateOffset()
	{
		Vector3 newPos;
		newPos = crosshair1.transform.localPosition;
		newPos.y = offset;
		crosshair1.transform.localPosition = newPos;
		
		newPos = crosshair2.transform.localPosition;
		newPos.y = -offset;
		crosshair2.transform.localPosition = newPos;
		
		newPos = crosshair3.transform.localPosition;
		newPos.x = offset;
		crosshair3.transform.localPosition = newPos;
		
		newPos = crosshair3.transform.localPosition;
		newPos.x = -offset;
		crosshair4.transform.localPosition = newPos;
	}
	
	void UpdateCrosshairColor()
	{
		if(crosshair4Image != null)
		{
			crosshair1Image.color = crosshairColour;
			crosshair2Image.color = crosshairColour;
			crosshair3Image.color = crosshairColour;
			crosshair4Image.color = crosshairColour;
			dotImage.color = crosshairColour;
		}
	}
	
	void UpdateCrosshairSize()
	{
		crosshair1Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, thickness * 100);
		crosshair1Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, length * 100);
		crosshair2Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, thickness * 100);
		crosshair2Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, length * 100);
		crosshair3Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, thickness * 100);
		crosshair3Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, length * 100);
		crosshair4Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, thickness * 100);
		crosshair4Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, length * 100);
	}
	
	void UpdateDot()
	{
		dot.SetActive(enableDot);
		dotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dotSize * 100);
		dotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dotSize * 100);
	}
	
	public void UpdateMainWordText()
	{
		string word = FindObjectOfType<LoadLanguage>().GetMainWord();
		mainWordText.text = wordFormat.FormatWord(word);
	}
	
	public void UpdateScoreText(int score)
	{
		scoreText.text = score.ToString();
	}
}
