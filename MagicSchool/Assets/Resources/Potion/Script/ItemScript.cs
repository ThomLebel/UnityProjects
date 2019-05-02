using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour {

	public float spellForce = 30f;
	public float preparingCoef = 0.2f;

	public bool isPrepared = false;
	public bool isDone = false;
	public bool onCraftingTable = false;

	//public GameObject _progressBar;
	private ProgressBarScript _progressBarScript;
	private ItemInfoScript _itemInfo;

	private void Awake()
	{
		_progressBarScript = gameObject.GetComponentInChildren<ProgressBarScript>();
	}

	public virtual void AccioItem(Vector3 pDir)
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

	public virtual void PrepareIngredient()
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
}
