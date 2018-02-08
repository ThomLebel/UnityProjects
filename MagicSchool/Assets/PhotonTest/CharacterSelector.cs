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

		public bool p1Joined;
		public bool p2Joined;
		public bool p3Joined;
		public bool p4Joined;


		static public CharacterSelector Instance;

		// Use this for initialization
		void Start()
		{
			Instance = this;
		}

		// Update is called once per frame
		void Update()
		{
			if (PhotonNetwork.connected && photonView.isMine)
			{
				CharacterSelectionOnline();
				//SelectSpriteOnline();
			}
			else
			{
				CharacterSelection();
				//SelectSpriteOffline();
			}
		}

		[PunRPC]
		private void CharacterSelectionOnline()
		{
			if (photonView.isMine)
			{
				if (Input.GetButtonUp("Fire1_P1"))
				{
					if (!p1Joined)
					{
						p1Joined = true;

						//InstantiateCarroussel(1);
						photonView.RPC("InstantiateCarroussel", PhotonTargets.All, 1);
					}
				}

				if (Input.GetButtonUp("Fire1_P2"))
				{
					if (!p2Joined)
					{
						p2Joined = true;

						//InstantiateCarroussel(2);
						photonView.RPC("InstantiateCarroussel", PhotonTargets.All, 2);
					}
				}

				if (Input.GetButtonUp("Fire1_P3"))
				{
					if (!p3Joined)
					{
						p3Joined = true;

						//InstantiateCarroussel(3);
						photonView.RPC("InstantiateCarroussel", PhotonTargets.All, 3);
					}
				}

				if (Input.GetButtonUp("Fire1_P4"))
				{
					if (!p4Joined)
					{
						p4Joined = true;

						//InstantiateCarroussel(4);
						photonView.RPC("InstantiateCarroussel", PhotonTargets.All, 4);
					}
				}
			}
		}

		private void CharacterSelection()
		{
			if (Input.GetButtonUp("Fire1_P1"))
			{
				if (!p1Joined)
				{
					p1Joined = true;

					InstantiateCarroussel(1);
				}
			}

			if (Input.GetButtonUp("Fire1_P2"))
			{
				if (!p2Joined)
				{
					p2Joined = true;

					InstantiateCarroussel(2);
				}
			}

			if (Input.GetButtonUp("Fire1_P3"))
			{
				if (!p3Joined)
				{
					p3Joined = true;

					InstantiateCarroussel(3);
				}
			}

			if (Input.GetButtonUp("Fire1_P4"))
			{
				if (!p4Joined)
				{
					p4Joined = true;

					InstantiateCarroussel(4);
				}
			}
		}

		[PunRPC]
		public void ConfigurePlayer(int pID, int pController, int pSpriteID)
		{
			GameObject player;

			if (PhotonNetwork.connected && PhotonNetwork.isMasterClient)
			{
				player = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0) as GameObject;
			}
			else
			{
				player = Instantiate(playerPrefab) as GameObject;
			}

			player.GetComponent<SpriteRenderer>().enabled = false;
			player.GetComponent<PlayerNetwork>().enabled = false;
			player.GetComponent<PlayerInfo>().playerID = pID;
			player.GetComponent<PlayerInfo>().playerNumber = pController;
			player.GetComponent<PlayerInfo>().playerSprite = spriteList[pSpriteID];
		}


		private void InstantiateCarroussel(int pControllerNumber)
		{
			callToActionList[playerID - 1].text = confirmCharacterPhrase;

			GameObject carroussel;

			Debug.Log("Instantiating carroussel id : "+playerID+"; controller : "+pControllerNumber);

			if (PhotonNetwork.connected)
			{
				carroussel = PhotonNetwork.Instantiate(carrousselPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0) as GameObject;
				carroussel.GetComponent<PhotonView>().RPC("SetIDs", PhotonTargets.All, playerID, pControllerNumber);
			}
			else
			{
				carroussel = Instantiate(carrousselPrefab) as GameObject;
				carroussel.GetComponent<CharacterCarroussel>().playerID = playerID;
				carroussel.GetComponent<CharacterCarroussel>().playerNumber = pControllerNumber;
			}

			carrousselList[playerID - 1].enabled = true;

			

			playerID++;
		}



		void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.isWriting)
			{
				// We own this player: send the others our data
				stream.SendNext(playerID);
				stream.SendNext(p1Joined);
				stream.SendNext(p2Joined);
				stream.SendNext(p3Joined);
				stream.SendNext(p4Joined);
			}
			else
			{
				// Network player, receive data
				this.playerID = (int)stream.ReceiveNext();
				this.p1Joined = (bool)stream.ReceiveNext();
				this.p2Joined = (bool)stream.ReceiveNext();
				this.p3Joined = (bool)stream.ReceiveNext();
				this.p4Joined = (bool)stream.ReceiveNext();
			}
		}
	}
}
