using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PlayerPotionAction : MonoBehaviour {

	private PlayerInfo _playerInfo;
	private UseItem _useItem;

	private int horizontal;
	private int vertical;
	private IEnumerator stunCoroutine;


	public float speed = 5;
	public Vector3 lastDir;


	public GameObject projectilePrefab;
	public float castingRate;
	public float spellForce;
	public float stunTime;
	private float nextCast;


	private void Start()
	{
		_playerInfo = gameObject.GetComponent<PlayerInfo>();
		_useItem = gameObject.GetComponent<UseItem>();
		
		lastDir = new Vector3(1,0,0);
	}

	private void Update()
	{
		if (!_playerInfo.isStun) {
			UpdateMovement();
			ItemAction();
			CastSpell();
		}
	}

	private void UpdateMovement()
	{
		horizontal = 0;     //Used to store the horizontal move direction.
		vertical = 0;       //Used to store the vertical move direction.

		//Get input from the input manager and store in horizontal to set x axis move direction
		if (Input.GetAxisRaw("Horizontal_P" + _playerInfo.playerNumber) > 0.5)
		{
			horizontal = 1;
			lastDir = new Vector3(1, 0, 0);
		}
		if (Input.GetAxisRaw("Horizontal_P" + _playerInfo.playerNumber) < -0.5)
		{
			horizontal = -1;
			lastDir = new Vector3(-1, 0, 0);
		}

		//Get input from the input manager and store in vertical to set y axis move direction
		if (Input.GetAxisRaw("Vertical_P" + _playerInfo.playerNumber) > 0.5)
		{
			vertical = 1;
			lastDir = new Vector3(0, 1, 0);
		}
		if (Input.GetAxisRaw("Vertical_P" + _playerInfo.playerNumber) < -0.5)
		{
			vertical = -1;
			lastDir = new Vector3(0, -1, 0);
		}

		transform.position += new Vector3(horizontal, vertical, 0) * speed * Time.deltaTime;

		//Change player orientation
		if(lastDir.x != 0)
			transform.localScale = new Vector3(_playerInfo.originalScale * lastDir.x, transform.localScale.y, transform.localScale.z);
	}

	private void CastSpell()
	{
		if (Input.GetButton("Fire3_P"+_playerInfo.playerNumber) && Time.time > nextCast)
		{
			nextCast = Time.time + castingRate;
			GameObject projectile = Instantiate(projectilePrefab);
			
			projectile.GetComponent<SpellProjectile>().direction = lastDir;
			projectile.transform.position = transform.position + lastDir;
		}
	}

	public void SpellHit(Vector3 dir)
	{
		if (_playerInfo.isHolding)
		{
			_useItem.DropOff();
		}
		if (dir.x != 0)
		{
			gameObject.GetComponent<Rigidbody2D>().AddForce(transform.right * spellForce * dir.x);
		}
		else
		{
			gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * spellForce * dir.y);
		}
		
		Debug.Log("Player "+_playerInfo.playerNumber+" is Stun !");
		_playerInfo.isStun = true;
		stunCoroutine = PlayerStun(stunTime);
		StartCoroutine(stunCoroutine);
	}

	private void ItemAction()
	{
		if (Input.GetButtonDown("Fire1_P"+ _playerInfo.playerNumber))
		{
			if (!_playerInfo.isHolding)
			{
				_useItem.PickItem();
			}
			else
			{
				_useItem.DropItem();
			}
		}
	}

	private IEnumerator PlayerStun(float stunTime)
	{
		yield return new WaitForSeconds(stunTime);
		_playerInfo.isStun = false;
		Debug.Log("Isn't stun anymore !");
	}
}
