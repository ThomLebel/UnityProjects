using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaudronScriptNetwork : Photon.PunBehaviour, IPunObservable
{
	public int playerOwner;

	public bool isFull;
	public bool isCooking;
	public bool isDone;
	public bool isBurning;

	public float cookingCoef = 0.05f;
	private float initialCookingCoef;
	private float burningTimer = 0f;
	public float burningDelay = 5f;
	private float initialBurningDelay;

	public GameObject _progressBar;
	private ProgressBarScriptNetwork _progressBarScript;
	private ItemInfoNetwork _itemInfo;

	private Color _barColor;
	private float _burningCoef = 0f;

	// Use this for initialization
	void Start()
	{
		_itemInfo = gameObject.GetComponent<ItemInfoNetwork>();
		_progressBarScript = _progressBar.GetComponent<ProgressBarScriptNetwork>();

		_barColor = _progressBar.GetComponentInChildren<SpriteRenderer>().color;

		isFull = false;
		isCooking = false;
		isDone = false;
		isBurning = false;

		initialCookingCoef = cookingCoef;
		initialBurningDelay = burningDelay;
	}

	private void Update()
	{
		if (isCooking && _itemInfo.itemList.Count > 0)
		{
			Cooking();
		}
		if (_itemInfo.itemList.Count == 0)
		{
			_progressBarScript.value = 0f;
		}
	}

	[PunRPC]
	public void AddItem(int pItemID)
	{
		if (_itemInfo.itemList.Count < _itemInfo.maxItem)
		{
			string itemName = PhotonView.Find(pItemID).gameObject.GetComponent<ItemInfoNetwork>().itemName;

			_itemInfo.itemList.Add(itemName);

			if (_itemInfo.itemList.Count > 1)
			{
				_progressBarScript.value = (_progressBarScript.value / 3f) * 2f;
			}
			if (_itemInfo.itemList.Count == _itemInfo.maxItem)
			{
				isFull = true;
			}
		}
		else
		{
			isFull = true;
		}
	}
	//Offline Version
	public void AddItem(string pName)
	{
		if (_itemInfo.itemList.Count < _itemInfo.maxItem)
		{
			_itemInfo.itemList.Add(pName);

			if (_itemInfo.itemList.Count > 1)
			{
				_progressBarScript.value = (_progressBarScript.value / 3f) * 2f;
			}
		}
		else
		{
			isFull = true;
		}
	}

	[PunRPC]
	public void ControlFire(int pPlayerOwner)
	{
		if (pPlayerOwner == playerOwner)
		{
			if (isBurning)
			{
				Debug.Log("on éteint son chaudron");
				isBurning = false;
				_progressBar.GetComponentInChildren<SpriteRenderer>().color = _barColor;
				_burningCoef = 0f;
				burningDelay = initialBurningDelay;
			}
			cookingCoef = initialCookingCoef;
		}
		else
		{
			Debug.Log("On accélère la cuisson du chaudron du joueur "+playerOwner);
			cookingCoef = 0.1f;
			burningDelay = 3f;
		}
	}

	private void Cooking()
	{
		if (_progressBarScript.value < 1)
			_progressBarScript.value += Time.deltaTime * cookingCoef;
		else
		{
			_progressBarScript.value = 1;
			if (!isDone)
			{
				burningTimer = Time.time + burningDelay;
				isDone = true;
			}
			if (isDone && !isBurning)
			{
				_progressBar.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(_barColor, Color.red, _burningCoef);
				_burningCoef += Time.deltaTime / burningDelay;
			}
			if (Time.time > burningTimer && !isBurning)
			{
				isBurning = true;
				Burning();
				_progressBar.GetComponentInChildren<SpriteRenderer>().color = Color.red;
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
			stream.SendNext(isFull);
			stream.SendNext(isCooking);
			stream.SendNext(isDone);
			stream.SendNext(isBurning);
		}
		else
		{
			// Network player, receive data
			this.isFull = (bool)stream.ReceiveNext();
			this.isCooking = (bool)stream.ReceiveNext();
			this.isDone = (bool)stream.ReceiveNext();
			this.isBurning = (bool)stream.ReceiveNext();
		}
	}
}
