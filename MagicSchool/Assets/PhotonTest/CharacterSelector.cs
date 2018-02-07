using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
	public class CharacterSelector : Photon.PunBehaviour, IPunObservable
	{
		public GameObject playerPrefab;
		public List<Text> callToActionList;
		public List<Image> carrousselList;
		public List<Sprite> spriteList;
		
		public string activateCharacterPhrase = "Press Action to Join";
		public string confirmCharacterPhrase = "Press Action to Validate";

		public int playerID = 0;

		private bool p1Joined;
		private bool p2Joined;
		private bool p3Joined;
		private bool p4Joined;

		private bool p1Validate;
		private bool p2Validate;
		private bool p3Validate;
		private bool p4Validate;

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if (PhotonNetwork.connected)
			{
				CharacterSelectionOnline();
			}
			else
			{
				CharacterSelection();
			}
		}

		[PunRPC]
		private void CharacterSelectionOnline()
		{
			if (Input.GetButton("Fire1_P1") && photonView.isMine)
			{
				if (!p1Joined)
				{
					p1Joined = true;

					InstantiatePlayer();

					playerID++;
				}
				else
				{
					if (!p1Validate)
					{
						p1Validate = true;
					}
				}
			}

			if (Input.GetButton("Fire1_P2") && photonView.isMine)
			{
				if (!p2Joined)
				{
					p2Joined = true;

					InstantiatePlayer();

					playerID++;
				}
				else
				{
					if (!p2Validate)
					{
						p2Validate = true;
					}
				}
			}

			if (Input.GetButton("Fire1_P3") && photonView.isMine)
			{
				if (!p3Joined)
				{
					p3Joined = true;

					InstantiatePlayer();

					playerID++;
				}
				else
				{
					if (!p1Validate)
					{
						p1Validate = true;
					}
				}
			}

			if (Input.GetButton("Fire1_P4") && photonView.isMine)
			{
				if (!p4Joined)
				{
					p4Joined = true;

					InstantiatePlayer();

					playerID++;
				}
				else
				{
					if (!p4Validate)
					{
						p4Validate = true;
					}
				}
			}
		}

		private void CharacterSelection()
		{
			if (Input.GetButton("Fire1_P1"))
			{
				if (!p1Joined)
				{
					p1Joined = true;

					InstantiatePlayer();

					playerID++;
				}
				else
				{
					if (!p1Validate)
					{
						p1Validate = true;
					}
				}
			}

			if (Input.GetButton("Fire1_P2"))
			{
				if (!p2Joined)
				{
					p2Joined = true;

					InstantiatePlayer();

					playerID++;
				}
				else
				{
					if (!p2Validate)
					{
						p2Validate = true;
					}
				}
			}

			if (Input.GetButton("Fire1_P3"))
			{
				if (!p3Joined)
				{
					p3Joined = true;

					InstantiatePlayer();

					playerID++;
				}
				else
				{
					if (!p1Validate)
					{
						p1Validate = true;
					}
				}
			}

			if (Input.GetButton("Fire1_P4"))
			{
				if (!p4Joined)
				{
					p4Joined = true;

					InstantiatePlayer();

					playerID++;
				}
				else
				{
					if (!p4Validate)
					{
						p4Validate = true;
					}
				}
			}
		}


		private void InstantiatePlayer()
		{
			callToActionList[playerID].text = confirmCharacterPhrase;

			GameObject player;

			if (PhotonNetwork.connected)
			{
				player = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0) as GameObject;
			}
			else
			{
				player = Instantiate(playerPrefab) as GameObject;
			}
			
			player.GetComponent<SpriteRenderer>().enabled = false;
			player.GetComponent<PlayerInfo>().playerID = playerID;

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
				stream.SendNext(p1Validate);
				stream.SendNext(p2Validate);
				stream.SendNext(p3Validate);
				stream.SendNext(p4Validate);
			}
			else
			{
				// Network player, receive data
				this.playerID = (int)stream.ReceiveNext();
				this.p1Joined = (bool)stream.ReceiveNext();
				this.p2Joined = (bool)stream.ReceiveNext();
				this.p3Joined = (bool)stream.ReceiveNext();
				this.p4Joined = (bool)stream.ReceiveNext();
				this.p1Validate = (bool)stream.ReceiveNext();
				this.p2Validate = (bool)stream.ReceiveNext();
				this.p3Validate = (bool)stream.ReceiveNext();
				this.p4Validate = (bool)stream.ReceiveNext();
			}
		}
	}
}
