using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoNetwork : Photon.PunBehaviour, IPunObservable
{

	public string itemName;
	public bool isHold;
	public int maxItem = 3;
	public string[] itemList;

	private void Start()
	{
		isHold = false;
		itemList = new string[maxItem];
	}

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(isHold);
			//stream.SendNext(transform.parent);
			//stream.SendNext(itemList);
		}
		else
		{
			// Network player, receive data
			this.isHold = (bool)stream.ReceiveNext();
			//this.transform.parent = (Transform)stream.ReceiveNext();
			//this.itemList = (string[])stream.ReceiveNext();
		}
	}
}
