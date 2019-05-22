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
	public Transform groundCheck;
	public GameObject projectilePrefab;
	public GameObject bubblePrefab;
	public bool isHolding, isStun, isPreparing, isProtected, canMove;
	public string State;


	[HideInInspector]
	public Rigidbody2D rb2d;

	//Mesures
	[HideInInspector]
	public float playerWidth, playerHeight, originalScale;
	public float itemOffset = 0.4f;

	//Layers
	[HideInInspector]
	public string[] pickupTags;
	public LayerMask groundLayerMask;
	public LayerMask jumpThroughLayerMask;

	[Tooltip("Numero du contrôleur du joueur")]
	public int playerController;
	[Tooltip("Numero du joueur")]
	public int playerID;
	[Tooltip("Sprite du joueur")]
	public Sprite playerSprite;
	[Tooltip("Numero de l'équipe du joueur")]
	public int playerTeam;

	public float pickUpRange = 0.5f;
	
	public GameObject playerBody;

	private void Awake()
	{
		playerBody = gameObject.transform.Find("base_wizard").gameObject;
		//spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
		rb2d = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		State = "idle";
		isHolding = false;
		isStun = false;
		isPreparing = false;
		isProtected = false;
		canMove = true;
		//playerWidth = GetComponentInChildren<Renderer>().bounds.size.x;
		//playerHeight = GetComponentInChildren<Renderer>().bounds.size.y;
		originalScale = playerBody.transform.localScale.x;

		Debug.Log("PLAYERINFO // Original scale = "+originalScale);
		
		pickupTags = new string[] { "item", "chaudron", "dispenser", "fiole" };

		jumpThroughLayerMask = LayerMask.GetMask("jumpThrough");
	}

	public void ConfigurePlayer(int pID, int pController, int pSpriteID)
	{
		Debug.Log("Configuration du joueur");
		string playerSpriteName = CharacterSelector.Instance.spriteList[pSpriteID].name;
		playerID = pID;
		playerController = pController;

		string spriteSheetName = playerSpriteName + "_wizard_spritesheet";
		Sprite[] spriteSheet = Resources.LoadAll<Sprite>("Sprites/Wizards/"+ spriteSheetName);
		SpriteRenderer[] playerSprites = GetComponentsInChildren<SpriteRenderer>();

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
		
		Animator bubbleAnimator = bubblePrefab.GetComponentInChildren<Animator>();
		bubbleAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Potion/Animations/" + playerSpriteName + "_animations/" + playerSpriteName + "_bubble", typeof(RuntimeAnimatorController));

		projectilePrefab = Resources.Load<GameObject>("Potion/Prefab/"+playerSpriteName+"_spell");

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
	}
}
