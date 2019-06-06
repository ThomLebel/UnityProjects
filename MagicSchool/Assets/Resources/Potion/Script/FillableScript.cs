using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillableScript : MonoBehaviour
{
	public int maxItem = 4;
	public List<string> itemList;
	public SpriteRenderer[] pictoList;

	private void Start()
	{
		itemList = new List<string>();
	}
}
