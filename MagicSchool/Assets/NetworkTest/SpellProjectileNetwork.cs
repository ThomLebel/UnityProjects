using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpellProjectileNetwork : NetworkBehaviour
{
	[SyncVar]
	public Vector3 direction;

	public int speed;
	public float lifeSpan;

	private IEnumerator coroutine;

	private void Start()
	{
		if (direction.x != 0)
			transform.localScale = new Vector3(transform.localScale.x * direction.x, transform.localScale.y, transform.localScale.z);
		else
		{
			transform.Rotate(0, 0, 90);
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * direction.y, transform.localScale.z);
		}

		coroutine = WaitAndDie(lifeSpan);
		StartCoroutine(coroutine);
	}

	// Update is called once per frame
	void Update()
	{
		transform.position += direction * speed * Time.deltaTime;
	}

	void OnTriggerEnter2D(Collider2D pOther)
	{
		Debug.Log(pOther.tag);
		if (pOther.tag == "Player")
		{
			pOther.GetComponent<PlayerNetwork>().SpellHit(direction);
		}
		else if (pOther.tag == "item" || pOther.tag == "fiole")
		{
			pOther.GetComponent<ItemScript>().AccioItem(direction);
		}
		KillProjectile();
	}

	private IEnumerator WaitAndDie(float pLifeTime)
	{
		yield return new WaitForSeconds(pLifeTime);
		KillProjectile();
	}

	public void KillProjectile()
	{
		Destroy(gameObject);
	}
}
