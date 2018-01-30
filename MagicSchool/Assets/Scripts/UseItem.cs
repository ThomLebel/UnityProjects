using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : MonoBehaviour {

	private PlayerInfo _playerInfo;
	private PlayerPotionAction _playerAction;
	private Transform _itemLocation;

	private void Start()
	{
		_playerInfo = gameObject.GetComponent<PlayerInfo>();
		_playerAction = gameObject.GetComponent<PlayerPotionAction>();

		_itemLocation = gameObject.transform.GetChild(0);
	}

	//Pick-up item !
	public void PickItem()
	{
		Collider2D target = GetClosestCollider(_playerInfo.pickUpLayer);

		if (target != null)
		{
			if ((_playerAction.lastDir.x > 0 && target.transform.position.x > transform.position.x) || (_playerAction.lastDir.x < 0 && target.transform.position.x < transform.position.x))
			{
				if (target.tag != "dispenser")
				{
					if (target.tag == "chaudron")
					{
						target.GetComponent<ChaudronScript>().isCooking = false; ;
						if (target.transform.parent != null)
							target.transform.parent.transform.parent.GetComponent<FireScript>().isOccupied = false;
					}
					PickUp(target);						
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

		int layerMask;

		if (itemHolded.tag == "item")
			layerMask = _playerInfo.chaudronLayer;
		else if (itemHolded.tag == "chaudron")
			layerMask = _playerInfo.fireLayer | _playerInfo.itemLayer;
		else if (itemHolded.tag == "fiole")
			layerMask = _playerInfo.pnjLayer | _playerInfo.chaudronLayer;
		else
			layerMask = 0;



		Collider2D target = GetClosestCollider(layerMask);

		Debug.Log(target);

		if (target != null)
		{
			if (itemHolded.tag == "item")
				CookItem(target, itemHolded);
			else if (itemHolded.tag == "chaudron")
			{
				if (target.tag == "fire")
					PlaceCauldron(target, itemHolded);
				else
					TransvasePotion(target, itemHolded);
			}
					
			else if (itemHolded.tag == "fiole")
			{
				if (target.tag == "pnj")
					ServePotion(target, itemHolded);
				else
					TransvasePotion(target, itemHolded);
			}
				
		}
		else
			DropOff();
	}


	//Drop this object into our cauldron !
	private void CookItem(Collider2D pTarget, Transform pItem)
	{
		if (!pTarget.GetComponent<ChaudronScript>().isFull)
		{
			if (CheckPlayerDirection(pTarget))
			{
				AddItem(pTarget, pItem);
			}
		}
	}


	//Put the cauldron over the fire !
	private void PlaceCauldron(Collider2D pTarget, Transform pItem)
	{
		if (CheckPlayerDirection(pTarget))
		{
			DropOff(pTarget.transform.GetChild(0).transform);
		}
	}


	//Transvase potion from cauldron to fiole
	private void TransvasePotion(Collider2D pTarget, Transform pItem)
	{
		if (CheckPlayerDirection(pTarget))
		{
			Switch(pTarget, pItem);
		}
	}

	private void Switch(Collider2D pTarget, Transform pItem)
	{
		FioleScript fioleScript = pTarget.GetComponent<FioleScript>();
		ChaudronScript chaudronScript = pItem.GetComponent<ChaudronScript>();

		if(fioleScript == null)
			fioleScript = pItem.GetComponent<FioleScript>();
		if(chaudronScript == null)
			chaudronScript = pTarget.GetComponent<ChaudronScript>();

		if (chaudronScript.itemList.Count != 0 && fioleScript.itemList.Count == 0 && chaudronScript.isDone)
		{
			Debug.Log("Cauldron to fiole !");
			fioleScript.itemList = chaudronScript.itemList;
			chaudronScript.itemList = new List<string>();
			chaudronScript.isFull = false;
			chaudronScript.isDone = false;
			chaudronScript.SetCookingTime(0f);
		}
		else if (fioleScript.itemList.Count != 0 && chaudronScript.itemList.Count == 0)
		{
			Debug.Log("Fiole to cauldron !");
			for (int i=0;i<fioleScript.itemList.Count;i++)
			{
				chaudronScript.AddItem(fioleScript.itemList[i]);
			}
			chaudronScript.isDone = true;
			chaudronScript.SetCookingTime(1f);
			fioleScript.itemList = new List<string>();
		}
	}

	//Serve potion !
	private void ServePotion(Collider2D pTarget, Transform pItem)
	{
		if (CheckPlayerDirection(pTarget))
		{
			PotionMasterScript potionMasterScript = pTarget.GetComponent<PotionMasterScript>();
			potionMasterScript.CheckPotionValidity(pItem.GetComponent<FioleScript>().itemList);
			Destroy(pItem.gameObject);
			_playerInfo.isHolding = false;
		}
	}


	public void AddItem(Collider2D pTarget, Transform pItem)
	{
		ItemInfo itemHoldedInfo = pItem.GetComponent<ItemInfo>();
		ChaudronScript chaudronScript = pTarget.GetComponent<ChaudronScript>();

		if (!chaudronScript.isBurning && !chaudronScript.isFull)
		{
			chaudronScript.AddItem(itemHoldedInfo.name);

			Destroy(pItem.gameObject);

			_playerInfo.isHolding = false;
		}	
	}

	public void PickUp(Collider2D pItem)
	{
		pItem.GetComponent<ItemInfo>().isHold = true;
		pItem.GetComponent<BoxCollider2D>().enabled = false;
		pItem.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
		pItem.transform.position = _itemLocation.position;
		pItem.transform.parent = _itemLocation;
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

				item.parent = null;

				item.GetComponent<ItemInfo>().isHold = false;
				item.GetComponent<BoxCollider2D>().enabled = true;
				item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
				_playerInfo.isHolding = false;
			}
		}
	}

	public void DropOff(Transform pTarget)
	{
		if (_playerInfo.isHolding)
		{
			if (transform.GetChild(0).transform.childCount > 0)
			{
				Transform item = transform.GetChild(0).transform.GetChild(0);

				item.parent = pTarget;
				item.position = pTarget.position;

				item.GetComponent<BoxCollider2D>().enabled = true;
				item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

				item.GetComponent<ItemInfo>().isHold = false;
				item.GetComponent<ChaudronScript>().isCooking = true;

				pTarget.parent.GetComponent<FireScript>().isOccupied = true;
				item.GetComponent<ChaudronScript>().isCooking = true;
				_playerInfo.isHolding = false;
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

				if ((item.transform.position.y > (transform.position.y - _playerInfo.playerHeight / 2)) && (item.transform.position.y < (transform.position.y + _playerInfo.playerHeight / 2)))
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

		return target;
	}

	private bool CheckPlayerDirection(Collider2D pTarget)
	{
		if (_playerAction.lastDir.x > 0)
		{
			if (pTarget.transform.position.x > transform.position.x)
			{
				return true;
			}
		}
		else
		{
			if (pTarget.transform.position.x < transform.position.x)
			{
				return true;
			}
		}
		return false;
	}
}

