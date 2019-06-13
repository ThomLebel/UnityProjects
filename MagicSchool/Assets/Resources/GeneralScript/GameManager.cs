using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.OniriqueStudio.MagicSchool
{
	public class GameManager : MonoBehaviour
	{
		#region Public Variables

		static public GameManager Instance;

		[Tooltip("The prefab to use for representing the player")]
		public GameObject playerPrefab;

		[Tooltip("The list of all players")]
		public List<GameObject> players = new List<GameObject>();

		[Tooltip("The number of team")]
		public int TeamNumber = 0;

		#endregion


		#region MonoBehaviour CallBacks
		private void Awake()
		{
			DontDestroyOnLoad(this.gameObject);
		}

		private void Start()
		{
			Instance = this;
		}

		#endregion


		#region Public Methods

		public void LeaveRoom()
		{
			LoadArena("Launcher");
		}

		public void LoadArena(string pLevelName)
		{
			SceneManager.LoadScene(pLevelName);
		}

		#endregion


		#region Private Methods



		#endregion
	}
}