using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScriptNetwork : MonoBehaviour, IPunObservable
{

	public bool isOccupied;

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(isOccupied);
		}
		else
		{
			// Network player, receive data
			this.isOccupied = (bool)stream.ReceiveNext();
		}
	}

	private void Start()
	{
		isOccupied = false;
	}
}
