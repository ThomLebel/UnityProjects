using Com.OniriqueStudio.MagicSchool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{

	//States
	//[HideInInspector]
	public Transform itemLocation;
	public GameObject projectilePrefab;
	public GameObject bubblePrefab;
	public bool isHolding, isStun, isPreparing, isProtected, canMove;
	public string State;

	//Mesures
	[HideInInspector]
	public float playerWidth, playerHeight, originalScale;
	public float itemOffset = 0.4f;

	//Layers
	[HideInInspector]
	public string[] pickupTags;

	[Tooltip("Numero du contrôleur du joueur")]
	public int playerController;
	[Tooltip("Numero du joueur")]
	public int playerID;
	[Tooltip("Sprite du joueur")]
	public Sprite playerSprite;
	[Tooltip("Numero de l'équipe du joueur")]
	public int playerTeam;

	public float pickUpRange = 0.5f;
	
	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
	}

	private void Start()
	{
		State = "idle";
		isHolding = false;
		isStun = false;
		isPreparing = false;
		isProtected = false;
		canMove = true;
		playerWidth = GetComponentInChildren<Renderer>().bounds.size.x;
		playerHeight = GetComponentInChildren<Renderer>().bounds.size.y;
		originalScale = spriteRenderer.transform.localScale.x;

		Debug.Log("PLAYERINFO // Original scale = "+originalScale);
		
		pickupTags = new string[] { "item", "chaudron", "dispenser", "fiole" };
	}

	public void ConfigurePlayer(int pID, int pController, int pSpriteID)
	{
		Debug.Log("Configuration du joueur");
		string playerSpriteName = CharacterSelector.Instance.spriteList[pSpriteID].name;
		playerID = pID;
		playerController = pController;
		playerSprite = CharacterSelector.Instance.spriteList[pSpriteID];
		spriteRenderer.sprite = playerSprite;
		spriteRenderer.enabled = false;
		Animator animator = GetComponentInChildren<Animator>();
		animator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Potion/Animations/"+playerSpriteName+"_animations/"+ playerSpriteName + "_controller", typeof(RuntimeAnimatorController));

		//bubblePrefab.GetComponentInChildren<SpriteRenderer>().sprite = ;
		Animator bubbleAnimator = bubblePrefab.GetComponentInChildren<Animator>();
		bubbleAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Potion/Animations/" + playerSpriteName + "_animations/" + playerSpriteName + "_bubble", typeof(RuntimeAnimatorController));

		projectilePrefab = Resources.Load<GameObject>("Potion/Prefab/"+playerSpriteName+"_spell");

		gameObject.GetComponent<PlayerPotion>().enabled = false;
		bubbleAnimator.enabled = false;
	}

	public void Init(Vector3 pPos)
	{
		spriteRenderer.enabled = true;
		gameObject.GetComponent<PlayerPotion>().enabled = true;
		transform.position = pPos;
	}
}
