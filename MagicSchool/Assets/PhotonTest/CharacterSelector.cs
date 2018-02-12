using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
	public class CharacterSelector : Photon.PunBehaviour, IPunObservable
	{
		public GameObject playerPrefab;
		public GameObject carrousselPrefab;
		public List<Text> callToActionList;
		public List<Image> carrousselList;
		public List<Sprite> spriteList;

		public string activateCharacterPhrase = "Press Action to Join";
		public string confirmCharacterPhrase = "Press Action to Validate";

		public int playerID = 1;
		
		public List<int> joinedPlayersID;

		private List<GameObject> playerPrefabList;
		private List<GameObject> carrousselPrefabList;

		static public CharacterSelector Instance;

		// Use this for initialization
		void Start()
		{
			Instance = this;
		}

		// Update is called once per frame
		void Update()
		{
			CharacterSelection();
			//SelectSpriteOffline();
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
						InstantiateCarroussel(1, PhotonNetwork.player.ID);
				}
			}

			if (Input.GetButtonUp("Fire1_P2"))
			{
				if (CheckJoinedPlayers(2))
				{
					if (PhotonNetwork.connected)
						photonView.RPC("InstantiateCarroussel", PhotonTargets.All, 2, PhotonNetwork.player.ID);
					else
						InstantiateCarroussel(2, PhotonNetwork.player.ID);
				}
			}

			if (Input.GetButtonUp("Fire1_P3"))
			{
				if (CheckJoinedPlayers(3))
				{
					if (PhotonNetwork.connected)
						photonView.RPC("InstantiateCarroussel", PhotonTargets.All, 3, PhotonNetwork.player.ID);
					else
						InstantiateCarroussel(3, PhotonNetwork.player.ID);
				}
			}

			if (Input.GetButtonUp("Fire1_P4"))
			{
				if (CheckJoinedPlayers(4))
				{
					if (PhotonNetwork.connected)
						photonView.RPC("InstantiateCarroussel", PhotonTargets.All, 4, PhotonNetwork.player.ID);
					else
						InstantiateCarroussel(4, PhotonNetwork.player.ID);
				}
			}
		}

		private bool CheckJoinedPlayers(int pID)
		{
			bool isAlreadyOn = false;

			int id = pID;
			if(PhotonNetwork.connected && photonView.isMine)
				id += (PhotonNetwork.player.ID*10);

			for (int i=0; i< joinedPlayersID.Count; i++)
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
					playerPrefabList.Add(player);
				}
			}
			else
			{
				Debug.Log("on fabrique un joueur offline");
				player = Instantiate(playerPrefab) as GameObject;
				player.GetComponent<PlayerInfo>().ConfigurePlayer(pID, pController, pNetworkID, pSpriteID);
				playerPrefabList.Add(player);
			}
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
					carrousselPrefabList.Add(carroussel);
				}
			}
			else
			{
				carroussel = Instantiate(carrousselPrefab) as GameObject;
				carroussel.GetComponent<CharacterCarroussel>().SetIDs(playerID, pControllerNumber, pNetworkID);
				carrousselPrefabList.Add(carroussel);
			}

			carrousselList[playerID - 1].enabled = true;

			playerID++;
		}

		[PunRPC]
		private void SetJoinedPlayersID(int pID)
		{
			Debug.Log("on ajoute l'id du joueur à la liste");
			joinedPlayersID.Add(pID);
		}


		void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.isWriting)
			{
				// We own this player: send the others our data
				stream.SendNext(playerID);
			}
			else
			{
				// Network player, receive data
				this.playerID = (int)stream.ReceiveNext();
			}
		}
	}
}
