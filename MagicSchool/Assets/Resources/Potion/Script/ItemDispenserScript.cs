using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ItemDispenserScript : ItemScript
{
	public GameObject itemGiven;

	public int cooldown;

	private float elapsedTime;
	private float timeBeforeNextItem;
	private IEnumerator coroutine;

	private void Start()
	{
		timeBeforeNextItem = cooldown;
	}

	public void GiveItem(GameObject pPlayer)
	{
		if (Time.time >= elapsedTime)
		{
			GameObject item;

			item = Instantiate(itemGiven) as GameObject;

			pPlayer.GetComponent<UseItemPotion>().PickUp(item);

			elapsedTime = Time.time + cooldown;
			timeBeforeNextItem = cooldown;

			coroutine = WaitAndPrint(1.0f);
			StartCoroutine(coroutine);
		}
	}


	private IEnumerator WaitAndPrint(float pWaitTime)
	{
		while (true)
		{
			yield return new WaitForSeconds(pWaitTime);
			Countdown();
		}
	}

	private void Countdown()
	{
		timeBeforeNextItem--;

		if (timeBeforeNextItem > 0)
		{
			Debug.Log("Time before next item : " + timeBeforeNextItem);
		}
		else
		{
			Debug.Log("Dispenser ready !");
			StopCoroutine(coroutine);
		}
	}
}
