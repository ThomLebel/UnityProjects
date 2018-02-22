using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.OniriqueStudio.MagicSchool
{
	public class GameManager : Photon.PunBehaviour
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

			if (playerPrefab == null)
			{
				Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager' ", this);
			}
			else
			{
				if (PlayerNetwork.LocalPlayerInstance == null)
				{
					Debug.Log("We are Instantiating LocalPlayer from " + SceneManager.GetActiveScene().name);
					//PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
				}
				else
				{
					Debug.Log("Ignoring scene load for " + SceneManager.GetActiveScene().name);
				}
			}
		}

		#endregion


		#region Photon Messages

		public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
		{
			Debug.Log("OnPhotonPlayerConnected() "+ newPlayer.NickName); // not seen if you're the player connecting

			if (PhotonNetwork.isMasterClient)
			{
				Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected

				LoadArena("PlayerSelection");
			}
		}

		public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
		{
			Debug.Log("OnPhotonPlayerDisconnected() "+ otherPlayer.NickName); // seen when other disconnects

			if (PhotonNetwork.isMasterClient)
			{
				Debug.Log("OnPhotonPlayerDisconnected isMasterClient" + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected

				LoadArena("Launcher");
			}
		}

		/// <summary>
		/// Called when the local player left the room. We need to load the launcher scene.
		/// </summary>
		public override void OnLeftRoom()
		{
			SceneManager.LoadScene(0);
		}

		#endregion


		#region Public Methods

		public void LeaveRoom()
		{
			if (PhotonNetwork.connected)
			{
				PhotonNetwork.LeaveRoom();
			}
			else
			{
				LoadArena("Launcher");
			}
		}

		[PunRPC]
		public void LoadArena(string pLevelName)
		{
			
			//PhotonNetwork.LoadLevel("Room for "+ PhotonNetwork.room.PlayerCount);
			if (PhotonNetwork.connected)
			{
				if (!PhotonNetwork.isMasterClient)
				{
					Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
				}
				Debug.Log("PhotonNetwork: Loading Level : " + PhotonNetwork.room.PlayerCount);

				PhotonNetwork.LoadLevel(pLevelName);
			}
			else
				SceneManager.LoadScene(pLevelName);
		}

		#endregion


		#region Private Methods



		#endregion
	}
}