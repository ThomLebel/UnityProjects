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
	public float speed = 40f;
	public float jumpTakeOff = 40f;
	public Vector3 lastDir;

	public float shootCastRate;
	public float protectCastRate;
	public float spellForce;
	public float stunTime;
	public float protectedTime;

	private PlayerInfo _playerInfo;
	private UseItemPotion _useItem;

	private float horizontal;
	private float vertical;
	private float safeSpot = 0.2f;
	[SerializeField]
	private bool isJumping = false;
	[SerializeField]
	private bool grounded = false;
	private IEnumerator stunCoroutine;
	private IEnumerator protectCoroutine;

	private float nextCastShoot;
	private float nextCastProtect;
	private Vector3 shootingDir;
	
	private SpriteRenderer spriteRenderer;
	private Animator animator;


	void Awake()
	{
		//#Critical
		//we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
		DontDestroyOnLoad(this.gameObject);

		//rb2d = GetComponent<Rigidbody2D>();
		spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
		animator = gameObject.GetComponentInChildren<Animator>();
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

	void Update()
	{
		if (!_playerInfo.isStun)
		{
			if(_playerInfo.canMove)
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


	void UpdateMovement()
	{
		grounded = Physics2D.Linecast(transform.position, _playerInfo.groundCheck.position, _playerInfo.groundLayerMask);

		animator.SetBool("playerMove", false);

		horizontal = Input.GetAxisRaw("Horizontal_P" + _playerInfo.playerController);     //Used to store the horizontal move direction.
		vertical = Input.GetAxisRaw("Vertical_P" + _playerInfo.playerController);       //Used to store the vertical move direction.

		if (Math.Abs(horizontal) > safeSpot)
		{
			animator.SetBool("playerMove", true);

			if (horizontal > safeSpot)
			{
				lastDir = new Vector3(1, 0, 0);
			}
			else if (horizontal < safeSpot * -1)
			{
				lastDir = new Vector3(-1, 0, 0);
			}
		}

		if (vertical > safeSpot && !isJumping && grounded)
		{
			isJumping = true;
			_playerInfo.rb2d.AddForce(new Vector2(0f, jumpTakeOff));
		}
		if (vertical < safeSpot)
		{
			if (grounded)
			{
				isJumping = false;
			}
			if (isJumping && _playerInfo.rb2d.velocity.y > 0)
			{
				_playerInfo.rb2d.velocity = new Vector2(_playerInfo.rb2d.velocity.x, _playerInfo.rb2d.velocity.y * 0.5f);
			}
			if (vertical < safeSpot * -1 && grounded)
			{
				RaycastHit2D ray = Physics2D.Linecast(transform.position, _playerInfo.groundCheck.position, _playerInfo.jumpThroughLayerMask);

				Debug.Log(ray);
			}
		}

		if (lastDir.x != 0)
		{
			spriteRenderer.transform.localScale = new Vector3(_playerInfo.originalScale * lastDir.x, spriteRenderer.transform.localScale.y, spriteRenderer.transform.localScale.z);
			_playerInfo.itemLocation.localPosition = new Vector3(lastDir.x * _playerInfo.itemOffset, _playerInfo.itemLocation.localPosition.y, _playerInfo.itemLocation.localPosition.z);
		}
	}

	private void FixedUpdate()
	{
		_playerInfo.rb2d.velocity = new Vector2(horizontal * speed * Time.fixedDeltaTime, _playerInfo.rb2d.velocity.y);
	}

	private void OnPlayerLanded()
	{
		isJumping = false;
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
			_playerInfo.canMove = false;
			_playerInfo.isPreparing = true;
			animator.SetBool("playerCook", true);
		}
		if (Input.GetButtonUp("Fire2_P" + _playerInfo.playerController))
		{
			_playerInfo.canMove = true;
			_playerInfo.isPreparing = false;
			animator.SetBool("playerCook", false);
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
		_playerInfo.canMove = false;
		animator.SetTrigger("playerShoot");
		shootingDir = pDir;
	}

	public void InstantiateBlast()
	{
		_playerInfo.canMove = true;

		GameObject projectile;
		projectile = Instantiate(_playerInfo.projectilePrefab) as GameObject;
		projectile.GetComponent<SpellProjectileScript>().direction = shootingDir;
		projectile.GetComponent<SpellProjectileScript>().playerOwner = _playerInfo.playerID;
		projectile.transform.position = new Vector3(transform.position.x + shootingDir.x, (transform.position.y + _playerInfo.playerHeight/2) + shootingDir.y, transform.position.z + shootingDir.z);
		//projectile.transform.position = transform.position + shootingDir;
	}

	private void Protect()
	{
		nextCastProtect = Time.time + protectCastRate;

		animator.SetTrigger("playerProtect");
		_playerInfo.canMove = false;
		_playerInfo.isProtected = true;
		_playerInfo.State = "protected";
		protectCoroutine = PlayerProtected(protectedTime);
		StartCoroutine(protectCoroutine);
	}

	public void InstantiateProtection()
	{
		_playerInfo.canMove = true;
		_playerInfo.bubblePrefab.SetActive(true);
		_playerInfo.bubblePrefab.GetComponentInChildren<Animator>().enabled = true;
	}

	private IEnumerator PlayerStun(float stunTime)
	{
		yield return new WaitForSeconds(stunTime);
		animator.SetBool("playerStun", false);
		_playerInfo.canMove = true;
		_playerInfo.isStun = false;
		_playerInfo.State = "idle";
		Debug.Log("Isn't stun anymore !");
	}

	private IEnumerator PlayerProtected(float protectedTime)
	{
		yield return new WaitForSeconds(protectedTime);
		DestroyBubble();
		_playerInfo.State = "idle";
		Debug.Log("Isn't protected anymore !");
	}

	private void DestroyBubble()
	{
		_playerInfo.isProtected = false;
		_playerInfo.bubblePrefab.SetActive(false);
		_playerInfo.bubblePrefab.GetComponentInChildren<Animator>().enabled = false;
	}

	public void SpellHit(Vector3 pDir)
	{
		if (_playerInfo.isProtected)
		{
			DestroyBubble();
			return;
		}
		if (_playerInfo.isStun)
		{
			return;
		}
		if (_playerInfo.isHolding)
		{
			_useItem.DropOff();
		}
		if (pDir.x != 0)
		{
			_playerInfo.rb2d.AddForce(transform.right * spellForce * pDir.x);
		}
		else
		{
			_playerInfo.rb2d.AddForce(transform.up * spellForce * pDir.y);
		}

		Debug.Log("Player " + _playerInfo.playerController + " is Stun !");
		animator.SetTrigger("playerHit");
		animator.SetBool("playerStun", true);
		animator.SetBool("playerMove", false);
		animator.SetBool("playerCook", false);
		_playerInfo.isStun = true;
		_playerInfo.State = "stun";
		stunCoroutine = PlayerStun(stunTime);
		StartCoroutine(stunCoroutine);
	}
}
