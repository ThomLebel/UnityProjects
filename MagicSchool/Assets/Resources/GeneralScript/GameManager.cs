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

		#endregion


		#region MonoBehaviour CallBacks

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