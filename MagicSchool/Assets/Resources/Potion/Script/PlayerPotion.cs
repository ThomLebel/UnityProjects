#if UNITY_5 && (!UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3) || UNITY_2017
#define UNITY_MIN_5_4
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PlayerPotion : MonoBehaviour
{

	#region Public Variables

	public float speed = 5;
	public Vector3 lastDir;

	public GameObject projectilePrefab;
	public float shootCastRate;
	public float protectCastRate;
	public float spellForce;
	public float stunTime;
	public float protectedTime;

	#endregion


	#region Private Variables

	private PlayerInfo _playerInfo;
	private UseItemPotion _useItem;

	private int horizontal;
	private int vertical;
	private IEnumerator stunCoroutine;
	private IEnumerator protectCoroutine;

	private float nextCastShoot;
	private float nextCastProtect;

	private SpriteRenderer spriteRenderer;
	private Rigidbody2D rb2d;

	#endregion


	#region MonoBehavior CallBacks

	void Awake()
	{
		//#Critical
		//we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
		DontDestroyOnLoad(this.gameObject);

		spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();

		rb2d = gameObject.GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		_playerInfo = gameObject.GetComponent<PlayerInfo>();
		_useItem = gameObject.GetComponent<UseItemPotion>();

		lastDir = new Vector3(1, 0, 0);


#if UNITY_MIN_5_4
		//Unity 5.4 has a new scene management. Register a method to call CalledOnLevelWasLoaded.
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
		{
			this.CalledOnLevelWasLoaded(scene.buildIndex);
		};
#endif
	}

	private void Update()
	{
		if (!_playerInfo.isStun)
		{
			UpdateMovement();
			//Action 1
			ItemAction();
			//Action 2
			PrepareItem();
			//Action 3
			CastSpell();
		}
	}

#if !UNITY_MIN_5_4
	/// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
	void OnLevelWasLoaded(int level){
		this.CalledOnLevelWasLoaded(level);
	}
#endif

	void CalledOnLevelWasLoaded(int level)
	{
		//check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
		transform.position = new Vector3(0f, 0f, 0f);
	}

	#endregion


	#region Private Methods

	private void UpdateMovement()
	{
		float newX = transform.position.x;
		float newY = transform.position.y;
		float newZ = transform.position.z;

		horizontal = 0;     //Used to store the horizontal move direction.
		vertical = 0;       //Used to store the vertical move direction.

		//Get input from the input manager and store in horizontal to set x axis move direction
		if (Input.GetAxisRaw("Horizontal_P" + _playerInfo.playerController) > 0.2)
		{
			horizontal = 1;
			lastDir = new Vector3(1, 0, 0);
		}
		if (Input.GetAxisRaw("Horizontal_P" + _playerInfo.playerController) < -0.2)
		{
			horizontal = -1;
			lastDir = new Vector3(-1, 0, 0);
		}

		//Get input from the input manager and store in vertical to set y axis move direction
		if (Input.GetAxisRaw("Vertical_P" + _playerInfo.playerController) > 0.2)
		{
			vertical = 1;
			lastDir = new Vector3(0, 1, 1);
		}
		if (Input.GetAxisRaw("Vertical_P" + _playerInfo.playerController) < -0.2)
		{
			vertical = -1;
			lastDir = new Vector3(0, -1, -1);
		}

		//Vector2 movement = new Vector2(horizontal, vertical);
		//rb2d.AddForce(movement * speed);


		//newX = transform.position.x;
		//newY = transform.position.y;
		newX += horizontal * speed * Time.deltaTime;
		newY += vertical * speed * Time.deltaTime;
		newZ = newY;

		transform.position = new Vector3(newX, newY, newZ);


		//Vector3 newPos = new Vector3(0,0,0);
		//newPos += new Vector3(horizontal, vertical, 0) * speed * Time.deltaTime;
		//transform.position += new Vector3(horizontal, vertical, 0) * speed * Time.deltaTime;
		//transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);


		//Change player orientation
		if (lastDir.x != 0)
		{
			spriteRenderer.transform.localScale = new Vector3(_playerInfo.originalScale * lastDir.x, spriteRenderer.transform.localScale.y, spriteRenderer.transform.localScale.z);
		}
		_playerInfo.itemLocation.localPosition = lastDir * _playerInfo.itemOffset;
	}

	private void ItemAction()
	{
		if (Input.GetButtonDown("Fire1_P" + _playerInfo.playerController))
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

	private void PrepareItem()
	{
		if (_playerInfo.isStun)
		{
			_playerInfo.isPreparing = false;
			return;
		}
		if (Input.GetButtonDown("Fire2_P" + _playerInfo.playerController))
		{
			_playerInfo.isPreparing = true;
		}
		if (Input.GetButtonUp("Fire2_P" + _playerInfo.playerController))
		{
			_playerInfo.isPreparing = false;
		}
		if (Input.GetButton("Fire2_P" + _playerInfo.playerController))
		{
			_useItem.PrepareItem();
		}
	}

	private void CastSpell()
	{
		if (_playerInfo.isHolding || _playerInfo.isPreparing)
		{
			return;
		}
		if (Input.GetButton("Fire3_P" + _playerInfo.playerController) && Time.time > nextCastShoot)
		{
			Shoot(lastDir);
		}
		else if (Input.GetButton("Fire4_P" + _playerInfo.playerController) && Time.time > nextCastProtect)
		{
			Protect();
		}
	}

	private void Shoot(Vector3 pDir)
	{
		nextCastShoot = Time.time + shootCastRate;

		GameObject projectile;

		projectile = Instantiate(projectilePrefab) as GameObject;

		projectile.GetComponent<SpellProjectileScript>().direction = pDir;
		projectile.GetComponent<SpellProjectileScript>().playerOwner = _playerInfo.playerID;
		projectile.transform.position = transform.position + pDir;
	}

	private void Protect()
	{
		nextCastProtect = Time.time + protectCastRate;
		
		Debug.Log("Player " + _playerInfo.playerController + " is Protected !");
		_playerInfo.isProtected = true;
		_playerInfo.State = "protected";
		protectCoroutine = PlayerProtected(protectedTime);
		StartCoroutine(protectCoroutine);
	}

	private IEnumerator PlayerStun(float stunTime)
	{
		yield return new WaitForSeconds(stunTime);
		_playerInfo.isStun = false;
		_playerInfo.State = "idle";
		Debug.Log("Isn't stun anymore !");
	}

	private IEnumerator PlayerProtected(float protectedTime)
	{
		yield return new WaitForSeconds(stunTime);
		_playerInfo.isProtected = false;
		_playerInfo.State = "idle";
		Debug.Log("Isn't protected anymore !");
	}

	#endregion


	#region Public Methods

	public void SpellHit(Vector3 pDir)
	{
		if (_playerInfo.isProtected)
		{
			return;
		}
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

		Debug.Log("Player " + _playerInfo.playerController + " is Stun !");
		_playerInfo.isStun = true;
		_playerInfo.State = "stun";
		stunCoroutine = PlayerStun(stunTime);
		StartCoroutine(stunCoroutine);
	}

	#endregion

}
