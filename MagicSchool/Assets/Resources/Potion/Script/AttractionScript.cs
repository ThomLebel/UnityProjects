using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractionScript : MonoBehaviour
{
	[SerializeField]
	private float accioForce = 30f;
	[SerializeField]
	private bool isMoving;
	private Rigidbody2D rb2d;

	void Awake()
	{
		rb2d = gameObject.GetComponent<Rigidbody2D>();
	}

	void Start()
	{
		isMoving = false;
	}

	void Update()
	{
		if (Math.Abs(rb2d.velocity.x) > 0)
		{
			isMoving = true;
		}
		else
		{
			isMoving = false;
		}
	}

	public void Accio(Vector3 pDir)
	{
		rb2d.AddForce(transform.right * accioForce * pDir.x * -1);
	}

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (!isMoving)
		{
			return;
		}
		if (collision.tag == "Player")
		{
			PlayerInfo playerInfoScript = collision.GetComponent<PlayerInfo>();
			if (!playerInfoScript.isHolding && !playerInfoScript.isStun && !playerInfoScript.isPreparing)
			{
				rb2d.velocity = Vector2.zero;
				collision.GetComponent<UseItemPotion>().PickUp(gameObject);
			}
		}
	}
}
