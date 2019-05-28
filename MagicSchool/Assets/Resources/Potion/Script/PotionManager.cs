using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionManager : MonoBehaviour {

	public float indicationSpace = 10f;

	public List<GameObject> ingredientsList;

	public List<Transform> SpawnPositionsList;

	public List<List<string>> recipesList = new List<List<string>>();

	[SerializeField]
	private Image recipePrefab;
	[SerializeField]
	private GameObject canvas;

	private float recipeWidth;
	private float recipeHeight;

	static public PotionManager Instance;

	// Use this for initialization
	void Start () {
		canvas = GameObject.FindGameObjectWithTag("canvas");

		GameObject[] _players =  GameObject.FindGameObjectsWithTag("Player");
		int i = 0;
		foreach (GameObject player in _players)
		{
			Debug.Log("Init a player : "+player);

			player.GetComponent<PlayerInfo>().Init(SpawnPositionsList[i].position);

			i++;
		}
		
		recipeWidth = recipePrefab.GetComponent<RectTransform>().rect.width;
		recipeHeight = recipePrefab.GetComponent<RectTransform>().rect.height;


		GenerateRecipe();
	}

	public void GenerateRecipe()
	{
		//List of all 4 basic ingredients shuffled
		List<GameObject> tempIngredientList = ShuffleList(ingredientsList);

		//List containing the recipe ingredients
		List<string> recipeIngredient = new List<string>();

		//Prefab of the recipe which will be drawn on screen
		Image recipe = Instantiate(recipePrefab);
		recipe.GetComponent<RectTransform>().SetParent(canvas.transform);
		recipe.GetComponent<RectTransform>().localPosition = new Vector3(0f,0f,0f);
		
		int randIngredientQuantity = 4;
		//int randIngredientQuantity = Random.Range(2, 4);
		float ingredientSpace = recipeHeight / randIngredientQuantity;
		
		for (int i = 0; i < randIngredientQuantity; i++)
		{
			GameObject ingredient = tempIngredientList[i];
			ItemScript itemScript = ingredient.GetComponent<ItemScript>();
			ItemInfoScript infoScript = ingredient.GetComponent<ItemInfoScript>();

			GameObject ingredientElement = new GameObject();
			Image ingredientSprite = ingredientElement.AddComponent<Image>();
			ingredientElement.GetComponent<RectTransform>().SetParent(recipe.transform);

			bool prepared = (Random.value > 0.5f);
			if (prepared)
			{
				recipeIngredient.Add(itemScript.prepName);
				ingredientSprite.sprite = itemScript.prepSprite;
			}
			else
			{
				recipeIngredient.Add(infoScript.itemName);
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
				indicationElement.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
				float ingredientWidth = ingredientElement.GetComponent<RectTransform>().rect.width * 0.5f;
				float indicationWidth = indicationElement.GetComponent<RectTransform>().rect.width * 0.2f;
				float posX = (ingredientWidth / 2 + indicationWidth / 2 + indicationSpace) * -1;

				//float ingredientXSpace = recipeWidth / 4;
				//xPos = (recipeWidth / 2) - (ingredientXSpace * 2) - (ingredientXSpace / 2);

				indicationElement.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, yPos);

				//xPos = (recipeWidth / 2) - ingredientXSpace - (ingredientXSpace / 2);
			}

			ingredientElement.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
		}

		recipesList.Add(recipeIngredient);
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
