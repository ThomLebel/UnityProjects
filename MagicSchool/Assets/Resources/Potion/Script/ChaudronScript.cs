using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ChaudronScript : MonoBehaviour
{
	public int playerOwner;

	public bool isFull;
	public bool isCooking;
	public bool isDone;
	public bool isBurning;

	public float cookingCoef = 0.05f;
	public float burningDelay = 5f;

	private float initialCookingCoef;
	private float burningTimer = 0f;
	private float initialBurningDelay;

	public GameObject _progressBar;
	private ProgressBarScript _progressBarScript;
	private ItemInfoScript _itemInfo;

	private Color _barColor;
	private float _burningCoef = 0f;

	// Use this for initialization
	void Start()
	{
		_itemInfo = gameObject.GetComponent<ItemInfoScript>();
		_progressBarScript = _progressBar.GetComponent<ProgressBarScript>();

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

	public void AddItem(string pItemName)
	{
		if (_itemInfo.itemList.Count < _itemInfo.maxItem)
		{
			isDone = false;
			isBurning = false;
			_progressBar.GetComponentInChildren<SpriteRenderer>().color = _barColor;
			_burningCoef = 0f;
			burningDelay = initialBurningDelay;
			
			SpriteRenderer currentPicto = null;

			for (int i = _itemInfo.pictoList.Length - 1; i >= 0; i--)
			{
				if (_itemInfo.pictoList[i].sprite == null)
				{
					currentPicto = _itemInfo.pictoList[i];
				}
			}
			if (currentPicto != null)
			{
				string pictoName = pItemName + "Picto";
				currentPicto.sprite = Resources.Load("Potion/Picto/"+pictoName, typeof(Sprite)) as Sprite;
			}

			_itemInfo.itemList.Add(pItemName);

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


	public void ControlFire(int pPlayerOwner)
	{
		Debug.Log("On joue avec le feu");
		if (pPlayerOwner == playerOwner)
		{
			Debug.Log("Est-ce que le chaudron brule ?");
			if (isBurning)
			{
				Debug.Log("on éteint son chaudron");
				isBurning = false;
				_progressBar.GetComponentInChildren<SpriteRenderer>().color = _barColor;
				_burningCoef = 0f;
				burningDelay = initialBurningDelay;

				Empty();
			}
			cookingCoef = initialCookingCoef;
		}
		else
		{
			if (isCooking)
			{
				Debug.Log("On accélère la cuisson du chaudron du joueur " + playerOwner);
				cookingCoef = 0.1f;
				burningDelay = 3f;
			}
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
			if (Time.time > burningTimer && !isBurning && isCooking)
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

	private void Empty()
	{
		_progressBarScript.value = 0f;
		_itemInfo.itemList = new List<string>();
		isDone = false;
		isFull = false;
		isCooking = false;
		isBurning = false;

		for (int i = 0; i < _itemInfo.pictoList.Length; i++)
		{
			_itemInfo.pictoList[i].sprite = null;
		}
	}

	public void SetCookingTime(float pTime)
	{
		_progressBarScript.value = pTime;
	}

	public float GetCookingTime()
	{
		return _progressBarScript.value;
	}
}
