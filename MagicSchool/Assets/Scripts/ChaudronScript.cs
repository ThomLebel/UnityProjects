using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaudronScript : MonoBehaviour {

	private ItemInfo _itemInfo;

	public int itemNeeded = 3;
	public bool isFull;
	public bool isCooking;

	public List<string> itemList;

	// Use this for initialization
	void Start () {
		_itemInfo = gameObject.GetComponent<ItemInfo>();
		isFull = false;
		isCooking = false;
	}

	public void AddItem(string pName)
	{
		if(itemList.Count < itemNeeded)
		{
			itemList.Add(pName);
		}
		if(itemList.Count == itemNeeded)
		{
			isFull = true;
		}
	}

	public void Cooking()
	{

	}
	
}
