using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour {

	public string itemName;
	public bool isHold;

	private void Start()
	{
		isHold = false;
	}
}
