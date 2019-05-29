using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionMasterScript : MonoBehaviour {

	[SerializeField]
	private PotionManager potionManager;

	public bool CheckPotionValidity(GameObject player, List<string> pPotion)
	{
		Debug.Log(pPotion);
		bool potionValide = false;

		if (potionManager.recipesList.Count > 0)
		{
			for (int i = 0; i < potionManager.recipesList.Count; i++)
			{
				List<string> recipe = potionManager.recipesList[i];
				int ingredientValide = 0;

				Debug.Log("Testing recipe number "+i);

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
						potionValide = true;
						break;
					}
					
				}
			}
		}

		if (potionValide)
		{
			return true;
		}

		return false;
	}
}
