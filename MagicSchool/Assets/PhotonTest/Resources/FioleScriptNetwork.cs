using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FioleScriptNetwork : MonoBehaviour, IPunObservable
{

	public SpriteRenderer[] pictoList;

	public List<string> itemList;

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(itemList);
		}
		else
		{
			this.itemList = (List<string>)stream.ReceiveNext();
		}
	}
}
