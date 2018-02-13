using Com.MyCompany.MyGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : Photon.PunBehaviour
{

	//States
	[HideInInspector]
	public bool isHolding, isStun;
	public string State;

	//Mesures
	[HideInInspector]
	public float playerWidth, playerHeight, originalScale;

	//Layers
	[HideInInspector]
	public int itemLayer, chaudronLayer, dispenserLayer, fireLayer, pickUpLayer, pnjLayer;

	[Tooltip("Numero du contrôleur du joueur")]
	public int playerController;
	[Tooltip("Numero du joueur")]
	public int playerID;
	[Tooltip("ID du pc sur le network")]
	public int networkID;
	public Sprite playerSprite;

	public float pickUpRange = 0.5f;

	private void Start()
	{
		State = "idle";
		isHolding = false;
		isStun = false;
		playerWidth = GetComponent<Renderer>().bounds.size.x;
		playerHeight = GetComponent<Renderer>().bounds.size.y;
		originalScale = transform.localScale.x;

		itemLayer = 1 << LayerMask.NameToLayer("item");
		chaudronLayer = 1 << LayerMask.NameToLayer("chaudron");
		dispenserLayer = 1 << LayerMask.NameToLayer("dispenser");
		fireLayer = 1 << LayerMask.NameToLayer("fire");
		pnjLayer = 1 << LayerMask.NameToLayer("pnj");
		pickUpLayer = itemLayer | chaudronLayer | dispenserLayer;
	}

	[PunRPC]
	public void ConfigurePlayer(int pID, int pController, int pNetworkID, int pSpriteID)
	{
		playerID = pID;
		playerController = pController;
		networkID = pNetworkID;
		playerSprite = CharacterSelector.Instance.spriteList[pSpriteID];
		gameObject.GetComponent<SpriteRenderer>().sprite = playerSprite;
		gameObject.GetComponent<SpriteRenderer>().enabled = false;
		gameObject.GetComponent<PlayerNetwork>().enabled = false;

		if (PhotonNetwork.connected && PhotonNetwork.player.ID != pNetworkID)
		{
			photonView.TransferOwnership(pNetworkID);
		}
	}

	[PunRPC]
	public void Init()
	{
		gameObject.GetComponent<SpriteRenderer>().enabled = true;
		gameObject.GetComponent<PlayerNetwork>().enabled = true;
	}
}
