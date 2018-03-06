using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.OniriqueStudio.MagicSchool
{
	public class CharacterSelector : Photon.PunBehaviour, IPunObservable
	{
		#region Public Variables
		static public CharacterSelector Instance;

		public GameObject playerPrefab;
		public GameObject carrousselPrefab;
		public List<Text> callToActionList;
		public List<Image> carrousselList;
		public List<Sprite> spriteList;
		public Text startText;

		public string activateCharacterPhrase = "Press Action to Join";
		public string confirmCharacterPhrase = "Press Action to Validate";

		public int playerID = 1;
		public int playersReady = 0;

		public List<int> joinedPlayersID;

		#endregion

		#region Private Variables
		[SerializeField]
		private List<GameObject> playerPrefabList;
		[SerializeField]
		private List<GameObject> carrousselPrefabList;

		private int maxPlayer = 4;
		private bool gameReady = false;

		private bool isLeaving = false;
		private float quitTime = 0f;
		private float quitTimer = 2f;
		#endregion


		// Use this for initialization
		void Start()
		{
			Instance = this;

			startText.enabled = false;

			joinedPlayersID = new List<int>();
			playerPrefabList = new List<GameObject>();
			carrousselPrefabList = new List<GameObject>();

			for (var i=0; i<maxPlayer; i++)
			{
				joinedPlayersID.Add(0);
				playerPrefabList.Add(null);
				carrousselPrefabList.Add(null);
			}
		}

		// Update is called once per frame
		void Update()
		{
			CharacterSelection();

			if (playersReady == playerID)
			{
				gameReady = true;
				startText.enabled = true;
			}
			else
			{
				gameReady = false;
				startText.enabled = false;
			}

			if (Input.GetButton("Fire2_P1") && Time.time >= quitTime && isLeaving)
			{
				GameManager.Instance.LoadArena("Launcher");
			}

			if (Input.GetButtonDown("Fire2_P1"))
			{
				if (PhotonNetwork.connected)
				{
					if (PhotonNetwork.isMasterClient)
					{
						quitTime = Time.time + quitTimer;
						isLeaving = true;
					}
				}
				else
				{
					quitTime = Time.time + quitTimer;
					isLeaving = true;
				}
			}
			if (Input.GetButtonUp("Fire2_P1"))
			{
				if (PhotonNetwork.connected)
				{
					if (PhotonNetwork.isMasterClient)
					{
						isLeaving = false;
					}
				}
				else
				{
					isLeaving = false;
				}
			}
		}

		[PunRPC]
		private void CharacterSelection()
		{
			if (Input.GetButtonUp("Fire1_P1"))
			{
				if (CheckJoinedPlayers(1))
				{
					if (PhotonNetwork.connected)
						photonView.RPC("InstantiateCarroussel", PhotonTargets.All, 1, PhotonNetwork.player.ID);
					else
						InstantiateCarroussel(1, 0);
				}
				else
				{
					if (gameReady)
					{
						if (PhotonNetwork.connected && !PhotonNetwork.isMasterClient)
						{
							return;
						}
						string levelName = "Room for "+playersReady;
						GameManager.Instance.LoadArena(levelName);
					}
				}
			}

			if (Input.GetButtonUp("Fire1_P2"))
			{
				if (CheckJoinedPlayers(2))
				{
					if (PhotonNetwork.connected)
						photonView.RPC("InstantiateCarroussel", PhotonTargets.All, 2, PhotonNetwork.player.ID);
					else
						InstantiateCarroussel(2, 0);
				}
			}

			if (Input.GetButtonUp("Fire1_P3"))
			{
				if (CheckJoinedPlayers(3))
				{
					if (PhotonNetwork.connected)
						photonView.RPC("InstantiateCarroussel", PhotonTargets.All, 3, PhotonNetwork.player.ID);
					else
						InstantiateCarroussel(3, 0);
				}
			}

			if (Input.GetButtonUp("Fire1_P4"))
			{
				if (CheckJoinedPlayers(4))
				{
					if (PhotonNetwork.connected)
						photonView.RPC("InstantiateCarroussel", PhotonTargets.All, 4, PhotonNetwork.player.ID);
					else
						InstantiateCarroussel(4, 0);
				}
			}
		}

		private bool CheckJoinedPlayers(int pID)
		{
			bool isAlreadyOn = false;

			int id = pID;
			if(PhotonNetwork.connected && photonView.isMine)
				id += (PhotonNetwork.player.ID*10);

			//for (int i=0; i< joinedPlayersID.Count; i++)
			for (int i=joinedPlayersID.Count-1; i>-1; i--)
			{
				if(joinedPlayersID[i] == id)
				{
					isAlreadyOn = true;
				}
			}
			if (!isAlreadyOn)
			{
				Debug.Log("Ce joueur n'a pas encore rejoint la party, on l'ajoute");

				if (PhotonNetwork.connected)
					photonView.RPC("SetJoinedPlayersID", PhotonTargets.All, id);
				else
					SetJoinedPlayersID(id);

				return true;
			}

			return false;
		}

		[PunRPC]
		public void ConfigurePlayer(int pID, int pController, int pNetworkID, int pSpriteID)
		{
			GameObject player;

			if (PhotonNetwork.connected)
			{
				if (PhotonNetwork.isMasterClient)
				{
					Debug.Log("on fabrique un joueur online a partir de " + PhotonNetwork.player.ID);
					player = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0) as GameObject;
					player.GetComponent<PhotonView>().RPC("ConfigurePlayer", PhotonTargets.All, pID, pController, pNetworkID, pSpriteID);
					playerPrefabList[pID - 1] = player;
				}
			}
			else
			{
				Debug.Log("on fabrique un joueur offline");
				player = Instantiate(playerPrefab) as GameObject;
				player.GetComponent<PlayerInfo>().ConfigurePlayer(pID, pController, pNetworkID, pSpriteID);
				playerPrefabList[pID - 1] = player;
			}

			playersReady++;
		}

		[PunRPC]
		private void InstantiateCarroussel(int pControllerNumber, int pNetworkID)
		{
			callToActionList[playerID - 1].text = confirmCharacterPhrase;

			GameObject carroussel;

			Debug.Log("Instantiating carroussel id : "+playerID+"; controller : "+pControllerNumber+"; network ID : "+pNetworkID);

			if (PhotonNetwork.connected)
			{
				if (PhotonNetwork.isMasterClient)
				{
					carroussel = PhotonNetwork.Instantiate(carrousselPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0) as GameObject;
					carroussel.GetComponent<PhotonView>().RPC("SetIDs", PhotonTargets.All, playerID, pControllerNumber, pNetworkID);
					carrousselPrefabList[playerID - 1] = carroussel;
				}
			}
			else
			{
				carroussel = Instantiate(carrousselPrefab) as GameObject;
				carroussel.GetComponent<CharacterCarroussel>().SetIDs(playerID, pControllerNumber, pNetworkID);
				carrousselPrefabList[playerID - 1] = carroussel;
			}

			
			carrousselList[playerID - 1].enabled = true;

			//playerID++;
		}

		[PunRPC]
		private void SetJoinedPlayersID(int pID)
		{
			Debug.Log("on ajoute l'id du joueur à la liste");
			playerID = 1;
			for (int i = joinedPlayersID.Count - 1; i > -1; i--)
			{
				if (joinedPlayersID[i] == 0)
				{
					playerID = i + 1;
				}
			}
			joinedPlayersID[playerID-1] = pID;
		}

		[PunRPC]
		public void DestroyCarroussel(int pPlayerID)
		{
			Debug.Log("On détruit le carroussel du joueur : "+pPlayerID);
			if (PhotonNetwork.connected)
			{
				if (PhotonNetwork.isMasterClient)
				{
					PhotonNetwork.Destroy(carrousselPrefabList[pPlayerID - 1]);
				}
			}
			else
			{
				Destroy(carrousselPrefabList[pPlayerID - 1]);
			}

			carrousselPrefabList[pPlayerID - 1] = null;
			joinedPlayersID[pPlayerID - 1] = 0;
			carrousselList[pPlayerID - 1].enabled = false;
			callToActionList[pPlayerID - 1].text = activateCharacterPhrase;
		}

		[PunRPC]
		public void DestroyPlayerPrefab(int pPlayerID)
		{
			Debug.Log("On détruit le préfab du joueur : "+pPlayerID);
			if (PhotonNetwork.connected)
			{
				if (PhotonNetwork.isMasterClient)
				{
					PhotonNetwork.Destroy(playerPrefabList[pPlayerID-1]);
					playerPrefabList[pPlayerID - 1] = null;
				}
			}
			else
			{
				Destroy(playerPrefabList[pPlayerID - 1]);
				playerPrefabList[pPlayerID - 1] = null;
			}

			playersReady--;
		}

		void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.isWriting)
			{
				// We own this player: send the others our data
				stream.SendNext(playerID);
				stream.SendNext(playersReady);
			}
			else
			{
				// Network player, receive data
				this.playerID = (int)stream.ReceiveNext();
				this.playersReady = (int)stream.ReceiveNext();
			}
		}
	}
}
