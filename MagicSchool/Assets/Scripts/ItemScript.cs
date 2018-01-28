using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour {

	public float spellForce = 30f;

	public void AccioItem(Vector3 dir)
	{
		if (dir.x != 0)
		{
			gameObject.GetComponent<Rigidbody2D>().AddForce(transform.right * spellForce * dir.x * -1);
		}
		else
		{
			gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * spellForce * dir.y * -1);
		}
	}
}
