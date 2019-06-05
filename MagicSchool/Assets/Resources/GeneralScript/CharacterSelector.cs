using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.OniriqueStudio.MagicSchool
{
	public class CharacterSelector : MonoBehaviour
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
				quitTime = Time.time + quitTimer;
				isLeaving = true;
			}
			if (Input.GetButtonUp("Fire2_P1"))
			{
				isLeaving = false;
			}
		}

		private void CharacterSelection()
		{
			if (Input.GetButtonUp("Fire1_P1"))
			{
				if (CheckJoinedPlayers(1))
				{
					InstantiateCarroussel(1);
				}
				else
				{
					if (gameReady)
					{
						for (int i=0; i<playerPrefabList.Count; i++)
						{
							if (playerPrefabList[i] != null)
							{
								GameManager.Instance.players.Add(playerPrefabList[i]);
							}
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
					InstantiateCarroussel(2);
				}
			}

			if (Input.GetButtonUp("Fire1_P3"))
			{
				if (CheckJoinedPlayers(3))
				{
					InstantiateCarroussel(3);
				}
			}

			if (Input.GetButtonUp("Fire1_P4"))
			{
				if (CheckJoinedPlayers(4))
				{
					InstantiateCarroussel(4);
				}
			}
		}

		private bool CheckJoinedPlayers(int pID)
		{
			bool isAlreadyOn = false;

			int id = pID;

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

				SetJoinedPlayersID(id);

				return true;
			}

			return false;
		}

		public void ConfigurePlayer(int pID, int pController, int pSpriteID)
		{
			GameObject player;

			Debug.Log("on fabrique un joueur offline");
			player = Instantiate(playerPrefab) as GameObject;
			player.GetComponent<PlayerInfo>().ConfigurePlayer(pID, pController, pSpriteID);
			playerPrefabList[pID - 1] = player;

			playersReady++;
		}

		private void InstantiateCarroussel(int pControllerNumber)
		{
			callToActionList[playerID - 1].text = confirmCharacterPhrase;

			GameObject carroussel;

			Debug.Log("Instantiating carroussel id : "+playerID+"; controller : "+pControllerNumber);

			carroussel = Instantiate(carrousselPrefab) as GameObject;
			carroussel.GetComponent<CharacterCarroussel>().SetIDs(playerID, pControllerNumber);
			carrousselPrefabList[playerID - 1] = carroussel;

			
			carrousselList[playerID - 1].enabled = true;

			//playerID++;
		}

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

		public void DestroyCarroussel(int pPlayerID)
		{
			Debug.Log("On détruit le carroussel du joueur : "+pPlayerID);

			Destroy(carrousselPrefabList[pPlayerID - 1]);

			carrousselPrefabList[pPlayerID - 1] = null;
			joinedPlayersID[pPlayerID - 1] = 0;
			carrousselList[pPlayerID - 1].enabled = false;
			callToActionList[pPlayerID - 1].text = activateCharacterPhrase;
		}

		public void DestroyPlayerPrefab(int pPlayerID)
		{
			Debug.Log("On détruit le préfab du joueur : "+pPlayerID);

			Destroy(playerPrefabList[pPlayerID - 1]);
			playerPrefabList[pPlayerID - 1] = null;

			playersReady--;
		}
	}
}
