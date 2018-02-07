using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour {

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

	public int playerNumber;
	public int playerID;
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
}
