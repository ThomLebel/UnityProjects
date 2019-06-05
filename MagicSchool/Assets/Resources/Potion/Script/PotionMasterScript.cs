using Com.OniriqueStudio.MagicSchool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionMasterScript : MonoBehaviour {

	public int pointByIngredient = 10;

	public MalusScript malusScript;

	[SerializeField]
	private PotionManager potionManager;


	public int CheckPotionValidity(GameObject player, List<string> pPotion)
	{
		int potionIndex = 0;
		bool potionValide = false;
		int score = 0;

		if (potionManager.recipesList.Count > 0)
		{
			for (int i = 0; i < potionManager.recipesList.Count; i++)
			{
				List<string> recipe = potionManager.recipesList[i];

				int ingredientValide = 0;

				if (recipe.Count == pPotion.Count)
				{
					for (int e = 0; e < recipe.Count; e++)
					{
						if (recipe[e] != pPotion[e])
						{
							break;
						}
						else{
							ingredientValide++;
						}
					}
					if (ingredientValide == recipe.Count)
					{
						potionIndex = i;
						score = recipe.Count * pointByIngredient;
						potionValide = true;
						break;
					}
				}
			}
		}

		if (potionValide)
		{
			MalusOnOtherPlayers(player);
			DestroyAndGenerateNewRecipe(potionIndex);
			return score;
		}

		MalusOnPlayer(player);
		score = pPotion.Count * pointByIngredient / 2;
		return score * -1;
	}

	private void MalusOnOtherPlayers(GameObject player)
	{
		Debug.Log("All other players get a malus");
		int playerID = player.GetComponent<PlayerInfo>().playerID;

		if (GameManager.Instance.players.Count > 1)
		{
			int malusIndex = Random.Range(0, malusScript.malusList.Count);

			List<GameObject> playersList = GameManager.Instance.players;
			for (int i=0; i<playersList.Count; i++)
			{
				GameObject target = playersList[i];
				int targetID = target.GetComponent<PlayerInfo>().playerID;

				//A changer par le team ID
				if (targetID != playerID)
				{
					malusScript.malusList[malusIndex](target);
				}
			}
		}
	}

	private void MalusOnPlayer(GameObject player)
	{
		Debug.Log("You get a malus");
		int malusIndex = Random.Range(0, malusScript.malusList.Count);
		malusScript.malusList[malusIndex](player);
	}

	private void DestroyAndGenerateNewRecipe(int potionIndex)
	{
		//Destroy the recipe
		potionManager.recipesList.RemoveAt(potionIndex);
		Destroy(potionManager.recipeImageList[potionIndex]);
		potionManager.recipeImageList.RemoveAt(potionIndex);

		//Generate a new recipe
		GameObject recipe = potionManager.GenerateRecipe();

		//Animate recipes that aren't at the right place
		float recipeAnimationTime = 1f;
		float recipeAnimationDelay = 0f;

		for (int i = potionIndex; i < potionManager.maxRecipes; i++)
		{
			GameObject animatedRecipe = potionManager.recipeImageList[i];

			potionManager.MoveRecipe(animatedRecipe, i, recipeAnimationTime, recipeAnimationDelay);

			recipeAnimationDelay += recipeAnimationTime / 2;
		}
	}
}
