using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Networking;

public class PlayerNetwork : NetworkBehaviour
{

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

	public Sprite sprite;


	private void Start()
	{
		_playerInfo = gameObject.GetComponent<PlayerInfo>();
		_useItem = gameObject.GetComponent<UseItem>();

		lastDir = new Vector3(1, 0, 0);
	}

	public override void OnStartLocalPlayer()
	{
		gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
	}

	private void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}
		if (!_playerInfo.isStun)
		{
			UpdateMovement();
			ItemAction();
			CmdCastSpell();
		}
	}

	private void UpdateMovement()
	{
		horizontal = 0;     //Used to store the horizontal move direction.
		vertical = 0;       //Used to store the vertical move direction.

		//Get input from the input manager and store in horizontal to set x axis move direction
		if (Input.GetAxisRaw("Horizontal_P" + _playerInfo.playerNumber) > 0.2)
		{
			horizontal = 1;
			lastDir = new Vector3(1, 0, 0);
		}
		if (Input.GetAxisRaw("Horizontal_P" + _playerInfo.playerNumber) < -0.2)
		{
			horizontal = -1;
			lastDir = new Vector3(-1, 0, 0);
		}

		//Get input from the input manager and store in vertical to set y axis move direction
		if (Input.GetAxisRaw("Vertical_P" + _playerInfo.playerNumber) > 0.2)
		{
			vertical = 1;
			lastDir = new Vector3(0, 1, 0);
		}
		if (Input.GetAxisRaw("Vertical_P" + _playerInfo.playerNumber) < -0.2)
		{
			vertical = -1;
			lastDir = new Vector3(0, -1, 0);
		}

		transform.position += new Vector3(horizontal, vertical, 0) * speed * Time.deltaTime;

		//Change player orientation
		if (lastDir.x != 0)
			transform.localScale = new Vector3(_playerInfo.originalScale * lastDir.x, transform.localScale.y, transform.localScale.z);
	}

	[Command]
	void CmdCastSpell()
	{
		Debug.Log("cast");
		if (Input.GetButton("Fire3_P" + _playerInfo.playerNumber) && Time.time > nextCast)
		{
			Debug.Log("cast spell !");
			nextCast = Time.time + castingRate;
			GameObject projectile = Instantiate(projectilePrefab);

			projectile.GetComponent<SpellProjectileNetwork>().direction = lastDir;
			projectile.transform.position = transform.position + lastDir;

			NetworkServer.Spawn(projectile);
		}
	}

	private void ItemAction()
	{
		if (Input.GetButtonDown("Fire1_P" + _playerInfo.playerNumber))
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

	public void SpellHit(Vector3 pDir)
	{
		if (_playerInfo.isHolding)
		{
			_useItem.DropOff();
		}
		if (pDir.x != 0)
		{
			gameObject.GetComponent<Rigidbody2D>().AddForce(transform.right * spellForce * pDir.x);
		}
		else
		{
			gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * spellForce * pDir.y);
		}

		Debug.Log("Player " + _playerInfo.playerNumber + " is Stun !");
		_playerInfo.isStun = true;
		stunCoroutine = PlayerStun(stunTime);
		StartCoroutine(stunCoroutine);
	}

	private IEnumerator PlayerStun(float stunTime)
	{
		yield return new WaitForSeconds(stunTime);
		_playerInfo.isStun = false;
		Debug.Log("Isn't stun anymore !");
	}
}
