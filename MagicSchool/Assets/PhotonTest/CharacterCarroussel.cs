using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
	public class CharacterCarroussel : Photon.PunBehaviour, IPunObservable
	{
		[Tooltip("Numero du contrôleur du joueur")]
		public int playerController;
		[Tooltip("Numero du joueur")]
		public int playerID;
		[Tooltip("ID du pc sur le network")]
		public int networkID;

		public float slideRate = 0.5f;
		private float nextSlide;
		private int currentSprite = 0;

		private bool validate = false;


		// Use this for initialization
		void Start()
		{
			Debug.Log("Character Carroussel : ID : "+playerID+"; Controller : "+ playerController);
		}

		// Update is called once per frame
		void Update()
		{
			if (PhotonNetwork.connected)
			{
				if (PhotonNetwork.player.ID != networkID)
				{
					return;
				}
			}


			if (Input.GetButtonUp("Fire1_P" + playerController) && !validate)
			{
				if (PhotonNetwork.connected)
				{
					validate = true;
					Debug.Log("On valide notre choix de skin online !");
					CharacterSelector.Instance.photonView.RPC("ConfigurePlayer", PhotonTargets.All, playerID, playerController, networkID, currentSprite);
				}
				else
				{
					validate = true;
					Debug.Log("On valide notre choix de skin !");
					CharacterSelector.Instance.ConfigurePlayer(playerID, playerController, networkID, currentSprite);
				}
			}
			if (!validate)
			{
				CheckSlide();
				if (Input.GetButtonUp("Fire2_P" + playerController))
				{
					if (PhotonNetwork.connected)
					{
						CharacterSelector.Instance.photonView.RPC("DestroyCarroussel", PhotonTargets.All, playerID);
					}
					else
					{
						CharacterSelector.Instance.DestroyCarroussel(playerID);
					}
				}
			}
			else
			{
				if (Input.GetButtonUp("Fire2_P" + playerController))
				{
					validate = false;
					if (PhotonNetwork.connected)
					{
						CharacterSelector.Instance.photonView.RPC("DestroyPlayerPrefab", PhotonTargets.All, playerID);
					}
					else
					{
						CharacterSelector.Instance.DestroyPlayerPrefab(playerID);
					}
				}
			}
		}

		private void CheckSlide()
		{
			if (Time.time > nextSlide)
			{
				if (Input.GetAxisRaw("Horizontal_P" + playerController) > 0.2)
				{
					Debug.Log("Slide right for player " + playerID);
					if (PhotonNetwork.connected && photonView.isOwnerActive)
					{
						photonView.RPC("Slide", PhotonTargets.All, 1);
					}
					else
					{
						Slide(1);
					}
				}
				if (Input.GetAxisRaw("Horizontal_P" + playerController) < -0.2)
				{
					Debug.Log("Slide left for player " + playerID);
					if (PhotonNetwork.connected && photonView.isOwnerActive)
					{
						photonView.RPC("Slide", PhotonTargets.All, -1);
					}
					else
					{
						Slide(-1);
					}
				}
			}
		}

		[PunRPC]
		private void Slide(int pDir)
		{
			currentSprite += pDir;
			if (currentSprite >= CharacterSelector.Instance.spriteList.Count)
			{
				currentSprite = 0;
			}
			if (currentSprite < 0)
			{
				currentSprite = CharacterSelector.Instance.spriteList.Count - 1;
			}

			CharacterSelector.Instance.carrousselList[playerID - 1].sprite = CharacterSelector.Instance.spriteList[currentSprite];

			nextSlide = Time.time + slideRate;
		}

		[PunRPC]
		public void SetIDs(int pID, int pController, int pNetworkID)
		{
			playerID = pID;
			playerController = pController;
			networkID = pNetworkID;
		}

		void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			
		}
	}
}

