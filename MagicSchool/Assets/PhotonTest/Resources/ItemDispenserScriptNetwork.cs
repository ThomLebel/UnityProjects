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

	[PunRPC]
	public void GiveItem(int pPlayerID)
	{
		if (Time.time >= elapsedTime)
		{
			GameObject item;
			GameObject player = PhotonView.Find(pPlayerID).gameObject;

			if (PhotonNetwork.connected && PhotonNetwork.isMasterClient)
			{
				item = PhotonNetwork.Instantiate(this.itemGiven.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0) as GameObject;
				int itemID = item.GetComponent<PhotonView>().viewID;
				player.GetComponent<PhotonView>().RPC("PickUp", PhotonTargets.All, itemID);
			}

			elapsedTime = Time.time + cooldown;
			timeBeforeNextItem = cooldown;

			coroutine = WaitAndPrint(1.0f);
			StartCoroutine(coroutine);
		}
	}
	//Offline version
	public void GiveItem(GameObject pPlayer)
	{
		if (Time.time >= elapsedTime)
		{
			GameObject item;

			if(PhotonNetwork.connected && PhotonNetwork.isMasterClient)
				item = PhotonNetwork.Instantiate(this.itemGiven.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0) as GameObject;
			else
				item = Instantiate(itemGiven) as GameObject;

			int itemID = item.GetComponent<PhotonView>().viewID;
			pPlayer.GetComponent<PhotonView>().RPC("PickUp", PhotonTargets.All, itemID);

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
