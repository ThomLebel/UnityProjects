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
		public List<GameObject> playerSelectors;
		public List<GameObject> selectorPrefabList;
		public List<Sprite> skinList;
		public List<Sprite> teamSpriteList;
		public Text startText;

		public List<Sprite> tempSkinList = new List<Sprite>();
		public List<Sprite> tempTeamSpriteList = new List<Sprite>();

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
		private int minTeam = 2;
		private int differentTeam = 0;
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

			for (int s = 0; s < skinList.Count; s++ )
			{
				tempSkinList.Add(skinList[s]);
			}
			for (int t = 0; t < teamSpriteList.Count; t++)
			{
				tempTeamSpriteList.Add(teamSpriteList[t]);
			}


			for (int i=0; i<maxPlayer; i++)
			{
				joinedPlayersID.Add(0);
				playerPrefabList.Add(null);
				selectorPrefabList.Add(null);
			}
		}

		// Update is called once per frame
		void Update()
		{
			CharacterSelection();

			//All players are ready, pop up the launch text
			if (playersReady == playerID)
			{
				gameReady = true;
				startText.enabled = true;
			}
			//Players aren't ready, hide the launch txt
			else
			{
				gameReady = false;
				startText.enabled = false;
			}

			//Leave the character selection and go back to main menu
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
			//Player 1 is joining the game
			if (Input.GetButtonUp("Fire1_P1"))
			{
				//Check if this player is not already in and if not, allow him to select his name, skin and team
				if (CheckJoinedPlayers(1))
				{
					InstantiateCarroussel(1);
				}
				//If this player is already in
				else
				{
					//And if all the other players are ready too
					if (gameReady)
					{
						//Check if we have enough team to play the game
						if (differentTeam >= minTeam || playersReady == 1)
						{
							//Add all players to the players list
							for (int i = 0; i < playerPrefabList.Count; i++)
							{
								if (playerPrefabList[i] != null)
								{
									GameManager.Instance.players.Add(playerPrefabList[i]);
								}
							}
							//Load the level for this number of player
							string levelName = "Room for " + playersReady;
							GameManager.Instance.LoadArena(levelName);
						}
						else
						{
							//Display a message on screen
							Debug.LogError("There isn't enough different team, at least 2 differents team are recquired !");
						}
					}
				}
			}
			//Player 2 is joining the game
			if (Input.GetButtonUp("Fire1_P2"))
			{
				if (CheckJoinedPlayers(2))
				{
					InstantiateCarroussel(2);
				}
			}
			//Player 3 is joining the game
			if (Input.GetButtonUp("Fire1_P3"))
			{
				if (CheckJoinedPlayers(3))
				{
					InstantiateCarroussel(3);
				}
			}
			//Player 4 is joining the game
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

		public void ConfigurePlayer(string pName, int pID, int pController, int pTeam, Sprite pSprite, Sprite pTeamSprite)
		{
			GameObject player;

			Debug.Log("on fabrique un joueur offline");
			player = Instantiate(playerPrefab) as GameObject;
			player.GetComponent<PlayerInfo>().ConfigurePlayer(pName, pID, pController, pTeam, pSprite, pTeamSprite);
			playerPrefabList[pID - 1] = player;

			playersReady++;

			bool playerTeamIsDifferent = true;

			for (int i = 0; i < playerPrefabList.Count; i++)
			{
				GameObject tempPlayer = playerPrefabList[i];
				if (tempPlayer != null && tempPlayer != player)
				{
					if (tempPlayer.GetComponent<PlayerInfo>().playerTeam == pTeam)
					{
						playerTeamIsDifferent = false;
					}
				}
			}

			if (playerTeamIsDifferent)
			{
				differentTeam++;
			}
		}

		private void InstantiateCarroussel(int pControllerNumber)
		{
			PlayerSelector playerSelector = playerSelectors[playerID - 1].GetComponent<PlayerSelector>();
			playerSelector.ActivateSelector(playerID, pControllerNumber);
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
			
			joinedPlayersID[pPlayerID - 1] = 0;
		}

		public void DestroyPlayerPrefab(int pPlayerID)
		{
			Debug.Log("On détruit le préfab du joueur : "+pPlayerID);

			Destroy(playerPrefabList[pPlayerID - 1]);
			playerPrefabList[pPlayerID - 1] = null;

			playersReady--;
			gameReady = false;
			startText.enabled = false;
		}

		public void AddSpriteToList(List<Sprite> masterList, List<Sprite> list, Sprite sprite)
		{
			int index = masterList.IndexOf(sprite);
			list.Insert(index, sprite);
			foreach (GameObject selector in playerSelectors)
			{
				PlayerSelector playerSelector = selector.GetComponent<PlayerSelector>();
				if (playerSelector.active)
				{
					playerSelector.AdjustSkinList();
				}
			}
		}

		public void RemoveSpriteFromList(List<Sprite> list, Sprite sprite, GameObject pSelector)
		{
			list.Remove(sprite);
			foreach (GameObject selector in playerSelectors)
			{
				if (selector != pSelector)
				{
					PlayerSelector playerSelector = selector.GetComponent<PlayerSelector>();
					if (playerSelector.active)
					{
						playerSelector.AdjustSkinList();
					}
				}
			}
		}

	}
}
