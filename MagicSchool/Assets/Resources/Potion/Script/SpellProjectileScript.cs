using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectileScript : MonoBehaviour
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
			transform.localScale = new Vector3(transform.localScale.x * direction.y, transform.localScale.y, transform.localScale.z);
		}

		coroutine = WaitAndDie(lifeSpan);
		StartCoroutine(coroutine);
	}

	// Update is called once per frame
	void Update()
	{
		transform.position += direction * speed * Time.deltaTime;
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
	}

	void OnTriggerEnter2D(Collider2D pOther)
	{
		Debug.Log(pOther.tag);
		if (pOther.tag == "Player")
		{
			Debug.Log("On touche un joueur !");
			pOther.GetComponent<PlayerPotion>().SpellHit(direction);
		}
		else if (pOther.tag == "item" || pOther.tag == "fiole")
		{
			Debug.Log("On touche un item !");
			pOther.GetComponent<AttractionScript>().Accio(direction);
		}
		else if (pOther.tag == "chaudron")
		{
			Debug.Log("On touche un chaudron !");
			ChaudronScript chaudronScript = pOther.GetComponent<ChaudronScript>();

			chaudronScript.ControlFire(playerOwner);
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
