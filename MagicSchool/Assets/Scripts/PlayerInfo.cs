using Com.OniriqueStudio.MagicSchool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : Photon.PunBehaviour, IPunObservable
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
	[Tooltip("ID du pc sur le network")]
	public int networkID;
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

	[PunRPC]
	public void ConfigurePlayer(int pID, int pController, int pNetworkID, int pSpriteID)
	{
		Debug.Log("Configuration du joueur");
		playerID = pID;
		playerController = pController;
		networkID = pNetworkID;
		playerSprite = CharacterSelector.Instance.spriteList[pSpriteID];
		spriteRenderer.sprite = playerSprite;
		spriteRenderer.enabled = false;
		gameObject.GetComponent<PlayerNetwork>().enabled = false;

		if (PhotonNetwork.connected && PhotonNetwork.player.ID != pNetworkID)
		{
			photonView.TransferOwnership(pNetworkID);
		}
	}

	[PunRPC]
	public void Init(Vector3 pPos)
	{
		spriteRenderer.enabled = true;
		gameObject.GetComponent<PlayerNetwork>().enabled = true;
		transform.position = pPos;
	}

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(isHolding);
			stream.SendNext(isStun);
			stream.SendNext(isPreparing);
			stream.SendNext(itemLocation.localPosition);
			stream.SendNext(spriteRenderer.transform.localScale);
		}
		else
		{
			// Network player, receive data
			this.isHolding = (bool)stream.ReceiveNext();
			this.isStun = (bool)stream.ReceiveNext();
			this.isPreparing = (bool)stream.ReceiveNext();
			this.itemLocation.localPosition = (Vector3)stream.ReceiveNext();
			this.spriteRenderer.transform.localScale = (Vector3)stream.ReceiveNext();
		}
	}
}
