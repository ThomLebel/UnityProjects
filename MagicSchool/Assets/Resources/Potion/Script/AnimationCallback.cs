using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCallback : MonoBehaviour {

	private void shootAnimOver()
	{
		transform.parent.GetComponent<PlayerPotion>().InstantiateBlast();
	}
	private void protectAnimOver()
	{
		transform.parent.GetComponent<PlayerPotion>().InstantiateProtection();
	}

	private void catchItem()
	{
		transform.parent.GetComponent<PlayerPotion>().PickDropItem();
	}
}
