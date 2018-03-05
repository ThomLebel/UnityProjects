using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour, IPunObservable {

	public float spellForce = 30f;
	public float preparingCoef = 0.1f;

	public bool isPrepared = false;
	public bool isDone = false;
	public bool onCraftingTable = false;

	//public GameObject _progressBar;
	private ProgressBarScriptNetwork _progressBarScript;
	private ItemInfoNetwork _itemInfo;

	private void Awake()
	{
		_progressBarScript = gameObject.GetComponentInChildren<ProgressBarScriptNetwork>();
	}

	private void Update()
	{
		//if (isPrepared && !isDone)
		//{
		//	PrepareIngredient();
		//}
	}

	public void AccioItem(Vector3 pDir)
	{
		if (pDir.x != 0)
		{
			gameObject.GetComponent<Rigidbody2D>().AddForce(transform.right * spellForce * pDir.x * -1);
		}
		else
		{
			gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * spellForce * pDir.y * -1);
		}
	}

	[PunRPC]
	public void PrepareIngredient()
	{
		if (isDone)
		{
			return;
		}
		if (_progressBarScript.value < 1)
			_progressBarScript.value += Time.deltaTime * preparingCoef;
		else
		{
			_progressBarScript.value = 1;
			if (!isDone)
			{
				isDone = true;
			}
		}
	}

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(isPrepared);
			stream.SendNext(isDone);
			stream.SendNext(onCraftingTable);
		}
		else
		{
			// Network player, receive data
			this.isPrepared = (bool)stream.ReceiveNext();
			this.isDone = (bool)stream.ReceiveNext();
			this.onCraftingTable = (bool)stream.ReceiveNext();
		}
	}
}
