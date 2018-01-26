using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDispenserScript : MonoBehaviour {

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
			GameObject item = Instantiate(itemGiven);

			pPlayer.GetComponent<UseItem>().PickUp(item.GetComponent<Collider2D>());

			elapsedTime = Time.time + cooldown;
			timeBeforeNextItem = cooldown;

			coroutine = WaitAndPrint(1.0f);
			StartCoroutine(coroutine);
		}
	}

	private IEnumerator WaitAndPrint(float waitTime)
	{
		while (true)
		{
			yield return new WaitForSeconds(waitTime);
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
