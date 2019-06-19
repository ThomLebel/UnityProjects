using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UseItemPotion : MonoBehaviour
{
	public Transform itemLocation;
	[SerializeField]
	private GameObject itemHolded;

	private PlayerInfo playerInfo;
	private PlayerPotion playerAction;
	[SerializeField]
	private SpriteRenderer[] pictoList;

	private int targetID, itemID;

	private void Start()
	{
		playerInfo = gameObject.GetComponent<PlayerInfo>();
		playerAction = gameObject.GetComponent<PlayerPotion>();
	}

	//Pick-up item !
	public void PickItem()
	{
		Collider2D target = GetClosestCollider(playerAction.pickupTags);

		if (target != null)
		{
			if (CheckPlayerDirection(target))
			{
				Debug.Log("On regarde dans la bonne direction !");
				if (target.tag != "dispenser")
				{
					PickUp(target.gameObject);
				}
				else
				{
					target.GetComponent<ItemDispenserScript>().GiveItem(gameObject);
				}
			}
		}
	}

	//What to do with this object ?
	public void DropItem()
	{
		Debug.Log("trying to drop : " + itemHolded.name);

		string[] tagList;

		if (itemHolded.tag == "item")
			tagList = new string[] { "chaudron", "craftTable", "trashcan" };
		else if (itemHolded.tag == "chaudron")
			tagList = new string[] { "fiole", "fire", "item", "trashcan" };
		else if (itemHolded.tag == "fiole")
			tagList = new string[] { "pnj", "chaudron", "trashcan" };
		else
			tagList = new string[] { "" };

		Collider2D target = GetClosestCollider(tagList);

		Debug.Log("Closest target : "+target);
		
		if (target != null)
		{
			Debug.Log("Closest target : " + target.tag);

			if (CheckPlayerDirection(target))
			{
				if (itemHolded.tag == "item")
				{
					if (target.tag == "craftTable")
					{
						DropOnSupport(target.gameObject);
					}
					else if (target.tag == "trashcan")
					{
						TrashContent();
					}
					else
					{
						if (!itemHolded.GetComponent<IngredientScript>().isPrepared)
						{
							AddItemToCauldron(target.gameObject, itemHolded.gameObject);
						}
						else
						{
							DropOff();
						}
					}
				}
				else if (itemHolded.tag == "chaudron")
				{
					if (target.tag == "fire")
					{
						DropOnSupport(target.gameObject);
					}
					else if (target.tag == "fiole")
					{
						SwitchContent(target.gameObject, itemHolded.gameObject);
					}
					else if (target.tag == "trashcan")
					{
						TrashContent();
					}
					else
					{
						if (!target.GetComponent<IngredientScript>().isPrepared)
						{
							AddItemToCauldron(itemHolded.gameObject, target.gameObject);
						}
						else
						{
							DropOff();
						}
					}
				}
				else if (itemHolded.tag == "fiole")
				{
					if (target.tag == "pnj")
					{
						ServePotion(target.gameObject, itemHolded.gameObject);
					}
					else if (target.tag == "trashcan")
					{
						TrashContent();
					}
					else
					{
						SwitchContent(target.gameObject, itemHolded.gameObject);
					}
				}
			}
		}
		else
		{
			DropOff();
		}
	}

	//Prepare ingredient
	public void PrepareItem()
	{
		//Collider2D target = GetClosestCollider(playerInfo.craftTableLayer);
		Collider2D target = GetClosestCollider(new string[] { "craftTable" });

		if (target != null)
		{
			SupportScript supportScript = target.GetComponent<SupportScript>();

			if (!supportScript.isOccupied)
			{
				return;
			}

			IngredientScript ingredientScript = target.GetComponentInChildren<IngredientScript>();
			if (target.GetComponentInChildren<SupportableScript>().onSupport)
			{
				if (CheckPlayerDirection(target))
				{
					if (playerAction.isPreparing)
					{
						ingredientScript.PrepareIngredient();
					}
				}
			}
		}
	}


	public void SwitchContent(GameObject pTarget, GameObject pItem)
	{
		FillableScript targetFillableScript = pTarget.GetComponent<FillableScript>();
		FillableScript itemFillableScript = pItem.GetComponent<FillableScript>();
		ChaudronScript chaudronScript;
		FioleScript fioleScript;

		if (pItem.tag == "chaudron")
		{
			chaudronScript = pItem.GetComponent<ChaudronScript>();
			fioleScript = pTarget.GetComponent<FioleScript>();
		}
		else
		{
			chaudronScript = pTarget.GetComponent<ChaudronScript>();
			fioleScript = pItem.GetComponent<FioleScript>();
		}
			

		Debug.Log("target : " + pTarget.tag + "; item Count : " + targetFillableScript.itemList.Count + " && item : " + pItem.tag + "; item Count : " + itemFillableScript.itemList.Count);

		if (targetFillableScript.itemList.Count == 0 && itemFillableScript.itemList.Count != 0)
		{
			if (pTarget.tag == "chaudron")
			{
				Debug.Log("On tient une potion pleine et on la verse dans le chaudron vide");
				for (int i = 0; i < itemFillableScript.itemList.Count; i++)
				{
					chaudronScript.AddItem(itemFillableScript.itemList[i], itemFillableScript.pictoList[i].sprite);
					itemFillableScript.pictoList[i].sprite = null;
				}
				fioleScript.EmptyPotion();
				chaudronScript.isDone = true;
				chaudronScript.SetCookingTime(1f);
				itemFillableScript.itemList = new List<string>();
				RemovePicto(pItem);
			}
			else
			{
				if (chaudronScript.isDone && !chaudronScript.isBurning)
				{
					Debug.Log("On tient un chaudron plein et on le verse dans la potion vide");
					targetFillableScript.itemList = itemFillableScript.itemList;

					for (int i = 0; i < itemFillableScript.pictoList.Length; i++)
					{
						targetFillableScript.pictoList[i].sprite = itemFillableScript.pictoList[i].sprite;
					}
					RemovePicto(pItem);
					chaudronScript.Empty();
					fioleScript.SetPotionVisual();
				}
			}
		}
		else if (itemFillableScript.itemList.Count == 0 && targetFillableScript.itemList.Count != 0)
		{
			if (pTarget.tag == "chaudron")
			{
				if (chaudronScript.isDone && !chaudronScript.isBurning)
				{
					Debug.Log("On tient une potion vide et on la remplit au chaudron");
					itemFillableScript.itemList = targetFillableScript.itemList;

					for (int i = 0; i < itemFillableScript.pictoList.Length; i++)
					{
						itemFillableScript.pictoList[i].sprite = targetFillableScript.pictoList[i].sprite;
					}

					AddPicto(itemHolded);
					chaudronScript.Empty();
					fioleScript.SetPotionVisual();
				}
			}
			else
			{
				Debug.Log("On tient un chaudron vide et on le remplit à la potion");
				for (int i = 0; i < targetFillableScript.itemList.Count; i++)
				{
					chaudronScript.AddItem(targetFillableScript.itemList[i], targetFillableScript.pictoList[i].sprite);
					targetFillableScript.pictoList[i].sprite = null;
				}
				fioleScript.EmptyPotion();
				chaudronScript.isDone = true;
				chaudronScript.SetCookingTime(1f);
				targetFillableScript.itemList = new List<string>();

				AddPicto(itemHolded);
			}
		}
	}

	public void ServePotion(GameObject pTarget, GameObject pItem)
	{
		PotionMasterScript potionMasterScript = pTarget.GetComponent<PotionMasterScript>();
		int score = potionMasterScript.CheckPotionValidity(gameObject, pItem.GetComponent<FillableScript>().itemList);

		RemovePicto(pItem);
		Destroy(pItem.gameObject);

		playerAction.isHolding = false;
		itemHolded = null;
		
		playerInfo.potionScore += score;
		//playerInfo.gameManager.GetComponent<PotionManager>().SetPlayerScore(playerInfo.playerTeam - 1, playerInfo.potionScore);
		PotionManager.Instance.SetPlayerScore(playerInfo.playerTeam -1, playerInfo.potionScore);

		if (score > 0)
		{
			//Play happy animation
			Debug.Log("Congrats ! You delivered a good potion");
		}
		else
		{
			//Play sad animation
			Debug.Log("This potion isn't good, try again !");
		}
	}

	public void AddItemToCauldron(GameObject pTarget, GameObject pItem)
	{
		Debug.Log("Adding to cauldron " + pItem.name);

		ChaudronScript chaudronScript = pTarget.GetComponent<ChaudronScript>();
		IngredientScript itemScript = pItem.GetComponent<IngredientScript>();
		SupportableScript supportableScript = pItem.GetComponent<SupportableScript>();

		string itemName = itemScript.itemName;

		if (!chaudronScript.isBurning && !chaudronScript.isFull && !itemScript.isPrepared)
		{
			chaudronScript.AddItem(itemName, pItem.GetComponentInChildren<SpriteRenderer>().sprite);

			if (itemHolded.tag == "item")
			{
				playerAction.isHolding = false;
			}
			else if (itemHolded.tag == "chaudron")
			{
				if (supportableScript.support != null)
				{
					GameObject craftingTable = supportableScript.support;
					craftingTable.GetComponent<SupportScript>().isOccupied = false;
					supportableScript.support = null;
				}
				AddPicto(itemHolded);
				Destroy(pItem);
			}
			
			if (itemHolded.tag != "chaudron")
			{
				itemHolded = null;
				Destroy(pItem);
			}
		}
	}

	public void PickUp(GameObject pItem)
	{
		Debug.Log("Picking up " + pItem.name);

		itemHolded = pItem;

		if (pItem.tag == "chaudron" || pItem.tag == "item")
		{
			SupportableScript supportableScript = pItem.GetComponent<SupportableScript>();
			supportableScript.onSupport = false;

			if (pItem.tag == "chaudron")
			{
				pItem.GetComponent<ChaudronScript>().isCooking = false;
			}
			else if (pItem.tag == "item")
			{
				pItem.GetComponent<IngredientScript>().progressBarScript.ToggleVisibility(false);
				pItem.GetComponentInChildren<Animator>().enabled = false;
			}
			if (supportableScript.support != null)
			{
				supportableScript.support.GetComponent<SupportScript>().isOccupied = false;
			}
		}

		if (pItem.tag == "chaudron" || pItem.tag == "fiole")
		{
			AddPicto(pItem);
		}

		pItem.GetComponent<TransportableScript>().isHold = true;
		pItem.GetComponent<Rigidbody2D>().isKinematic = true;
		BoxCollider2D[] _colliders = pItem.GetComponentsInChildren<BoxCollider2D>();
		foreach (BoxCollider2D collider in _colliders)
		{
			collider.enabled = false;
		}

		pItem.transform.parent = itemLocation;
		pItem.transform.localPosition = new Vector3(0,0,0);
		pItem.transform.localRotation = new Quaternion(0, 0, 0, 0);

		playerAction.isHolding = true;
	}

	public void DropOff()
	{
		if (playerAction.isHolding)
		{
			if (itemHolded != null)
			{
				Debug.Log("Droping on the floor " + itemHolded.name);

				if (itemHolded.tag == "chaudron" || itemHolded.tag == "fiole")
				{
					RemovePicto(itemHolded);
				}

				float itemHeight = itemHolded.GetComponentInChildren<Renderer>().bounds.size.y;
				float yPos = transform.position.y + itemHeight;

				itemHolded.transform.parent = null;
				itemHolded.transform.rotation = Quaternion.Euler(0,0,0);

				//Cast a ray to detect closest floor to drop the item on
				RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, playerAction.groundLayerMask);
				if (hit.collider != null)
				{
					yPos = hit.transform.position.y + hit.transform.GetComponentInChildren<Renderer>().bounds.size.y + itemHeight + 0.3f;
				}

				itemHolded.transform.position = new Vector3(transform.position.x, yPos, transform.position.z);

				itemHolded.GetComponent<TransportableScript>().isHold = false;

				if(itemHolded.tag == "item" || itemHolded.tag == "fiole")
					itemHolded.GetComponent<Rigidbody2D>().isKinematic = false;

				BoxCollider2D[] _colliders = itemHolded.GetComponentsInChildren<BoxCollider2D>();
				foreach (BoxCollider2D collider in _colliders)
				{
					collider.enabled = true;
				}

				if (itemHolded.tag == "item")
				{
					itemHolded.GetComponentInChildren<Animator>().enabled = true;
				}

				playerAction.isHolding = false;
				itemHolded = null;
			}
		}
	}

	public void DropOnSupport(GameObject pTarget)
	{
		if (playerAction.isHolding)
		{
			if (itemHolded != null)
			{
				SupportScript targetScript = pTarget.GetComponent<SupportScript>();

				if (!targetScript.isOccupied)
				{
					Debug.Log("target name : " + pTarget.name);

					itemHolded.transform.parent = pTarget.transform.GetChild(0);
					itemHolded.transform.position = pTarget.transform.GetChild(0).position;
					itemHolded.transform.rotation = new Quaternion(0, 0, 0, 0);

					itemHolded.GetComponent<TransportableScript>().isHold = false;

					BoxCollider2D[] _colliders = itemHolded.GetComponentsInChildren<BoxCollider2D>();
					foreach (BoxCollider2D collider in _colliders)
					{
						collider.enabled = true;
					}
					SupportableScript supportableScript = itemHolded.GetComponent<SupportableScript>();

					if (itemHolded.tag == "chaudron")
					{
						itemHolded.GetComponent<ChaudronScript>().isCooking = true;
						itemHolded.GetComponent<ChaudronScript>().progressBarScript.ToggleVisibility(true);
						RemovePicto(itemHolded);
					}
					else
					{
						itemHolded.GetComponent<IngredientScript>().progressBarScript.ToggleVisibility(true);
					}

					supportableScript.onSupport = true;
					supportableScript.support = pTarget;
					targetScript.isOccupied = true;
					playerAction.isHolding = false;
					itemHolded = null;
				}
				else
				{
					DropOff();
				}
			}
		}
	}

	public void TrashContent()
	{
		if (itemHolded.tag == "chaudron")
		{
			RemovePicto(itemHolded);
			itemHolded.GetComponent<ChaudronScript>().Empty();
		}
		else if (itemHolded.tag == "fiole")
		{
			RemovePicto(itemHolded);
			Destroy(itemHolded);
			playerAction.isHolding = false;
			itemHolded = null;
		}
		else
		{
			Destroy(itemHolded);
			playerAction.isHolding = false;
			itemHolded = null;
		}
	}

	public void AddPicto(GameObject pItem)
	{
		FillableScript itemInfo = pItem.GetComponent<FillableScript>();

		if (itemInfo.itemList.Count > 0)
		{
			for (int i = 0; i < itemInfo.pictoList.Length; i++)
			{
				pictoList[i].sprite = itemInfo.pictoList[i].sprite;
				itemInfo.pictoList[i].enabled = false;
			}
		}
	}

	public void RemovePicto(GameObject pItem)
	{
		FillableScript itemInfo = pItem.GetComponent<FillableScript>();

		for (int i = 0; i < itemInfo.pictoList.Length; i++)
		{
			pictoList[i].sprite = null;
			itemInfo.pictoList[i].enabled = true;
		}
	}

	private Collider2D GetClosestCollider(String[] tagList)
	{
		float dist = 1000;
		Collider2D target = null;

		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, playerAction.pickUpRange);
		if (colliders.Length > 0)
		{
			for (int i = 0; i < colliders.Length; i++)
			{
				Collider2D item = colliders[i];

				//Check if this object has the tag needed
				for (int t = 0; t < tagList.Length; t++)
				{
					if (item.tag == tagList[t])
					{
						float distTemp = Math.Abs(item.transform.position.x - transform.position.x);
						if (distTemp < dist)
						{
							dist = distTemp;
							target = item;
						}
					}
				}
			}
		}

		return target;
	}

	private bool CheckPlayerDirection(Collider2D pTarget)
	{
		//Horizontal
		if (playerAction.lastDir.x > 0)
		{
			if (pTarget.transform.position.x >= transform.position.x)
			{
				return true;
			}
		}
		else if(playerAction.lastDir.x < 0)
		{
			if (pTarget.transform.position.x <= transform.position.x)
			{
				return true;
			}
		}
		//Vertical
		else if (playerAction.lastDir.y > 0)
		{
			if (pTarget.transform.position.y >= transform.position.y)
			{
				return true;
			}
		}
		else if (playerAction.lastDir.y < 0)
		{
			if (pTarget.transform.position.y <= transform.position.y)
			{
				return true;
			}
		}

		return false;
	}
}