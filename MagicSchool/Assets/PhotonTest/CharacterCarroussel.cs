using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
	public class CharacterCarroussel : Photon.PunBehaviour, IPunObservable
	{
		[Tooltip("Numero du contrôleur du joueur")]
		public int playerNumber;
		[Tooltip("Numero du joueur")]
		public int playerID;

		public float slideRate = 0.5f;
		private float nextSlide;
		private int currentSprite = 0;

		private bool validate = false;


		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if (PhotonNetwork.connected && photonView.isMine)
			{
				if (Input.GetButtonUp("Fire1_P"+ playerNumber) && !validate)
				{
					Debug.Log("On valide notre choix de skin online !");
					validate = true;
					CharacterSelector.Instance.photonView.RPC("ConfigurePlayer", PhotonTargets.All, playerID, playerNumber, currentSprite);
				}
				if (!validate)
				{
					Slide();
				}
			}
			else
			{
				if (Input.GetButtonUp("Fire1_P" + playerNumber) && !validate)
				{
					Debug.Log("On valide notre choix de skin !");
					validate = true;
					CharacterSelector.Instance.ConfigurePlayer( playerID, playerNumber, currentSprite);
				}
				if (!validate)
				{
					Slide();
				}
			}
		}

		private void Slide()
		{
			if (Time.time > nextSlide)
			{
				if (Input.GetAxisRaw("Horizontal_P" + playerNumber) > 0.2)
				{
					Debug.Log("Slide right for player " + playerID);
					currentSprite++;
					if (currentSprite >= CharacterSelector.Instance.spriteList.Count)
					{
						currentSprite = 0;
					}

					CharacterSelector.Instance.carrousselList[playerID-1].sprite = CharacterSelector.Instance.spriteList[currentSprite];

					nextSlide = Time.time + slideRate;
				}
				if (Input.GetAxisRaw("Horizontal_P" + playerNumber) < -0.2)
				{
					Debug.Log("Slide left for player " + playerID);
					currentSprite--;
					if (currentSprite < 0)
					{
						currentSprite = CharacterSelector.Instance.spriteList.Count - 1;
					}

					CharacterSelector.Instance.carrousselList[playerID-1].sprite = CharacterSelector.Instance.spriteList[currentSprite];

					nextSlide = Time.time + slideRate;
				}
			}
		}

		void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			
		}
	}
}

