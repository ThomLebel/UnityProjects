using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeScript : MonoBehaviour
{
	public void MoveRecipe(Vector2 position)
	{
		gameObject.GetComponent<RectTransform>().anchoredPosition = position;
	}
}
