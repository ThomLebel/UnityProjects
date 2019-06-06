using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientScript : ItemScript
{
	public string prepName = "";
	public Sprite prepSprite;

	public bool isPrepared = false;
	public float preparingCoef = 0.2f;

	public ProgressBarScript progressBarScript;

	void Awake()
	{
		progressBarScript = gameObject.GetComponentInChildren<ProgressBarScript>();
	}

	public void PrepareIngredient()
	{
		if (progressBarScript.value < 1)
		{
			progressBarScript.value += Time.deltaTime * preparingCoef;

			if (progressBarScript.value > 0)
				isPrepared = true;
		}
		else
		{
			progressBarScript.value = 1;
			if (isPrepared)
			{
				//Change visuel and name
				isPrepared = false;
				itemName = prepName;
				GetComponentInChildren<SpriteRenderer>().sprite = prepSprite;
			}
		}
	}
}
