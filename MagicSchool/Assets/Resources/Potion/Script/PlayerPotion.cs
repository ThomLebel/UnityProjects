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
	public float originalSpeed = 150f;
	public float speed;
	public float slowedSpeed = 40f;
	public float jumpTakeOff = 40f;
	public Vector3 lastDir;
	public float inverted = 1f;

	public float shootCastRate;
	public float protectCastRate;
	public float spellForce;
	public float stunTime;
	public float protectedTime;
	public float invertedTime;
	public float slowedTime;
	public float silenceTime;
	public float heavyTime;
	public float uncarryTime;
	public float uncraftTime;

	private PlayerInfo _playerInfo;
	private UseItemPotion _useItem;

	private float horizontal;
	private float vertical;
	private float safeSpot = 0.2f;
	[SerializeField]
	private bool grounded = false;
	[SerializeField]
	private bool isJumping = false;
	[SerializeField]
	private bool isFalling = false;
	[SerializeField]
	private bool isInverted = false;
	[SerializeField]
	private bool isSlowed = false;
	[SerializeField]
	private bool isSilenced = false;
	[SerializeField]
	private bool isHeavy = false;
	[SerializeField]
	private bool cantCarry = false;
	[SerializeField]
	private bool cantCraft = false;

	private Collider2D platformCollider;
	private IEnumerator stunCoroutine;
	private IEnumerator protectCoroutine;
	private IEnumerator invertCoroutine;
	private IEnumerator slowCoroutine;
	private IEnumerator silenceCoroutine;
	private IEnumerator heavyCoroutine;
	private IEnumerator uncarryCoroutine;
	private IEnumerator uncraftCoroutine;

	private float nextCastShoot;
	private float nextCastProtect;
	private Vector3 shootingDir;

	private CapsuleCollider2D playerCollider;
	private Animator animator;


	void Awake()
	{
		playerCollider = gameObject.GetComponent<CapsuleCollider2D>();
		animator = gameObject.GetComponentInChildren<Animator>();
	}

	private void Start()
	{
		_playerInfo = gameObject.GetComponent<PlayerInfo>();
		_useItem = gameObject.GetComponent<UseItemPotion>();

		lastDir = new Vector3(1, 0, 0);
		speed = originalSpeed;


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

		horizontal = Input.GetAxisRaw("Horizontal_P" + _playerInfo.playerController) * inverted;     //Used to store the horizontal move direction.
		vertical = Input.GetAxisRaw("Vertical_P" + _playerInfo.playerController);       //Used to store the vertical move direction.

		if (Math.Abs(horizontal) > safeSpot)
		{
			if(!isJumping)
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

		if (_playerInfo.rb2d.velocity.y > 0)
		{
			animator.SetBool("playerJump", true);
		}else if (_playerInfo.rb2d.velocity.y < 0)
		{
			animator.SetBool("playerJump", false);
			animator.SetBool("playerFall", true);
		}
		else
		{
			animator.SetBool("playerJump", false);
			animator.SetBool("playerFall", false);
		}

		if (vertical > safeSpot && !isJumping && grounded && !isHeavy)
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
				
				if (ray.collider != null)
				{
					platformCollider = ray.collider;
					playerCollider.isTrigger = true;
					isFalling = true;
				}
			}
		}

		if (isFalling)
		{
			if (!playerCollider.IsTouching(platformCollider))
			{
				isFalling = false;
				platformCollider = null;
				playerCollider.isTrigger = false;
			}
		}

		if (lastDir.x != 0)
		{
			_playerInfo.playerBody.transform.localScale = new Vector3(_playerInfo.originalScale * lastDir.x, transform.localScale.y, transform.localScale.z);
		}
	}

	private void FixedUpdate()
	{
		if (!_playerInfo.isStun && _playerInfo.canMove)
		_playerInfo.rb2d.velocity = new Vector2(horizontal * speed * Time.fixedDeltaTime, _playerInfo.rb2d.velocity.y);
	}

	private void ItemAction()
	{
		if (Input.GetButtonDown("Fire1_P" + _playerInfo.playerController))
		{
			if (!_playerInfo.isHolding && !cantCarry)
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
		if (_playerInfo.isStun || _playerInfo.isHolding || cantCraft)
		{
			_playerInfo.isPreparing = false;
			return;
		}
		if (Input.GetButtonDown("Fire2_P" + _playerInfo.playerController))
		{
			if(grounded)
				_playerInfo.rb2d.velocity = Vector2.zero;

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
		if (_playerInfo.isHolding || _playerInfo.isPreparing || isSilenced)
		{
			return;
		}
		if (Input.GetButton("Fire3_P" + _playerInfo.playerController) && Time.time > nextCastShoot)
		{
			if (grounded)
				_playerInfo.rb2d.velocity = Vector2.zero;

			Shoot(lastDir);
		}
		else if (Input.GetButton("Fire4_P" + _playerInfo.playerController) && Time.time > nextCastProtect)
		{
			if (grounded)
				_playerInfo.rb2d.velocity = Vector2.zero;

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

		Debug.Log("Player " + _playerInfo.playerController + " is Stun !");
		animator.SetTrigger("playerHit");
		animator.SetBool("playerStun", true);
		animator.SetBool("playerMove", false);
		animator.SetBool("playerCook", false);
		_playerInfo.isStun = true;
		_playerInfo.State = "stun";
		stunCoroutine = PlayerStun(stunTime);
		StartCoroutine(stunCoroutine);

		_playerInfo.rb2d.AddForce(new Vector2(spellForce * pDir.x, _playerInfo.rb2d.velocity.y), ForceMode2D.Force);
	}

	public void InvertDirection()
	{
		if (isInverted)
		{
			return;
		}

		isInverted = true;
		inverted = -1f;

		invertCoroutine = PlayerInverted(invertedTime);
		StartCoroutine(invertCoroutine);
	}

	public void SlowSpeed()
	{
		if (isSlowed)
		{
			return;
		}

		isSlowed = true;
		speed = slowedSpeed;

		slowCoroutine = PlayerSlowed(invertedTime);
		StartCoroutine(slowCoroutine);
	}

	public void SilencePlayer()
	{
		if (isSilenced)
		{
			return;
		}

		isSilenced = true;

		silenceCoroutine = PlayerSilenced(silenceTime);
		StartCoroutine(silenceCoroutine);
	}

	public void HeavyPlayer()
	{
		if (isSilenced)
		{
			return;
		}

		isHeavy = true;

		heavyCoroutine = PlayerHeavy(heavyTime);
		StartCoroutine(heavyCoroutine);
	}

	public void PlayerCantCarry()
	{
		if (cantCarry)
		{
			return;
		}

		cantCarry = true;

		uncarryCoroutine = CantCarry(uncarryTime);
		StartCoroutine(uncarryCoroutine);
	}

	public void PlayerCantCraft()
	{
		if (cantCraft)
		{
			return;
		}

		cantCraft = true;

		uncraftCoroutine = CantCraft(uncraftTime);
		StartCoroutine(uncraftCoroutine);
	}

	private IEnumerator PlayerStun(float time)
	{
		yield return new WaitForSeconds(time);
		animator.SetBool("playerStun", false);
		_playerInfo.canMove = true;
		_playerInfo.isStun = false;
		_playerInfo.State = "idle";
		Debug.Log("Isn't stun anymore !");
	}

	private IEnumerator PlayerProtected(float time)
	{
		yield return new WaitForSeconds(time);
		DestroyBubble();
		_playerInfo.State = "idle";
		Debug.Log("Isn't protected anymore !");
	}

	private IEnumerator PlayerInverted(float time)
	{
		yield return new WaitForSeconds(time);
		inverted = 1f;
		isInverted = false;
	}

	private IEnumerator PlayerSlowed(float time)
	{
		yield return new WaitForSeconds(time);
		speed = originalSpeed;
		isSlowed = false;
	}

	private IEnumerator PlayerSilenced(float time)
	{
		yield return new WaitForSeconds(time);

		isSilenced = false;
	}

	private IEnumerator PlayerHeavy(float time)
	{
		yield return new WaitForSeconds(time);

		isHeavy = false;
	}

	private IEnumerator CantCarry(float time)
	{
		yield return new WaitForSeconds(time);

		cantCarry = false;
	}

	private IEnumerator CantCraft(float time)
	{
		yield return new WaitForSeconds(time);

		cantCraft = false;
	}
}
