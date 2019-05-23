using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour {

	public float spellForce = 30f;
	public float preparingCoef = 0.2f;

	public bool isPrepared = false;
	public bool isDone = false;
	public bool onCraftingTable = false;
	public GameObject craftingTable;

	private bool moving = false;
	private Rigidbody2D rb2d;
	private ProgressBarScript _progressBarScript;
	private ItemInfoScript _itemInfo;

	private void Awake()
	{
		rb2d = gameObject.GetComponent<Rigidbody2D>();
		_progressBarScript = gameObject.GetComponentInChildren<ProgressBarScript>();
	}

	void Start()
	{
		//_progressBarScript.ToggleVisibility(false);
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
		if (isDone)
		{
			return;
		}
		if (_progressBarScript.value < 1)
			_progressBarScript.value += Time.deltaTime * preparingCoef;
		else
		{
			_progressBarScript.value = 1;
			if (!isDone)
			{
				isDone = true;
			}
		}
	}
}
