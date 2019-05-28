using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour {

	public float spellForce = 30f;
	public float preparingCoef = 0.2f;

	public bool isPrepared = false;
	public bool onCraftingTable = false;
	public GameObject craftingTable;
	
	public string prepName = "";
	public Sprite prepSprite;

	private bool moving = false;

	public ProgressBarScript _progressBarScript;

	private Rigidbody2D rb2d;
	private ItemInfoScript _itemInfo;

	private void Awake()
	{
		rb2d = gameObject.GetComponent<Rigidbody2D>();
		_progressBarScript = gameObject.GetComponentInChildren<ProgressBarScript>();
		_itemInfo = gameObject.GetComponentInChildren<ItemInfoScript>();
	}

	private void Update()
	{
		if (Math.Abs(rb2d.velocity.x) > 0)
		{
			moving = true;
		}
		else
		{
			moving = false;
		}
	}

	public virtual void AccioItem(Vector3 pDir)
	{
		rb2d.AddForce(transform.right * spellForce * pDir.x * -1);
	}

	void OnTriggerEnter2D(Collider2D pOther)
	{
		if (!moving)
		{
			return;
		}
		if (pOther.tag == "Player")
		{
			PlayerInfo playerInfoScript = pOther.GetComponent<PlayerInfo>();
			if (!playerInfoScript.isHolding && !playerInfoScript.isStun && !playerInfoScript.isPreparing)
			{
				rb2d.velocity = Vector2.zero;
				pOther.GetComponent<UseItemPotion>().PickUp(gameObject);
			}
		}
	}

	public virtual void PrepareIngredient()
	{
		if (_progressBarScript.value < 1)
		{
			_progressBarScript.value += Time.deltaTime * preparingCoef;

			if (_progressBarScript.value > 0)
				isPrepared = true;
		}
		else
		{
			_progressBarScript.value = 1;
			if (isPrepared)
			{
				//Change visuel and name
				isPrepared = false;
				_itemInfo.itemName = prepName;
				GetComponentInChildren<SpriteRenderer>().sprite = prepSprite;
			}
		}
	}
}
