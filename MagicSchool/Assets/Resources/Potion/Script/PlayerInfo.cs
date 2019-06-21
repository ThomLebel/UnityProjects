using Com.OniriqueStudio.MagicSchool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerInfo : MonoBehaviour
{
	//States
	//[HideInInspector]
	public string State;

	[HideInInspector]
	public Rigidbody2D rb2d;

	//Mesures
	[HideInInspector]
	public float playerWidth, playerHeight, originalScale;
	public float itemOffset = 0.4f;

	[Tooltip("Nom du joueur")]
	public string playerName;
	[Tooltip("Numero du contrôleur du joueur")]
	public int playerController;
	[Tooltip("Numero du joueur")]
	public int playerID;
	[Tooltip("Sprite du joueur")]
	public Sprite playerSprite;
	[Tooltip("Numero de l'équipe du joueur")]
	public int playerTeam;

	public float score = 0;
	public float potionScore = 0;

	public GameObject playerBody;

	public GameObject gameManager;
	

	private void Awake()
	{
		DontDestroyOnLoad(this.gameObject);

		playerBody = gameObject.transform.Find("base_wizard").gameObject;
		rb2d = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		State = "idle";
		//playerWidth = GetComponentInChildren<Renderer>().bounds.size.x;
		//playerHeight = GetComponentInChildren<Renderer>().bounds.size.y;
		originalScale = playerBody.transform.localScale.x;

		Debug.Log("PLAYERINFO // Original scale = "+originalScale);
	}

	public void ConfigurePlayer(string pName, int pID, int pController, int pTeam, Sprite pSprite)
	{
		Debug.Log("Configuration du joueur");
		string playerSpriteName = pSprite.name;
		playerName = pName;
		playerID = pID;
		playerController = pController;
		playerTeam = pTeam;

		string spriteSheetName = playerSpriteName + "_wizard_spritesheet";
		Sprite[] spriteSheet = Resources.LoadAll<Sprite>("Sprites/Wizards/"+ spriteSheetName);
		SpriteRenderer[] playerSprites = playerBody.GetComponentsInChildren<SpriteRenderer>();

		for (int i = 0; i < playerSprites.Length; i++)
		{
			//Get the original sprite name
			string spriteName = playerSprites[i].sprite.name;
			//Init the new sprite index in the spritesheet
			int spriteIndex = 0;
			//Get the sprite index from the original sprite name (last digit : 0 to 11)
			Int32.TryParse(Regex.Match(spriteName, @"\d+").Value, out spriteIndex);
			//Replace the orignal sprite whith the new one
			playerSprites[i].sprite = spriteSheet[spriteIndex];
		}
		
		Animator bubbleAnimator = gameObject.GetComponent<PlayerPotion>().bubblePrefab.GetComponentInChildren<Animator>();
		bubbleAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Potion/Animations/" + playerSpriteName + "_animations/" + playerSpriteName + "_bubble", typeof(RuntimeAnimatorController));

		gameObject.GetComponent<PlayerPotion>().projectilePrefab = Resources.Load<GameObject>("Potion/Prefab/"+playerSpriteName+"_spell");

		gameObject.GetComponent<PlayerPotion>().enabled = false;
		bubbleAnimator.enabled = false;
		rb2d.isKinematic = true;
		GetComponent<SortingGroup>().sortingOrder = pID;
		playerBody.SetActive(false);
	}

	public void Init(Vector3 pPos)
	{
		playerBody.SetActive(true);
		gameObject.GetComponent<PlayerPotion>().enabled = true;
		transform.position = pPos;
		rb2d.isKinematic = false;
		gameManager = GameObject.FindWithTag("GameManager");
	}
}
