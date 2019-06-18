using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionManager : MonoBehaviour {

	public List<Text> playersScore;

	//public float indicationSpace = 10f;
	public int maxRecipes = 6;

	public List<GameObject> ingredientsList;

	public List<Transform> SpawnPositionsList;

	public List<List<string>> recipesList = new List<List<string>>();
	public List<GameObject> recipeImageList = new List<GameObject>();

	[SerializeField]
	private Image recipePrefab;
	[SerializeField]
	private Sprite recipeSprite;
	[SerializeField]
	private GameObject canvas;
	[SerializeField]
	private GameObject medalPrefab;

	private float canvasWidth;
	private float canvasHeight;
	private float recipeSpace;
	private float recipeWidth;
	private float recipeHeight;

	static public PotionManager Instance;

	// Use this for initialization
	void Start () {
		canvas = GameObject.FindGameObjectWithTag("canvas");

		canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
		canvasHeight = canvas.GetComponent<RectTransform>().rect.height;

		recipeSpace = canvasWidth / maxRecipes;
		recipeWidth = recipePrefab.GetComponent<RectTransform>().rect.width;
		recipeHeight = recipePrefab.GetComponent<RectTransform>().rect.height;

		GameObject[] _players =  GameObject.FindGameObjectsWithTag("Player");
		int i = 0;
		foreach (GameObject player in _players)
		{
			PlayerInfo info = player.GetComponent<PlayerInfo>();

			Debug.Log("Init a player : "+player);
			info.Init(SpawnPositionsList[i].position);

			Debug.Log("Create a medal for this player");
			GameObject medal = Instantiate(medalPrefab);
			medal.transform.Find("back/playerFace/head").GetComponent<Image>().sprite = player.transform.Find("base_wizard/spineBone/headBone/head/headSprite").GetComponent<SpriteRenderer>().sprite;
			medal.transform.Find("back/playerFace/eyes").GetComponent<Image>().sprite = player.transform.Find("base_wizard/spineBone/headBone/base_eyes_open").GetComponent<SpriteRenderer>().sprite;
			medal.transform.SetParent(canvas.transform.Find("BotPanel"));
			//Position the medal based on team number
			RectTransform medalTransform = medal.GetComponent<RectTransform>();
			//medalTransform.anchoredPosition = new Vector2(0f,0f);
			float medalSpace = canvasWidth / _players.Length;
			float medalX = ((canvasWidth / 2) * -1) + (medalSpace *(info.playerID - 1)) + (medalSpace / 2);
			medalTransform.anchoredPosition = new Vector2(medalX, 0f);

			i++;
		}

		//recipeWidth = recipeSprite.rect.width;
		//recipeHeight = recipeSprite.rect.height;

		float recipeAnimationTime = 1f;
		float recipeAnimationDelay = 0f;

		for (int r=0; r<maxRecipes; r++)
		{
			GameObject recipe = GenerateRecipe();

			MoveRecipe(recipe, r, recipeAnimationTime, recipeAnimationDelay);

			recipeAnimationDelay += recipeAnimationTime/2;
		}
	}

	public void SetPlayerScore(int id, float score)
	{
		playersScore[id].text = score.ToString();
	}

	public GameObject GenerateRecipe()
	{
		if (recipesList.Count >= maxRecipes)
		{
			return null;
		}
		//List of all 4 basic ingredients shuffled
		List<GameObject> tempIngredientList = ShuffleList(ingredientsList);

		//List containing the recipe ingredients
		List<string> recipeIngredient = new List<string>();

		//Prefab of the recipe which will be drawn on screen
		GameObject recipe = new GameObject();
		Image _recipeSprite = recipe.AddComponent<Image>();
		recipe.AddComponent<RecipeScript>();
		_recipeSprite.sprite = recipeSprite;
		_recipeSprite.SetNativeSize();
		recipe.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1f);
		recipe.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1f);
		recipe.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
		recipe.GetComponent<RectTransform>().SetParent(canvas.transform);
		recipe.GetComponent<RectTransform>().anchoredPosition = new Vector2((canvasWidth / 2 + recipeWidth / 2) + 10, 0f);

		int randIngredientQuantity = Random.Range(2, 4);
		float ingredientSpace = recipeHeight / randIngredientQuantity;
		
		for (int i = 0; i < randIngredientQuantity; i++)
		{
			GameObject ingredient = tempIngredientList[i];
			IngredientScript ingredientScript = ingredient.GetComponent<IngredientScript>();

			GameObject ingredientElement = new GameObject();
			Image ingredientSprite = ingredientElement.AddComponent<Image>();
			ingredientElement.GetComponent<RectTransform>().SetParent(recipe.transform);

			bool prepared = (Random.value > 0.5f);
			if (prepared)
			{
				recipeIngredient.Add(ingredientScript.prepName);
				ingredientSprite.sprite = ingredientScript.prepSprite;
			}
			else
			{
				recipeIngredient.Add(ingredientScript.itemName);
				ingredientSprite.sprite = ingredient.GetComponentInChildren<SpriteRenderer>().sprite;
			}

			ingredientSprite.SetNativeSize();
			ingredientElement.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
			float yPos = (recipeHeight / 2) - (ingredientSpace * i) - (ingredientSpace / 2);
			float xPos = 0f;

			//Create indication of which ingredient we should prepare first
			if (prepared)
			{
				GameObject indicationElement = new GameObject();
				Image indicationSprite = indicationElement.AddComponent<Image>();

				indicationElement.GetComponent<RectTransform>().SetParent(recipe.transform);
				indicationSprite.sprite = ingredient.GetComponentInChildren<SpriteRenderer>().sprite;
				indicationSprite.SetNativeSize();
				indicationElement.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

				//float ingredientWidth = ingredientElement.GetComponent<RectTransform>().rect.width * 0.5f;
				//float indicationWidth = indicationElement.GetComponent<RectTransform>().rect.width * 0.2f;
				//float posX = (ingredientWidth / 2 + indicationWidth / 2 + indicationSpace) * -1;

				//float ingredientXSpace = recipeWidth / 4;
				//xPos = (recipeWidth / 2) - (ingredientXSpace * 2) - (ingredientXSpace / 2);

				indicationElement.GetComponent<RectTransform>().anchoredPosition = new Vector2(-20f, yPos);

				xPos = 20f;
			}

			ingredientElement.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
		}

		recipesList.Add(recipeIngredient);
		recipeImageList.Add(recipe);

		return recipe;
	}

	public void MoveRecipe(GameObject recipe, int index, float time, float delay)
	{
		RectTransform recipeTransform = recipe.GetComponent<RectTransform>();
		
		float recipeX = ((canvasWidth / 2) * -1) + (recipeSpace * index) + (recipeSpace / 2);

		iTween.ValueTo(recipe, iTween.Hash(
			"from", recipeTransform.anchoredPosition,
			"to", new Vector2(recipeX, recipeTransform.anchoredPosition.y),
			"time", time,
			"delay", delay,
			"onupdatetarget", recipe,
			"onupdate", "MoveRecipe"
		));
	}

	public static List<GameObject> ShuffleList(List<GameObject> aList)
	{
		System.Random _random = new System.Random();

		GameObject myGO;

		int n = aList.Count;
		for (int i = 0; i < n; i++)
		{
			// NextDouble returns a random number between 0 and 1.
			// ... It is equivalent to Math.random() in Java.
			int r = i + (int)(_random.NextDouble() * (n - i));
			myGO = aList[r];
			aList[r] = aList[i];
			aList[i] = myGO;
		}

		return aList;
	}
}
