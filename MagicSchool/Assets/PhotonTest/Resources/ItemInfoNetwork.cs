using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoNetwork : Photon.PunBehaviour, IPunObservable
{

	public string itemName;
	public int maxItem = 3;
	public List<string> itemList;


	public bool isHold;
	public bool isOccupied;

	private void Start()
	{
		isHold = false;
		isOccupied = false;
		//itemList = new string[maxItem];
	}

	private void Update()
	{
		//transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
	}

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(isHold);
			stream.SendNext(isOccupied);
		}
		else
		{
			// Network player, receive data
			this.isHold = (bool)stream.ReceiveNext();
			this.isOccupied = (bool)stream.ReceiveNext();
		}
	}
}
