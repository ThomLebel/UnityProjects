using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blobScript : ItemScript
{
	public override void AccioItem(Vector3 pDir)
	{
		base.AccioItem(pDir);
		Debug.Log("Accio on the blob !");
	}

	public override void PrepareIngredient()
	{
		base.PrepareIngredient();
		Debug.Log("Preparing the blob !");
	}
}
