using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemNetwork : Photon.PunBehaviour
{
	private PlayerInfo _playerInfo;
	private PlayerNetwork _playerAction;

	private int targetID, itemID;

	private void Start()
	{
		_playerInfo = gameObject.GetComponent<PlayerInfo>();
		_playerAction = gameObject.GetComponent<PlayerNetwork>();
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
					if (PhotonNetwork.connected)
					{
						targetID = target.GetComponent<PhotonView>().viewID;
						photonView.RPC("PickUp", PhotonTargets.All, targetID);
					}
					else
					{
						PickUp(target);
					}
				}
				else
				{
					if (PhotonNetwork.connected)
					{
						int gameObjectID = gameObject.GetComponent<PhotonView>().viewID;
						target.GetComponent<PhotonView>().RPC("GiveItem", PhotonTargets.All, gameObjectID);
					}
					else
					{
						target.GetComponent<ItemDispenserScriptNetwork>().GiveItem(gameObject);
					}
					
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
			if (PhotonNetwork.connected)
			{
				targetID = target.GetComponent<PhotonView>().viewID;
				itemID = itemHolded.GetComponent<PhotonView>().viewID;
			}

			if (itemHolded.tag == "item")
			{
				if (target.tag == "craftTable")
				{
					if (PhotonNetwork.connected)
					{
						PlaceObject(targetID);
						//photonView.RPC("PlaceObject", PhotonTargets.All, targetID);
					}
					else
					{
						PlaceObject(target, itemHolded);
					}
				}
				else
				{
					if (PhotonNetwork.connected)
					{
						CookItem(targetID, itemID);
						//photonView.RPC("CookItem", PhotonTargets.All, targetID, itemID);
					}
					else
					{
						CookItem(target, itemHolded);
					}
				}
			}
			else if (itemHolded.tag == "chaudron")
			{
				if (target.tag == "fire")
				{
					if (PhotonNetwork.connected)
					{
						PlaceObject(targetID);
						//photonView.RPC("PlaceObject", PhotonTargets.All, targetID);
					}
					else
					{
						PlaceObject(target, itemHolded);
					}
				}
				else
				{
					if (PhotonNetwork.connected)
					{
						Debug.Log("Trying to transvase chaudron to potion");
						TransvasePotion(targetID, itemID);
						//photonView.RPC("TransvasePotion", PhotonTargets.All, targetID, itemID);
					}
					else
					{
						TransvasePotion(target, itemHolded);
					}
				}
			}
			else if (itemHolded.tag == "fiole")
			{
				if (target.tag == "pnj")
				{
					if (PhotonNetwork.connected)
					{
						ServePotion(targetID, itemID);
						//photonView.RPC("ServePotion", PhotonTargets.All, targetID, itemID);
					}
					else
					{
						ServePotion(target, itemHolded);
					}
				}
				else
				{
					if (PhotonNetwork.connected)
					{
						Debug.Log("Trying to transvase potion to chaudron");
						TransvasePotion(targetID, itemID);
						//photonView.RPC("TransvasePotion", PhotonTargets.All, targetID, itemID);
					}
					else
					{
						TransvasePotion(target, itemHolded);
					}
				}
			}
		}
		else
		{
			if (PhotonNetwork.connected)
			{
				photonView.RPC("DropOff", PhotonTargets.All);
			}
			else
			{
				DropOff();
			}
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

	//Drop this object into our cauldron !
	[PunRPC]
	private void CookItem(int targetID, int itemID)
	{
		Collider2D pItem = PhotonView.Find(itemID).GetComponent<Collider2D>();
		Collider2D pTarget = PhotonView.Find(targetID).GetComponent<Collider2D>();

		Debug.Log("Trying to cook "+pItem.name);

		if (!pTarget.GetComponent<ChaudronScriptNetwork>().isFull)
		{
			if (CheckPlayerDirection(pTarget))
			{
				photonView.RPC("AddItemToCauldron", PhotonTargets.All, targetID, itemID);
			}
		}
	}
	//Offline Version
	private void CookItem(Collider2D pTarget, Transform pItem)
	{
		if (!pTarget.GetComponent<ChaudronScriptNetwork>().isFull)
		{
			if (CheckPlayerDirection(pTarget))
			{
				AddItemToCauldron(pTarget, pItem);
			}
		}
	}

	//Put the cauldron over the fire !
	[PunRPC]
	private void PlaceObject(int targetID)
	{
		Collider2D pTarget = PhotonView.Find(targetID).GetComponent<Collider2D>();
		ItemInfoNetwork targetScript = pTarget.GetComponent<ItemInfoNetwork>();

		Debug.Log("Trying to put object on " + pTarget.name);

		if (CheckPlayerDirection(pTarget) && !targetScript.isOccupied)
		{
			photonView.RPC("DropOff", PhotonTargets.All, targetID);
		}
		else
		{
			photonView.RPC("DropOff", PhotonTargets.All);
		}
	}
	//Offline Version
	private void PlaceObject(Collider2D pTarget, Transform pItem)
	{
		if (CheckPlayerDirection(pTarget))
		{
			DropOff(pTarget.transform.GetChild(0).transform);
		}
		else
		{
			DropOff();
		}
	}

	//Transvase potion from cauldron to fiole
	[PunRPC]
	private void TransvasePotion(int targetID, int itemID)
	{
		Collider2D pTarget = PhotonView.Find(targetID).GetComponent<Collider2D>();
		if (CheckPlayerDirection(pTarget))
		{
			Debug.Log("Trying to switch chaudron and potion");
			photonView.RPC("Switch", PhotonTargets.All, targetID, itemID);
			//Switch(targetID, itemID);
		}
	}
	//Offline Version
	private void TransvasePotion(Collider2D pTarget, Transform pItem)
	{
		if (CheckPlayerDirection(pTarget))
		{
			Switch(pTarget, pItem);
		}
	}

	[PunRPC]
	private void Switch(int targetID, int itemID)
	{
		GameObject pItem = PhotonView.Find(itemID).gameObject;
		GameObject pTarget = PhotonView.Find(targetID).gameObject;

		ItemInfoNetwork targetInfoScript = pTarget.GetComponent<ItemInfoNetwork>();
		ItemInfoNetwork itemInfoScript = pItem.GetComponent<ItemInfoNetwork>();
		ChaudronScriptNetwork chaudronScript = pItem.GetComponent<ChaudronScriptNetwork>();

		if (chaudronScript == null)
			chaudronScript = pTarget.GetComponent<ChaudronScriptNetwork>();

		Debug.Log("target : "+ targetInfoScript.itemName + "; item Count : "+ targetInfoScript.itemList.Count + " && item : "+ itemInfoScript.itemName + "; item Count : "+itemInfoScript.itemList.Count);

		if (targetInfoScript.itemList.Count == 0 && itemInfoScript.itemList.Count != 0)
		{
			if(pTarget.tag == "chaudron")
			{
				Debug.Log("On tient une potion pleine et on la verse dans le chaudron vide");
				for (int i = 0; i < itemInfoScript.itemList.Count; i++)
				{
					chaudronScript.AddItem(itemInfoScript.itemList[i]);
				}
				chaudronScript.isDone = true;
				chaudronScript.SetCookingTime(1f);
				itemInfoScript.itemList = new List<string>();

				for (int i = 0; i<itemInfoScript.pictoList.Length; i++)
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
	//Offline Version
	private void Switch(Collider2D pTarget, Transform pItem)
	{
		ItemInfoNetwork targetInfoScript = pTarget.GetComponent<ItemInfoNetwork>();
		ItemInfoNetwork itemInfoScript = pItem.GetComponent<ItemInfoNetwork>();
		ChaudronScriptNetwork chaudronScript = pItem.GetComponent<ChaudronScriptNetwork>();

		if (chaudronScript == null)
			chaudronScript = pTarget.GetComponent<ChaudronScriptNetwork>();

		if (targetInfoScript.itemList.Count == 0 && itemInfoScript.itemList.Count != 0)
		{
			if (pTarget.tag == "chaudron")
			{
				for (int i = 0; i < itemInfoScript.itemList.Count; i++)
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
			itemInfoScript.itemList = new List<string>();
		}
		else if (itemInfoScript.itemList.Count == 0 && targetInfoScript.itemList.Count != 0)
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
				for (int i = 0; i < targetInfoScript.itemList.Count; i++)
				{
					chaudronScript.AddItem(targetInfoScript.itemList[i]);
				}
				chaudronScript.isDone = true;
				chaudronScript.SetCookingTime(1f);
			}
			targetInfoScript.itemList = new List<string>();
		}
	}


	//Serve potion !
	[PunRPC]
	private void ServePotion(int targetID, int itemID)
	{
		GameObject pItem = PhotonView.Find(itemID).gameObject;
		Collider2D pTarget = PhotonView.Find(targetID).GetComponent<Collider2D>();

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
	//Offline Version
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


	//Add item into cauldron
	[PunRPC]
	public void AddItemToCauldron(int targetID, int itemID)
	{
		Debug.Log("item Id : "+itemID);

		GameObject pItem = PhotonView.Find(itemID).gameObject;
		GameObject pTarget = PhotonView.Find(targetID).gameObject;

		Debug.Log("Adding to cauldron " + pItem.name);
		
		ChaudronScriptNetwork chaudronScript = pTarget.GetComponent<ChaudronScriptNetwork>();
		ItemScript itemScript = pItem.GetComponent<ItemScript>();

		if (!chaudronScript.isBurning && !chaudronScript.isFull && itemScript.isDone)
		{
			if (PhotonNetwork.isMasterClient)
			{
				pTarget.GetComponent<PhotonView>().RPC("AddItem", PhotonTargets.All, itemID);
				PhotonNetwork.Destroy(pItem.gameObject);
			}

			_playerInfo.isHolding = false;
		}
	}
	//Offline Version
	public void AddItemToCauldron(Collider2D pTarget, Transform pItem)
	{
		ItemInfoNetwork itemHoldedInfo = pItem.GetComponent<ItemInfoNetwork>();
		ItemScript itemScript = pItem.GetComponent<ItemScript>();
		ChaudronScriptNetwork chaudronScript = pTarget.GetComponent<ChaudronScriptNetwork>();

		if (!chaudronScript.isBurning && !chaudronScript.isFull && itemScript.isDone)
		{
			chaudronScript.AddItem(itemHoldedInfo.itemName);

			Destroy(pItem.gameObject);

			_playerInfo.isHolding = false;
		}
	}

	[PunRPC]
	public void PickUp(int targetID)
	{
		GameObject pItem = PhotonView.Find(targetID).gameObject;

		Debug.Log("Picking up "+pItem.name);
		if (pItem.tag == "chaudron")
		{
			pItem.GetComponent<ChaudronScriptNetwork>().isCooking = false;
		}
		if (pItem.tag == "item")
		{
			pItem.GetComponent<ItemScript>().isPrepared = false;
			pItem.GetComponent<ItemScript>().onCraftingTable = false;
		}
		if (pItem.transform.parent != null)
			pItem.transform.parent.parent.GetComponent<ItemInfoNetwork>().isOccupied = false;

		pItem.GetComponent<ItemInfoNetwork>().isHold = true;
		//pItem.GetComponent<BoxCollider2D>().enabled = false;
		pItem.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

		BoxCollider2D[] _colliders = pItem.GetComponentsInChildren<BoxCollider2D>();
		foreach (BoxCollider2D collider in _colliders)
		{
			collider.enabled = false;
		}

		//pItem.GetComponent<PhotonTransformView>().enabled = false;
		pItem.transform.position = _playerInfo.itemLocation.position;
		pItem.transform.parent = _playerInfo.itemLocation;
		pItem.transform.rotation = new Quaternion(0, 0, 0, 0);

		_playerInfo.isHolding = true;
	}
	//Offline Version
	public void PickUp(Collider2D pItem)
	{
		Debug.Log("Picking up " + pItem.name);
		if (pItem.tag == "chaudron")
		{
			pItem.GetComponent<ChaudronScriptNetwork>().isCooking = false;
		}
		if (pItem.tag == "item")
		{
			pItem.GetComponent<ItemScript>().isPrepared = false;
		}
		if (pItem.transform.parent != null)
			pItem.transform.parent.parent.GetComponent<ItemInfoNetwork>().isOccupied = false;

		pItem.GetComponent<ItemInfoNetwork>().isHold = true;
		//pItem.GetComponent<BoxCollider2D>().enabled = false;
		pItem.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

		BoxCollider2D[] _colliders = pItem.GetComponentsInChildren<BoxCollider2D>();
		foreach (BoxCollider2D collider in _colliders)
		{
			collider.enabled = false;
		}

		//pItem.GetComponent<PhotonTransformView>().enabled = false;
		pItem.transform.position = _playerInfo.itemLocation.position;
		pItem.transform.parent = _playerInfo.itemLocation;
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

				Debug.Log("Droping on the floor " + item.name);

				item.parent = null;

				item.GetComponent<ItemInfoNetwork>().isHold = false;
				//item.GetComponent<BoxCollider2D>().enabled = true;

				BoxCollider2D[] _colliders = item.GetComponentsInChildren<BoxCollider2D>();
				foreach (BoxCollider2D collider in _colliders)
				{
					collider.enabled = true;
				}

				//item.GetComponent<PhotonTransformView>().enabled = true;
				item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
				_playerInfo.isHolding = false;
			}
		}
	}

	//Place object on support
	[PunRPC]
	public void DropOff(int viewID)
	{
		if (_playerInfo.isHolding)
		{
			if (transform.GetChild(0).transform.childCount > 0)
			{
				Transform pTarget = PhotonView.Find(viewID).transform;

				Debug.Log("target name : "+pTarget.name);

				Transform item = transform.GetChild(0).transform.GetChild(0);

				//item.GetComponent<PhotonTransformView>().enabled = true;

				item.parent = pTarget.GetChild(0);
				item.position = pTarget.GetChild(0).position;

				item.GetComponent<ItemInfoNetwork>().isHold = false;
				//item.GetComponent<BoxCollider2D>().enabled = true;
				item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

				BoxCollider2D[] _colliders = item.GetComponentsInChildren<BoxCollider2D>();
				foreach (BoxCollider2D collider in _colliders)
				{
					collider.enabled = true;
				}

				if (item.tag == "chaudron")
				{
					item.GetComponent<ChaudronScriptNetwork>().isCooking = true;
					pTarget.GetComponent<ItemInfoNetwork>().isOccupied = true;
				}
				else
				{
					item.GetComponent<ItemScript>().onCraftingTable = true;
					pTarget.GetComponent<ItemInfoNetwork>().isOccupied = true;
				}

				_playerInfo.isHolding = false;
			}
		}
	}
	//Offline Version
	public void DropOff(Transform pTarget)
	{
		if (_playerInfo.isHolding)
		{
			if (transform.GetChild(0).transform.childCount > 0)
			{
				Transform item = transform.GetChild(0).transform.GetChild(0);

				//item.GetComponent<PhotonTransformView>().enabled = true;

				item.parent = pTarget;
				item.position = pTarget.position;

				item.GetComponent<ItemInfoNetwork>().isHold = false;
				//item.GetComponent<BoxCollider2D>().enabled = true;
				item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

				BoxCollider2D[] _colliders = item.GetComponentsInChildren<BoxCollider2D>();
				foreach (BoxCollider2D collider in _colliders)
				{
					collider.enabled = true;
				}

				if (item.tag == "chaudron")
				{
					item.GetComponent<ChaudronScriptNetwork>().isCooking = true;
					pTarget.parent.GetComponent<ItemInfoNetwork>().isOccupied = true;
				}
				else
				{
					item.GetComponent<ItemScript>().onCraftingTable = true;
					pTarget.parent.GetComponent<ItemInfoNetwork>().isOccupied = true;
				}

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

				//if ((item.transform.position.y > (transform.position.y - _playerInfo.playerHeight / 2)) && (item.transform.position.y < (transform.position.y + _playerInfo.playerHeight / 2)))
				//{
					float distTemp = Math.Abs(item.transform.position.x - transform.position.x);
					if (distTemp < dist)
					{
						dist = distTemp;
						target = item;
					}
				//}
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

