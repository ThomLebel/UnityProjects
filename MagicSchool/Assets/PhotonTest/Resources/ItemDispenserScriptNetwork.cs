using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDispenserScriptNetwork : Photon.PunBehaviour, IPunObservable
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

			if(PhotonNetwork.connected)
				item = PhotonNetwork.Instantiate(this.itemGiven.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0) as GameObject;
			else
				item = Instantiate(itemGiven) as GameObject;

			int itemID = item.GetComponent<PhotonView>().viewID;
			photonView.RPC("PickUp", PhotonTargets.All, item);

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

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(elapsedTime);
		}
		else
		{
			// Network player, receive data
			this.elapsedTime = (float)stream.ReceiveNext();
		}
	}
}
