#if UNITY_5 && (!UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3) || UNITY_2017
#define UNITY_MIN_5_4
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PlayerNetwork : Photon.PunBehaviour, IPunObservable
{

	#region Public Variables

	public float speed = 5;
	public Vector3 lastDir;

	public GameObject projectilePrefab;
	public float castingRate;
	public float spellForce;
	public float stunTime;

	[Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
	public static GameObject LocalPlayerInstance;

	#endregion


	#region Private Variables

	private PlayerInfo _playerInfo;
	private UseItemNetwork _useItem;

	private int horizontal;
	private int vertical;
	private IEnumerator stunCoroutine;

	private float nextCast;

	#endregion


	#region MonoBehavior CallBacks

	void Awake()
	{
		//#Important
		//used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
		if (photonView.isMine)
		{
			PlayerNetwork.LocalPlayerInstance = this.gameObject;
		}
		//#Critical
		//we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
		DontDestroyOnLoad(this.gameObject);
	}

	private void Start()
	{
		_playerInfo = gameObject.GetComponent<PlayerInfo>();
		_useItem = gameObject.GetComponent<UseItemNetwork>();

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
		if (!photonView.isMine && PhotonNetwork.connected)
		{
			return;
		}
		if (!_playerInfo.isStun)
		{
			UpdateMovement();
			ItemAction();
			CastSpell();
			//photonView.RPC("CastSpell", PhotonTargets.All);
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

	private void CastSpell()
	{
		if (Input.GetButton("Fire3_P" + _playerInfo.playerNumber) && Time.time > nextCast && !_playerInfo.isHolding)
		{
			photonView.RPC("Shoot", PhotonTargets.All, lastDir);
		}
	}

	[PunRPC]
	private void Shoot(Vector3 pDir){
		nextCast = Time.time + castingRate;

		GameObject projectile;

		if (PhotonNetwork.connected)
			projectile = PhotonNetwork.Instantiate(this.projectilePrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0) as GameObject;
		else
			projectile = Instantiate(projectilePrefab) as GameObject;


		projectile.GetComponent<SpellProjectileNetwork>().direction = pDir;
		projectile.GetComponent<SpellProjectileNetwork>().playerOwner = _playerInfo.playerNumber;
		projectile.transform.position = transform.position + pDir;
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

	private IEnumerator PlayerStun(float stunTime)
	{
		yield return new WaitForSeconds(stunTime);
		_playerInfo.isStun = false;
		_playerInfo.State = "idle";
		Debug.Log("Isn't stun anymore !");
	}

	#endregion


	#region Public Methods

	public void SpellHit(Vector3 pDir)
	{
		if (_playerInfo.isHolding)
		{
			if (PhotonNetwork.connected)
			{
				photonView.RPC("DropOff", PhotonTargets.All);
			}
			else
			{
				_useItem.DropOff();
			}
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
		_playerInfo.State = "stun";
		stunCoroutine = PlayerStun(stunTime);
		StartCoroutine(stunCoroutine);
	}

	#endregion



	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		
	}
}
