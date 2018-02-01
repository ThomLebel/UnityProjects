using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoNetwork : MonoBehaviour, IPunObservable
{

	public string itemName;
	public bool isHold;

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(isHold);
		}
		else
		{
			// Network player, receive data
			this.isHold = (bool)stream.ReceiveNext();
		}
	}

	private void Start()
	{
		isHold = false;
	}
}
