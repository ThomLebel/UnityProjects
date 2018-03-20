using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UseItemPotion : MonoBehaviour
{
	private PlayerInfo _playerInfo;
	private PlayerPotion _playerAction;

	private int targetID, itemID;

	private void Start()
	{
		_playerInfo = gameObject.GetComponent<PlayerInfo>();
		_playerAction = gameObject.GetComponent<PlayerPotion>();
	}

	//Pick-up item !
	public void PickItem()
	{
		Collider2D target = GetClosestCollider(_playerInfo.pickUpLayer);

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
		Transform itemHolded = transform.GetChild(0).transform.GetChild(0);

		Debug.Log("trying to drop : "+itemHolded.name);

		int layerMask;

		if (itemHolded.tag == "item")
			layerMask = _playerInfo.chaudronLayer | _playerInfo.craftTableLayer;
		else if (itemHolded.tag == "chaudron")
			layerMask = _playerInfo.fireLayer | _playerInfo.itemLayer;
		else if (itemHolded.tag == "fiole")
			layerMask = _playerInfo.pnjLayer | _playerInfo.chaudronLayer;
		else
			layerMask = 0;

		Collider2D target = GetClosestCollider(layerMask);

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
					else
					{
						AddItemToCauldron(target.gameObject, itemHolded.gameObject);
					}
				}
				else if (itemHolded.tag == "chaudron")
				{
					if (target.tag == "fire")
					{
						DropOnSupport(target.gameObject);
					}
					else
					{
						SwitchContent(target.gameObject, itemHolded.gameObject);
					}
				}
				else if (itemHolded.tag == "fiole")
				{
					if (target.tag == "pnj")
					{
						ServePotion(target.gameObject, itemHolded.gameObject);
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
		Collider2D target = GetClosestCollider(_playerInfo.craftTableLayer);

		if (target != null)
		{
			ItemScript _itemScript = target.GetComponentInChildren<ItemScript>();
			if (!_itemScript.isDone && _itemScript.onCraftingTable)
			{
				if (CheckPlayerDirection(target))
				{
					if (_playerInfo.isPreparing)
					{
						_itemScript.PrepareIngredient();
					}
				}
			}
		}
	}


	public void SwitchContent(GameObject pTarget, GameObject pItem)
	{
		ItemInfoScript targetInfoScript = pTarget.GetComponent<ItemInfoScript>();
		ItemInfoScript itemInfoScript = pItem.GetComponent<ItemInfoScript>();
		ChaudronScript chaudronScript = pItem.GetComponent<ChaudronScript>();

		if (chaudronScript == null)
			chaudronScript = pTarget.GetComponent<ChaudronScript>();

		Debug.Log("target : " + targetInfoScript.itemName + "; item Count : " + targetInfoScript.itemList.Count + " && item : " + itemInfoScript.itemName + "; item Count : " + itemInfoScript.itemList.Count);

		if (targetInfoScript.itemList.Count == 0 && itemInfoScript.itemList.Count != 0)
		{
			if (pTarget.tag == "chaudron")
			{
				Debug.Log("On tient une potion pleine et on la verse dans le chaudron vide");
				for (int i = 0; i < itemInfoScript.itemList.Count; i++)
				{
					chaudronScript.AddItem(itemInfoScript.itemList[i]);
				}
				chaudronScript.isDone = true;
				chaudronScript.SetCookingTime(1f);
				itemInfoScript.itemList = new List<string>();

				for (int i = 0; i < itemInfoScript.pictoList.Length; i++)
				{
					itemInfoScript.pictoList[i].sprite = null;
				}
			}
			else
			{
				if (chaudronScript.isDone && !chaudronScript.isBurning)
				{
					Debug.Log("On tient un chaudron plein et on le verse dans la potion vide");
					targetInfoScript.itemList = itemInfoScript.itemList;
					chaudronScript.isFull = false;
					chaudronScript.isDone = false;
					chaudronScript.SetCookingTime(0f);
					itemInfoScript.itemList = new List<string>();

					for (int i = 0; i < itemInfoScript.pictoList.Length; i++)
					{
						targetInfoScript.pictoList[i].sprite = itemInfoScript.pictoList[i].sprite;
						itemInfoScript.pictoList[i].sprite = null;
					}
				}
			}
		}
		else if (itemInfoScript.itemList.Count == 0 && targetInfoScript.itemList.Count != 0)
		{
			if (pTarget.tag == "chaudron")
			{
				if (chaudronScript.isDone && !chaudronScript.isBurning)
				{
					Debug.Log("On tient une potion vide et on la remplit au chaudron");
					itemInfoScript.itemList = targetInfoScript.itemList;
					chaudronScript.isFull = false;
					chaudronScript.isDone = false;
					chaudronScript.SetCookingTime(0f);
					targetInfoScript.itemList = new List<string>();

					for (int i = 0; i < itemInfoScript.pictoList.Length; i++)
					{
						itemInfoScript.pictoList[i].sprite = targetInfoScript.pictoList[i].sprite;
						targetInfoScript.pictoList[i].sprite = null;
					}
				}
			}
			else
			{
				Debug.Log("On tient un chaudron vide et on le remplit à la potion");
				for (int i = 0; i < targetInfoScript.itemList.Count; i++)
				{
					chaudronScript.AddItem(targetInfoScript.itemList[i]);
				}
				chaudronScript.isDone = true;
				chaudronScript.SetCookingTime(1f);
				targetInfoScript.itemList = new List<string>();

				for (int i = 0; i < itemInfoScript.pictoList.Length; i++)
				{
					targetInfoScript.pictoList[i].sprite = null;
				}
			}
		}
	}

	public void ServePotion(GameObject pTarget, GameObject pItem)
	{
		PotionMasterScript potionMasterScript = pTarget.GetComponent<PotionMasterScript>();
		potionMasterScript.CheckPotionValidity(pItem.GetComponent<FioleScript>().itemList);

		Destroy(pItem.gameObject);

		_playerInfo.isHolding = false;
	}

	public void AddItemToCauldron(GameObject pTarget, GameObject pItem)
	{
		Debug.Log("Adding to cauldron " + pItem.name);

		ChaudronScript chaudronScript = pTarget.GetComponent<ChaudronScript>();
		ItemScript itemScript = pItem.GetComponent<ItemScript>();
		string itemName = pItem.GetComponent<ItemInfoScript>().itemName;

		if (!chaudronScript.isBurning && !chaudronScript.isFull && itemScript.isDone)
		{
			chaudronScript.AddItem(itemName);

			Destroy(pItem);

			_playerInfo.isHolding = false;
		}
	}

	public void PickUp(GameObject pItem)
	{
		Debug.Log("Picking up " + pItem.name);
		if (pItem.tag == "chaudron")
		{
			pItem.GetComponent<ChaudronScript>().isCooking = false;
		}
		if (pItem.tag == "item")
		{
			pItem.GetComponent<ItemScript>().isPrepared = false;
			pItem.GetComponent<ItemScript>().onCraftingTable = false;
		}
		if (pItem.transform.parent != null)
			pItem.transform.parent.parent.GetComponent<ItemInfoScript>().isOccupied = false;

		pItem.GetComponent<ItemInfoScript>().isHold = true;
		pItem.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

		BoxCollider2D[] _colliders = pItem.GetComponentsInChildren<BoxCollider2D>();
		foreach (BoxCollider2D collider in _colliders)
		{
			collider.enabled = false;
		}
		
		pItem.transform.position = _playerInfo.itemLocation.position;
		pItem.transform.parent = _playerInfo.itemLocation;
		pItem.transform.rotation = new Quaternion(0, 0, 0, 0);

		_playerInfo.isHolding = true;
	}

	public void DropOff()
	{
		if (_playerInfo.isHolding)
		{
			if (transform.GetChild(0).transform.childCount > 0)
			{
				Transform item = transform.GetChild(0).transform.GetChild(0);

				Debug.Log("Droping on the floor " + item.name);

				item.parent = null;

				item.GetComponent<ItemInfoScript>().isHold = false;

				BoxCollider2D[] _colliders = item.GetComponentsInChildren<BoxCollider2D>();
				foreach (BoxCollider2D collider in _colliders)
				{
					collider.enabled = true;
				}
				
				item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
				_playerInfo.isHolding = false;
			}
		}
	}

	public void DropOnSupport(GameObject pTarget)
	{
		if (_playerInfo.isHolding)
		{
			if (transform.GetChild(0).transform.childCount > 0)
			{
				ItemInfoScript targetScript = pTarget.GetComponent<ItemInfoScript>();

				if (!targetScript.isOccupied)
				{
					Debug.Log("target name : " + pTarget.name);

					Transform item = transform.GetChild(0).transform.GetChild(0);
					

					item.parent = pTarget.transform.GetChild(0);
					item.position = pTarget.transform.GetChild(0).position;

					item.GetComponent<ItemInfoScript>().isHold = false;
					item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

					BoxCollider2D[] _colliders = item.GetComponentsInChildren<BoxCollider2D>();
					foreach (BoxCollider2D collider in _colliders)
					{
						collider.enabled = true;
					}

					if (item.tag == "chaudron")
					{
						item.GetComponent<ChaudronScript>().isCooking = true;
						pTarget.GetComponent<ItemInfoScript>().isOccupied = true;
					}
					else
					{
						item.GetComponent<ItemScript>().onCraftingTable = true;
						pTarget.GetComponent<ItemInfoScript>().isOccupied = true;
					}

					_playerInfo.isHolding = false;
				}
				else
				{
					DropOff();
				}
			}
		}
	}


	private Collider2D GetClosestCollider(int pLayerMask)
	{
		float dist = 1000;
		Collider2D target = null;

		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _playerInfo.pickUpRange, pLayerMask);
		if (colliders.Length > 0)
		{

			for (int i = 0; i < colliders.Length; i++)
			{
				Collider2D item = colliders[i];

				float distTemp = Math.Abs(item.transform.position.x - transform.position.x);
				if (distTemp < dist)
				{
					dist = distTemp;
					target = item;
				}
			}
		}

		return target;
	}

	private bool CheckPlayerDirection(Collider2D pTarget)
	{
		//Horizontal
		if (_playerAction.lastDir.x > 0)
		{
			if (pTarget.transform.position.x > transform.position.x)
			{
				return true;
			}
		}
		else if(_playerAction.lastDir.x < 0)
		{
			if (pTarget.transform.position.x < transform.position.x)
			{
				return true;
			}
		}
		//Vertical
		else if (_playerAction.lastDir.y > 0)
		{
			if (pTarget.transform.position.y > transform.position.y)
			{
				return true;
			}
		}
		else if (_playerAction.lastDir.y < 0)
		{
			if (pTarget.transform.position.y < transform.position.y)
			{
				return true;
			}
		}

		return false;
	}
}

