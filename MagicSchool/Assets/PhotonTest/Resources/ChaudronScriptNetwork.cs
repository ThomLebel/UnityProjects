﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaudronScriptNetwork : MonoBehaviour, IPunObservable
{

	//private ItemInfo _itemInfo;

	public int itemNeeded = 3;
	public bool isFull;
	public bool isCooking;
	public bool isDone;
	public bool isBurning;

	public float cookingCoef = 0.05f;
	private float burningTimer = 0f;
	public float burningDelay = 5f;

	public List<string> itemList;

	public GameObject _progressBar;
	private ProgressBarScriptNetwork _progressBarScript;

	// Use this for initialization
	void Start()
	{
		//_itemInfo = gameObject.GetComponent<ItemInfo>();
		_progressBarScript = _progressBar.GetComponent<ProgressBarScriptNetwork>();

		isFull = false;
		isCooking = false;
		isDone = false;
		isBurning = false;
	}

	private void Update()
	{
		if (isCooking && itemList.Count > 0)
		{
			Cooking();
		}
		if (itemList.Count == 0)
		{
			_progressBarScript.value = 0f;
		}
	}

	public void AddItem(string pName)
	{
		if (itemList.Count < itemNeeded)
		{
			itemList.Add(pName);
			if (itemList.Count > 1)
			{
				_progressBarScript.value = (_progressBarScript.value / 3f) * 2f;
			}
		}
		else
		{
			isFull = true;
		}
	}

	private void Cooking()
	{
		if (_progressBarScript.value < 1)
			_progressBarScript.value += Time.deltaTime * cookingCoef;
		else
		{
			_progressBarScript.value = 1;
			if (isDone == false)
			{
				burningTimer = Time.time + burningDelay;
				isDone = true;
			}
			if (Time.time > burningTimer && !isBurning)
			{
				isBurning = true;
				Burning();
			}
		}
	}

	private void Burning()
	{
		Debug.Log("Cauldron is burning !!");
	}

	public void SetCookingTime(float pTime)
	{
		_progressBarScript.value = pTime;
	}
	public float GetCookingTime()
	{
		return _progressBarScript.value;
	}

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(itemList);
			stream.SendNext(isFull);
			stream.SendNext(isCooking);
			stream.SendNext(isDone);
			stream.SendNext(isBurning);
		}
		else
		{
			// Network player, receive data
			this.itemList = (List<string>)stream.ReceiveNext();
			this.isFull = (bool)stream.ReceiveNext();
			this.isCooking = (bool)stream.ReceiveNext();
			this.isDone = (bool)stream.ReceiveNext();
			this.isBurning = (bool)stream.ReceiveNext();
		}
	}
}
