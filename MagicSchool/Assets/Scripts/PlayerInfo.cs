using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour {

	public int playerNumber;
	public bool isHolding;
	public bool isStun;

	public float playerWidth, playerHeight;
	public float originalScale;

	public int itemLayer, chaudronLayer, dispenserLayer, fireLayer;
	public int pickUpLayer;
	public float pickUpRange = 0.5f;

	private void Start()
	{
		isHolding = false;
		isStun = false;
		playerWidth = GetComponent<Renderer>().bounds.size.x;
		playerHeight = GetComponent<Renderer>().bounds.size.y;
		originalScale = transform.localScale.x;

		itemLayer = 1 << LayerMask.NameToLayer("item");
		chaudronLayer = 1 << LayerMask.NameToLayer("chaudron");
		dispenserLayer = 1 << LayerMask.NameToLayer("dispenser");
		fireLayer = 1 << LayerMask.NameToLayer("fire");
		pickUpLayer = itemLayer | chaudronLayer | dispenserLayer;
	}
}
