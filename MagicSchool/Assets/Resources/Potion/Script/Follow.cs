using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

	public Vector3 offset;          // The offset at which the Health Bar follows the player.

	public Transform target;       // Reference to the target.


	void Awake()
	{
		// Setting up the reference.
		//target = GameObject.FindGameObjectWithTag("Player").transform;
	}

	void Update()
	{
		// Set the position to the player's position with the offset.
		transform.position = target.position + offset;
	}
}
