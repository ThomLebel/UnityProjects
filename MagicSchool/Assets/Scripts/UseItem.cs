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
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _playerInfo.pickUpRange, _playerInfo.pickUpLayer);
		if (colliders.Length > 0)
		{
			Collider2D target = GetClosestCollider(colliders);

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
	}


	//What to do with this object ?
	public void DropItem()
	{
		Transform itemHolded = transform.GetChild(0).transform.GetChild(0);

		int layerMask;

		if (itemHolded.tag == "item")
			layerMask = _playerInfo.chaudronLayer;
		else
			layerMask = _playerInfo.fireLayer;



		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _playerInfo.pickUpRange, layerMask);
		if (colliders.Length > 0)
		{
			Collider2D target = GetClosestCollider(colliders);

			Debug.Log(target);

			if (itemHolded.tag == "item")
				CookItem(target, itemHolded);
			else
				PlaceCauldron(target, itemHolded);
		}
		else
			DropOff();
	}


	//Drop this object into our cauldron !
	private void CookItem(Collider2D target, Transform pItem)
	{
		if (target != null)
		{
			if (!target.GetComponent<ChaudronScript>().isFull)
			{
				if (_playerAction.lastDir.x > 0)
				{
					if (target.transform.position.x > transform.position.x)
					{
						AddItem(target, pItem);
					}
				}
				else
				{
					if (target.transform.position.x < transform.position.x)
					{
						AddItem(target, pItem);
					}
				}
			}
		}
	}


	//Put the cauldron over the fire !
	private void PlaceCauldron(Collider2D target, Transform pItem)
	{
		Debug.Log("place cauldron !");
		if (target != null)
		{
			target.GetComponent<FireScript>().isOccupied = true;
			pItem.GetComponent<ChaudronScript>().isCooking = true;
			DropOff(target.transform.GetChild(0).transform);
		}
	}


	public void AddItem(Collider2D target, Transform item)
	{
		ItemInfo itemHoldedInfo = item.GetComponent<ItemInfo>();

		target.GetComponent<ChaudronScript>().AddItem(itemHoldedInfo.name);

		Destroy(item.gameObject);

		_playerInfo.isHolding = false;
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
				Debug.Log("Cette fonction !");
				Transform item = transform.GetChild(0).transform.GetChild(0);

				item.parent = null;

				item.GetComponent<ItemInfo>().isHold = false;
				item.GetComponent<BoxCollider2D>().enabled = true;
				item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
				_playerInfo.isHolding = false;
			}
		}
	}

	public void DropOff(Transform target)
	{
		if (_playerInfo.isHolding)
		{
			if (transform.GetChild(0).transform.childCount > 0)
			{
				Debug.Log("Et celle-ci");
				Transform item = transform.GetChild(0).transform.GetChild(0);

				item.parent = target;
				item.position = target.position;

				item.GetComponent<BoxCollider2D>().enabled = true;
				item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

				item.GetComponent<ItemInfo>().isHold = false;
				_playerInfo.isHolding = false;
			}
		}
	}


	private Collider2D GetClosestCollider(Collider2D[] pColliders)
	{
		float dist = 1000;
		Collider2D target = null;

		for (int i = 0; i < pColliders.Length; i++)
		{
			Collider2D item = pColliders[i];

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

		return target;
	}
}
