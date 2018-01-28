using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : MonoBehaviour {

	public int speed;
	public Vector3 direction;
	public float lifeSpan;

	private IEnumerator coroutine;

	private void Start()
	{
		if(direction.x != 0)
			transform.localScale = new Vector3(transform.localScale.x * direction.x, transform.localScale.y, transform.localScale.z);
		else
		{
			transform.Rotate(0,0,90);
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * direction.y, transform.localScale.z);
		}

		coroutine = WaitAndDie(lifeSpan);
		StartCoroutine(coroutine);
	}

	// Update is called once per frame
	void Update () {
		transform.position += direction * speed * Time.deltaTime;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log(other.tag);
		if (other.tag == "Player")
		{
			other.GetComponent<PlayerPotionAction>().SpellHit(direction);
		}
		else if (other.tag == "item" || other.tag == "fiole")
		{
			other.GetComponent<ItemScript>().AccioItem(direction);
		}
		KillProjectile();
	}

	private IEnumerator WaitAndDie(float lifeTime)
	{
		yield return new WaitForSeconds(lifeTime);
		KillProjectile();
	}

	public void KillProjectile()
	{
		Destroy(gameObject);
	}
}
