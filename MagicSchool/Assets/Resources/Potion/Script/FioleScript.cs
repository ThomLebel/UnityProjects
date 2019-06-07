using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FioleScript : ItemScript
{
	public SpriteRenderer spriteRenderer;
	public Sprite emptyPotion;
	public Sprite bluePotion;
	public Sprite greenPotion;
	public Sprite redPotion;
	public Sprite yellowPotion;

	private FillableScript fillableScript;

	private void Start()
	{
		fillableScript = GetComponent<FillableScript>();
	}

	public void SetPotionVisual()
	{
		string firstIngredient = fillableScript.itemList[0];

		switch (firstIngredient)
		{
			case "spider":
				spriteRenderer.sprite = bluePotion;
				break;
			case "frog":
				spriteRenderer.sprite = greenPotion;
				break;
			case "bat":
				spriteRenderer.sprite = redPotion;
				break;
			case "rat":
				spriteRenderer.sprite = yellowPotion;
				break;
			default:
				spriteRenderer.sprite = emptyPotion;
				break;
		}
	}

	public void EmptyPotion()
	{
		spriteRenderer.sprite = emptyPotion;
	}
}
