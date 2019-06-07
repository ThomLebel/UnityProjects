using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ChaudronScript : ItemScript
{
	public bool isFull;
	public bool isCooking;
	public bool isDone;
	public bool isBurning;

	public float cookingCoef = 0.05f;
	public float burningDelay = 5f;

	private float initialCookingCoef;
	private float burningTimer = 0f;
	private float initialBurningDelay;

	public GameObject progressBar;
	public ProgressBarScript progressBarScript;
	public AffiliableScript affiliableScript;
	private FillableScript fillableScript;
	private SupportableScript supportableScript;

	private Color _barColor;
	private float _burningCoef = 0f;

	// Use this for initialization
	void Start()
	{
		fillableScript = gameObject.GetComponent<FillableScript>();
		supportableScript = gameObject.GetComponent<SupportableScript>();
		progressBarScript = progressBar.GetComponent<ProgressBarScript>();

		_barColor = progressBar.GetComponentInChildren<SpriteRenderer>().color;

		isFull = false;
		isCooking = false;
		isDone = false;
		isBurning = false;

		initialCookingCoef = cookingCoef;
		initialBurningDelay = burningDelay;
	}

	private void Update()
	{
		if (isCooking && fillableScript.itemList.Count > 0)
		{
			Cooking();
		}
		if (fillableScript.itemList.Count == 0)
		{
			progressBarScript.value = 0f;
		}
	}

	public void AddItem(string pName, Sprite pSprite)
	{
		if (fillableScript.itemList.Count < fillableScript.maxItem)
		{
			isDone = false;
			isBurning = false;
			progressBar.GetComponentInChildren<SpriteRenderer>().color = _barColor;
			_burningCoef = 0f;
			burningDelay = initialBurningDelay;
			
			SpriteRenderer currentPicto = null;

			for (int i = fillableScript.pictoList.Length - 1; i >= 0; i--)
			{
				if (fillableScript.pictoList[i].sprite == null)
				{
					currentPicto = fillableScript.pictoList[i];
				}
			}
			if (currentPicto != null)
			{
				currentPicto.sprite = pSprite;
			}

			fillableScript.itemList.Add(pName);

			if (fillableScript.itemList.Count > 1)
			{
				progressBarScript.value = (progressBarScript.value / 3f) * 2f;
			}
			if (fillableScript.itemList.Count == fillableScript.maxItem)
			{
				isFull = true;
			}
			if (supportableScript.onSupport && !isCooking)
			{
				isCooking = true;
				progressBarScript.ToggleVisibility(true);
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
		if (pPlayerOwner == affiliableScript.teamID)
		{
			Debug.Log("Est-ce que le chaudron brule ?");
			if (isBurning)
			{
				Debug.Log("on éteint son chaudron");
				isBurning = false;
				progressBar.GetComponentInChildren<SpriteRenderer>().color = _barColor;
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
				Debug.Log("On accélère la cuisson du chaudron du joueur " + affiliableScript.teamID);
				IncreaseFire();
			}
		}
	}

	public void IncreaseFire()
	{
		cookingCoef = 0.1f;
		burningDelay = 3f;
	}

	private void Cooking()
	{
		if (progressBarScript.value < 1)
			progressBarScript.value += Time.deltaTime * cookingCoef;
		else
		{
			progressBarScript.value = 1;
			if (!isDone)
			{
				burningTimer = Time.time + burningDelay;
				isDone = true;
			}
			if (isDone && !isBurning)
			{
				progressBar.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(_barColor, Color.red, _burningCoef);
				_burningCoef += Time.deltaTime / burningDelay;
			}
			if (Time.time > burningTimer && !isBurning && isCooking)
			{
				isBurning = true;
				Burning();
				progressBar.GetComponentInChildren<SpriteRenderer>().color = Color.red;
			}
		}
	}

	private void Burning()
	{
		Debug.Log("Cauldron is burning !!");
	}

	public void Empty()
	{
		progressBarScript.value = 0f;
		progressBarScript.ToggleVisibility(false);
		fillableScript.itemList = new List<string>();
		isDone = false;
		isFull = false;
		isCooking = false;
		isBurning = false;

		for (int i = 0; i < fillableScript.pictoList.Length; i++)
		{
			fillableScript.pictoList[i].sprite = null;
		}
	}

	public void SetCookingTime(float pTime)
	{
		progressBarScript.value = pTime;
	}

	public float GetCookingTime()
	{
		return progressBarScript.value;
	}
}
