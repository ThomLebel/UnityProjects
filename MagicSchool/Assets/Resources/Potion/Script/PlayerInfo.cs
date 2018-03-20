using Com.OniriqueStudio.MagicSchool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{

	//States
	//[HideInInspector]
	public bool isHolding, isStun, isPreparing;
	public string State;

	//Mesures
	[HideInInspector]
	public float playerWidth, playerHeight, originalScale;
	public float itemOffset = 0.4f;

	//Layers
	[HideInInspector]
	public int itemLayer, chaudronLayer, dispenserLayer, fireLayer, pickUpLayer, pnjLayer, craftTableLayer;

	[Tooltip("Numero du contrôleur du joueur")]
	public int playerController;
	[Tooltip("Numero du joueur")]
	public int playerID;
	[Tooltip("Sprite du joueur")]
	public Sprite playerSprite;

	public float pickUpRange = 0.5f;

	public Transform itemLocation;
	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
		//itemLocation = gameObject.transform.GetChild(0);
	}

	private void Start()
	{
		State = "idle";
		isHolding = false;
		isStun = false;
		isPreparing = false;
		playerWidth = GetComponentInChildren<Renderer>().bounds.size.x;
		playerHeight = GetComponentInChildren<Renderer>().bounds.size.y;
		originalScale = spriteRenderer.transform.localScale.x;

		Debug.Log("PLAYERINFO // Original scale = "+originalScale);

		itemLayer = 1 << LayerMask.NameToLayer("item");
		chaudronLayer = 1 << LayerMask.NameToLayer("chaudron");
		dispenserLayer = 1 << LayerMask.NameToLayer("dispenser");
		fireLayer = 1 << LayerMask.NameToLayer("fire");
		pnjLayer = 1 << LayerMask.NameToLayer("pnj");
		craftTableLayer = 1 << LayerMask.NameToLayer("craftTable");
		pickUpLayer = itemLayer | chaudronLayer | dispenserLayer;
	}

	public void ConfigurePlayer(int pID, int pController, int pSpriteID)
	{
		Debug.Log("Configuration du joueur");
		playerID = pID;
		playerController = pController;
		playerSprite = CharacterSelector.Instance.spriteList[pSpriteID];
		spriteRenderer.sprite = playerSprite;
		spriteRenderer.enabled = false;
		gameObject.GetComponent<PlayerPotion>().enabled = false;
	}

	public void Init(Vector3 pPos)
	{
		spriteRenderer.enabled = true;
		gameObject.GetComponent<PlayerPotion>().enabled = true;
		transform.position = pPos;
	}
}
