using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemNetwork : Photon.PunBehaviour
{
	private PlayerInfo _playerInfo;
	private PlayerNetwork _playerAction;
	private Transform _itemLocation;

	private void Start()
	{
		_playerInfo = gameObject.GetComponent<PlayerInfo>();
		_playerAction = gameObject.GetComponent<PlayerNetwork>();

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
						target.GetComponent<ChaudronScriptNetwork>().isCooking = false; ;
						if (target.transform.parent != null)
							target.transform.parent.transform.parent.GetComponent<FireScriptNetwork>().isOccupied = false;
					}
					int targetID = target.GetComponent<PhotonView>().viewID;
					photonView.RPC("PickUp", PhotonTargets.All, targetID);
					//PickUp(target);
				}
				else
				{
					target.GetComponent<ItemDispenserScriptNetwork>().GiveItem(gameObject);
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
			photonView.RPC("DropOff", PhotonTargets.All);
			//DropOff();
	}


	//Drop this object into our cauldron !
	private void CookItem(Collider2D pTarget, Transform pItem)
	{
		if (!pTarget.GetComponent<ChaudronScriptNetwork>().isFull)
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
			photonView.RPC("DropOff", PhotonTargets.All, pTarget.transform.GetChild(0).transform);
			//DropOff(pTarget.transform.GetChild(0).transform);
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
		ItemInfoNetwork targetInfoScript = pTarget.GetComponent<ItemInfoNetwork>();
		ItemInfoNetwork itemInfoScript = pItem.GetComponent<ItemInfoNetwork>();
		ChaudronScriptNetwork chaudronScript = pItem.GetComponent<ChaudronScriptNetwork>();

		if (chaudronScript == null)
			chaudronScript = pTarget.GetComponent<ChaudronScriptNetwork>();

		if (targetInfoScript.itemList.Length == 0 && itemInfoScript.itemList.Length != 0)
		{
			if(pTarget.tag == "chaudron")
			{
				for (int i = 0; i < itemInfoScript.itemList.Length; i++)
				{
					chaudronScript.AddItem(itemInfoScript.itemList[i]);
				}
				chaudronScript.isDone = true;
				chaudronScript.SetCookingTime(1f);
			}
			else
			{
				if (chaudronScript.isDone)
				{
					targetInfoScript.itemList = itemInfoScript.itemList;
					chaudronScript.isFull = false;
					chaudronScript.isDone = false;
					chaudronScript.SetCookingTime(0f);
				}
			}
			itemInfoScript.itemList = new string[itemInfoScript.maxItem];
		}
		else if (itemInfoScript.itemList.Length == 0 && targetInfoScript.itemList.Length != 0)
		{
			if (pTarget.tag == "chaudron")
			{
				if (chaudronScript.isDone)
				{
					itemInfoScript.itemList = targetInfoScript.itemList;
					chaudronScript.isFull = false;
					chaudronScript.isDone = false;
					chaudronScript.SetCookingTime(0f);
				}
			}
			else
			{
				for (int i = 0; i < targetInfoScript.itemList.Length; i++)
				{
					chaudronScript.AddItem(targetInfoScript.itemList[i]);
				}
				chaudronScript.isDone = true;
				chaudronScript.SetCookingTime(1f);
			}
			targetInfoScript.itemList = new string[targetInfoScript.maxItem];
		}
	}

	//Serve potion !
	private void ServePotion(Collider2D pTarget, Transform pItem)
	{
		if (CheckPlayerDirection(pTarget))
		{
			PotionMasterScript potionMasterScript = pTarget.GetComponent<PotionMasterScript>();
			potionMasterScript.CheckPotionValidity(pItem.GetComponent<FioleScriptNetwork>().itemList);

			if (PhotonNetwork.connected)
				PhotonNetwork.Destroy(pItem.gameObject);
			else
				Destroy(pItem.gameObject);

			_playerInfo.isHolding = false;
		}
	}


	public void AddItem(Collider2D pTarget, Transform pItem)
	{
		ItemInfoNetwork itemHoldedInfo = pItem.GetComponent<ItemInfoNetwork>();
		ChaudronScriptNetwork chaudronScript = pTarget.GetComponent<ChaudronScriptNetwork>();

		if (!chaudronScript.isBurning && !chaudronScript.isFull)
		{
			chaudronScript.AddItem(itemHoldedInfo.name);

			if (PhotonNetwork.connected)
				PhotonNetwork.Destroy(pItem.gameObject);
			else
				Destroy(pItem.gameObject);

			_playerInfo.isHolding = false;
		}
	}

	[PunRPC]
	public void PickUp(int viewID)
	{
		GameObject pItem = PhotonView.Find(viewID).gameObject;
		pItem.GetComponent<ItemInfoNetwork>().isHold = true;
		pItem.GetComponent<BoxCollider2D>().enabled = false;
		pItem.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
		pItem.GetComponent<PhotonTransformView>().enabled = false;
		pItem.transform.position = _itemLocation.position;
		pItem.transform.parent = _itemLocation;
		pItem.transform.rotation = new Quaternion(0, 0, 0, 0);

		_playerInfo.isHolding = true;
	}

	[PunRPC]
	public void DropOff()
	{
		if (_playerInfo.isHolding)
		{
			if (transform.GetChild(0).transform.childCount > 0)
			{
				Transform item = transform.GetChild(0).transform.GetChild(0);

				item.parent = null;

				item.GetComponent<ItemInfoNetwork>().isHold = false;
				item.GetComponent<BoxCollider2D>().enabled = true;
				item.GetComponent<PhotonTransformView>().enabled = true;
				item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
				_playerInfo.isHolding = false;
			}
		}
	}

	[PunRPC]
	public void DropOff(Transform pTarget)
	{
		if (_playerInfo.isHolding)
		{
			if (transform.GetChild(0).transform.childCount > 0)
			{
				Transform item = transform.GetChild(0).transform.GetChild(0);

				item.GetComponent<PhotonTransformView>().enabled = true;

				item.parent = pTarget;
				item.position = pTarget.position;

				item.GetComponent<BoxCollider2D>().enabled = true;
				item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

				item.GetComponent<ItemInfoNetwork>().isHold = false;
				item.GetComponent<ChaudronScriptNetwork>().isCooking = true;
				pTarget.parent.GetComponent<FireScriptNetwork>().isOccupied = true;

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

