using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

			if (PhotonNetwork.connected)
			{
				if (PhotonNetwork.isMasterClient)
				{
					item = PhotonNetwork.Instantiate(this.itemGiven.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0) as GameObject;
					int itemID = item.GetComponent<PhotonView>().viewID;
					pPlayer.GetComponent<PhotonView>().RPC("GetGameObjects", PhotonTargets.All, "PickUp", itemID);
				}
			}
			else
			{
				item = Instantiate(itemGiven) as GameObject;

				pPlayer.GetComponent<UseItemNetwork>().PickUp(item);
			}
			

			elapsedTime = Time.time + cooldown;
			timeBeforeNextItem = cooldown;

			coroutine = WaitAndPrint(1.0f);
			StartCoroutine(coroutine);
		}
	}

	[PunRPC]
	public void GetGameObjects(string pCallback, int pTargetID = -1)
	{
		GameObject target = null;

		if (pTargetID != -1)
			target = PhotonView.Find(pTargetID).gameObject;

		//Get the method information using the method info class
		MethodInfo mi = this.GetType().GetMethod(pCallback);

		//Invoke the method
		// (null- no parameter for the method call
		// or you can pass the array of parameters...)
		if (mi != null)
		{
			if (target != null)
			{
				mi.Invoke(this, new object[] { target });
			}
		}
		else
		{
			Debug.LogWarning("method with name: function" + pCallback + " doesn't exist");
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
