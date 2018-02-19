using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectileNetwork : Photon.PunBehaviour
{
	public Vector3 direction;

	public int speed;
	public float lifeSpan;
	public int playerOwner;

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
		//transform.position += direction * speed * Time.deltaTime;
		transform.position += new Vector3(direction.x*speed* Time.deltaTime, direction.y * speed * Time.deltaTime, direction.y*-1);
	}

	void OnTriggerEnter2D(Collider2D pOther)
	{
		Debug.Log(pOther.tag);
		if (pOther.tag == "Player")
		{
			Debug.Log("On touche un joueur !");
			pOther.GetComponent<PlayerNetwork>().SpellHit(direction);
		}
		else if (pOther.tag == "item" || pOther.tag == "fiole")
		{
			Debug.Log("On touche un item !");
			pOther.GetComponent<ItemScript>().AccioItem(direction);
		}
		else if (pOther.tag == "chaudron")
		{
			Debug.Log("On touche un chaudron !");
			ChaudronScriptNetwork chaudronScript = pOther.GetComponent<ChaudronScriptNetwork>();
			if (PhotonNetwork.connected)
			{
				pOther.GetComponent<PhotonView>().RPC("ControlFire", PhotonTargets.All, playerOwner);
			}
			else
			{
				chaudronScript.ControlFire(playerOwner);
			}
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
