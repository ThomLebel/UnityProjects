using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoScript : MonoBehaviour
{

	public string itemName;
	public int maxItem = 3;
	public List<string> itemList;
	public SpriteRenderer[] pictoList;


	public bool isHold;
	public bool isOccupied;

	private void Start()
	{
		isHold = false;
		isOccupied = false;
		//itemList = new string[maxItem];
	}

	private void Update()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, 0);
	}
}
