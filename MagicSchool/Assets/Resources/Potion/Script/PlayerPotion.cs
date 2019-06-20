#if UNITY_5 && (!UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3) || UNITY_2017
#define UNITY_MIN_5_4
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PlayerPotion : PlayerMovement
{
	public GameObject projectilePrefab;
	public GameObject bubblePrefab;

	public float pickUpRange = 0.5f;
	public float slowedSpeed = 40f;
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

	public bool isHolding, isStun, isPreparing, isProtected;

	public string[] pickupTags;

	private UseItemPotion useItem;

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


	public override void Start()
	{
		base.Start();
		useItem = gameObject.GetComponent<UseItemPotion>();

		pickupTags = new string[] { "item", "chaudron", "dispenser", "fiole" };

		jumpThroughLayerMask = LayerMask.GetMask("jumpThrough");

#if UNITY_MIN_5_4
		//Unity 5.4 has a new scene management. Register a method to call CalledOnLevelWasLoaded.
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
		{
			this.CalledOnLevelWasLoaded(scene.buildIndex);
		};
#endif
	}

	public override void Update()
	{
		base.Update();

		if (!isStun)
		{
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


	private void ItemAction()
	{
		if (isPreparing)
		{
			return;
		}
		if (Input.GetButtonDown("Fire1_P" + playerInfo.playerController))
		{
			animator.SetTrigger("playerGrab");
		}
	}

	public void PickDropItem()
	{
		if (!isHolding && !cantCarry)
		{
			useItem.PickItem();
		}
		else
		{
			useItem.DropItem();
		}
	}

	private void PrepareItem()
	{
		if (isStun || isHolding || cantCraft || isJumping || isFalling)
		{
			isPreparing = false;
			return;
		}
		if (Input.GetButtonDown("Fire2_P" + playerInfo.playerController))
		{
			canMove = false;
			canJump = false;
			isPreparing = true;
			animator.SetBool("playerCook", true);
			animator.SetBool("playerMove", false);

			if (grounded)
				playerInfo.rb2d.velocity = Vector2.zero;
		}
		if (Input.GetButtonUp("Fire2_P" + playerInfo.playerController))
		{
			canMove = true;
			canJump = true;
			isPreparing = false;
			animator.SetBool("playerCook", false);
		}
		if (Input.GetButton("Fire2_P" + playerInfo.playerController))
		{
			useItem.PrepareItem();
		}
	}

	private void CastSpell()
	{
		if (isPreparing || isSilenced )
		{
			return;
		}
		if (Input.GetButton("Fire3_P" + playerInfo.playerController) && Time.time > nextCastShoot)
		{
			Shoot(lastDir);
		}
		else if (Input.GetButton("Fire4_P" + playerInfo.playerController) && Time.time > nextCastProtect)
		{
			Protect();
		}
	}

	private void Shoot(Vector3 pDir)
	{
		nextCastShoot = Time.time + shootCastRate;
		animator.SetTrigger("playerShoot");
		shootingDir = pDir;
	}

	public void InstantiateBlast()
	{
		GameObject projectile;
		projectile = Instantiate(projectilePrefab) as GameObject;
		projectile.GetComponent<SpellProjectileScript>().direction = shootingDir;
		projectile.GetComponent<SpellProjectileScript>().playerOwner = playerInfo.playerTeam;
		projectile.transform.position = new Vector3(transform.position.x + shootingDir.x, (transform.position.y + playerInfo.playerHeight/2) + shootingDir.y, transform.position.z + shootingDir.z);
		//projectile.transform.position = transform.position + shootingDir;
	}

	private void Protect()
	{
		nextCastProtect = Time.time + protectCastRate;

		animator.SetTrigger("playerProtect");
		isProtected = true;
		playerInfo.State = "protected";
		protectCoroutine = PlayerProtected(protectedTime);
		StartCoroutine(protectCoroutine);
	}

	public void InstantiateProtection()
	{
		bubblePrefab.SetActive(true);
		bubblePrefab.GetComponentInChildren<Animator>().enabled = true;
	}

	private void DestroyBubble()
	{
		isProtected = false;
		bubblePrefab.SetActive(false);
		bubblePrefab.GetComponentInChildren<Animator>().enabled = false;
	}

	public void SpellHit(Vector3 pDir)
	{
		if (isProtected)
		{
			DestroyBubble();
			return;
		}
		if (isStun)
		{
			return;
		}
		if (isHolding)
		{
			useItem.DropOff();
		}

		Debug.Log("Player " + playerInfo.playerController + " is Stun !");
		isStun = true;
		canMove = false;
		canJump = false;
		playerInfo.rb2d.velocity = new Vector2(playerInfo.rb2d.velocity.x, playerInfo.rb2d.velocity.y * 0.2f);
		animator.SetTrigger("playerHit");
		animator.SetBool("playerStun", true);
		animator.SetBool("playerMove", false);
		animator.SetBool("playerJump", false);
		animator.SetBool("playerFall", false);
		animator.SetBool("playerCook", false);
		playerInfo.State = "stun";
		stunCoroutine = PlayerStun(stunTime);
		StartCoroutine(stunCoroutine);

		playerInfo.rb2d.AddForce(new Vector2(spellForce * pDir.x, playerInfo.rb2d.velocity.y), ForceMode2D.Force);
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
		if (isHeavy)
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
		canMove = true;
		canJump = true;
		isStun = false;
		playerInfo.State = "idle";
		Debug.Log("Isn't stun anymore !");
	}

	private IEnumerator PlayerProtected(float time)
	{
		yield return new WaitForSeconds(time);
		DestroyBubble();
		playerInfo.State = "idle";
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
