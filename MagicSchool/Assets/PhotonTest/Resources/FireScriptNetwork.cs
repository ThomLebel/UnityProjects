﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScriptNetwork : Photon.PunBehaviour, IPunObservable
{

	public bool isOccupied;

	private void Start()
	{
		isOccupied = false;
	}


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
}