using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Com.OniriqueStudio.MagicSchool
{
	public class Launcher : MonoBehaviour
	{
		#region Public Variables

		[Tooltip("The UI Panel to let the user enter name, connect and play")]
		public GameObject controlPanel;

		#endregion


		#region Private Variables

		/// <summary>
		/// This client's version number. Users are separated from each other by gameversion (which allows to make breaking changes).
		/// </summary>
		string _gameVersion = "1";

		/// <summary>
		/// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
		/// we need to keep track of this to properly adjust the behaviour when we receive call back by Photon.
		/// Typically this is used for the OnConnectedMaster() callback.
		/// </summary>
		bool isConnecting;

		#endregion


		#region MonoBehaviour CallBacks

		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during early initialization phase.
		/// </summary>
		private void Awake()
		{

		}

		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during initialization phase.
		/// </summary>
		void Start()
		{
			//Connect();
			controlPanel.SetActive(true);
		}

		#endregion


		#region Public Methods

		public void CharacterSelection()
		{
			Debug.Log("Loading player selection scene");
			SceneManager.LoadScene("PlayerSelection");
		}
		#endregion
	}
}