using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCallback : MonoBehaviour {

	private PlayerPotion _parent;

	private void Start()
	{
		_parent = transform.parent.GetComponent<PlayerPotion>();
	}

	private void shootAnimOver()
	{
		_parent.instantiateBlast();
	}
	private void protectAnimOver()
	{
		_parent.instantiateProtection();
	}
}
