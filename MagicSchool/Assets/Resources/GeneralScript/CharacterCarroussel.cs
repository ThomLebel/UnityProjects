using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.OniriqueStudio.MagicSchool
{
	public class CharacterCarroussel : MonoBehaviour
	{
		[Tooltip("Numero du contrôleur du joueur")]
		public int playerController;
		[Tooltip("Numero du joueur")]
		public int playerID;

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
			if (Input.GetButtonUp("Fire1_P" + playerController) && !validate)
			{
				validate = true;
				Debug.Log("On valide notre choix de skin ! "+ CharacterSelector.Instance.spriteList[currentSprite].name);
				CharacterSelector.Instance.ConfigurePlayer(playerID, playerController, currentSprite);
			}
			if (!validate)
			{
				CheckSlide();
				if (Input.GetButtonUp("Fire2_P" + playerController))
				{
					CharacterSelector.Instance.DestroyCarroussel(playerID);
				}
			}
			else
			{
				if (Input.GetButtonUp("Fire2_P" + playerController))
				{
					validate = false;

					CharacterSelector.Instance.DestroyPlayerPrefab(playerID);
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
					Slide(1);
				}
				if (Input.GetAxisRaw("Horizontal_P" + playerController) < -0.2)
				{
					Debug.Log("Slide left for player " + playerID);
					Slide(-1);
				}
			}
		}

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
			CharacterSelector.Instance.carrousselList[playerID - 1].SetNativeSize();

			nextSlide = Time.time + slideRate;
		}

		public void SetIDs(int pID, int pController)
		{
			playerID = pID;
			playerController = pController;
		}
	}
}

