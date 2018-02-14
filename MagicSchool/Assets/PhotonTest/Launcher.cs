﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Com.OniriqueStudio.MagicSchool
{
	public class Launcher : Photon.PunBehaviour
	{
		#region Public Variables

		/// <summary>
		/// The PUN loglevel.
		/// </summary>
		public PhotonLogLevel LogLevel = PhotonLogLevel.Informational;

		/// <summary>
		/// The maximum number of players per room. When a room is full, it can't be joined by new players, and so a new room will be created.
		/// </summary>
		[Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
		public byte MaxPlayersPerRoom = 4;

		[Tooltip("The UI Panel to let the user enter name, connect and play")]
		public GameObject controlPanel;
		[Tooltip("The UI Label to inform the user that the connection is in progress")]
		public GameObject progressLabel;
		[Tooltip("The UI Label to create or join a room")]
		public GameObject connectPanel;
		[Tooltip("The UI Label to create a room")]
		public GameObject createPanel;
		[Tooltip("The UI Label to join a room")]
		public GameObject joinPanel;

		public InputField createInputField;
		public InputField joinInputField;

		public string roomName = "";

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
			// #NotImportant
			// Force LogLevel
			PhotonNetwork.logLevel = LogLevel;

			//#Critical
			//We don't join the lobby. There is no need to join a lobby to get the list of rooms.
			PhotonNetwork.autoJoinLobby = false;

			//#Critical
			//This makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
			PhotonNetwork.automaticallySyncScene = true;

		}

		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during initialization phase.
		/// </summary>
		void Start()
		{
			//Connect();
			progressLabel.SetActive(false);
			connectPanel.SetActive(false);
			createPanel.SetActive(false);
			joinPanel.SetActive(false);
			controlPanel.SetActive(true);
		}

		#endregion


		#region Public Methods

		public void CharacterSelection()
		{
			Debug.Log("Loading player selection scene");
			SceneManager.LoadScene("PlayerSelection");
		}

		public void DisplayConnectUI()
		{
			progressLabel.SetActive(false);
			connectPanel.SetActive(true);
			controlPanel.SetActive(false);
		}

		public void DisplayCreateRoomUI()
		{
			connectPanel.SetActive(false);
			createPanel.SetActive(true);
		}

		public void DisplayJoinRoomUI()
		{
			connectPanel.SetActive(false);
			joinPanel.SetActive(true);
		}

		public void BackPanel()
		{
			if (joinPanel.activeSelf)
			{
				connectPanel.SetActive(true);
				joinPanel.SetActive(false);
			}
			else if (createPanel.activeSelf)
			{
				connectPanel.SetActive(true);
				createPanel.SetActive(false);
			}
			else if (connectPanel.activeSelf)
			{
				connectPanel.SetActive(false);
				controlPanel.SetActive(true);
			}
		}

		public void SetRoomName(string value)
		{
			roomName = value;
		}

		public void CreateRoom()
		{
			isConnecting = true;

			progressLabel.SetActive(true);
			connectPanel.SetActive(false);
			createPanel.SetActive(false);
			joinPanel.SetActive(false);
			controlPanel.SetActive(false);

			//we check if we are connected or not, we join if we are, else we initiate the connection to the server.
			if (PhotonNetwork.connected)
			{
				//#Critical we need at this point to attempt joining a random room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
				Debug.Log("Creating and joining room : " + roomName);
				PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
			}
			else
			{
				//#Critical, we must first and foremost connect to Photon Online Server.
				PhotonNetwork.ConnectUsingSettings(_gameVersion);
			}
		}

		public void JoinRoom()
		{
			isConnecting = true;

			progressLabel.SetActive(true);
			connectPanel.SetActive(false);
			createPanel.SetActive(false);
			joinPanel.SetActive(false);
			controlPanel.SetActive(false);

			//we check if we are connected or not, we join if we are, else we initiate the connection to the server.
			if (PhotonNetwork.connected)
			{
				//#Critical we need at this point to attempt joining a random room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
				Debug.Log("Trying to join room : "+roomName);
				PhotonNetwork.JoinRoom(roomName);
			}
			else
			{
				//#Critical, we must first and foremost connect to Photon Online Server.
				PhotonNetwork.ConnectUsingSettings(_gameVersion);
			}
		}




		/// <summary>
		/// Start the connection process.
		/// - If already connected, we attempt joining a random room
		/// - If not yet connected, Connect this application instance to Photon Cloud Network
		/// </summary>
		public void Connect()
		{
			//keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
			isConnecting = true;

			progressLabel.SetActive(true);
			connectPanel.SetActive(false);
			createPanel.SetActive(false);
			joinPanel.SetActive(false);
			controlPanel.SetActive(false);
			//we check if we are connected or not, we join if we are, else we initiate the connection to the server.
			if (PhotonNetwork.connected)
			{
				//#Critical we need at this point to attempt joining a random room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
				PhotonNetwork.JoinRandomRoom();
			}
			else
			{
				//#Critical, we must first and foremost connect to Photon Online Server.
				PhotonNetwork.ConnectUsingSettings(_gameVersion);
			}
		}
		#endregion


		#region Photon.PunBehaviour CallBacks

		public override void OnConnectedToMaster()
		{
			Debug.Log("PhotonTest/Launcher: OnConnectedToMaster() was called by PUN");
			//We don't want to do anything if we are not attempting to join a room.
			//this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called,
			//in that case we don't whnt to do anything
			if (isConnecting) {
				//#Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed();
				PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
				//PhotonNetwork.JoinRandomRoom();
			}
		}

		public override void OnDisconnectedFromPhoton()
		{
			progressLabel.SetActive(false);
			connectPanel.SetActive(false);
			createPanel.SetActive(false);
			joinPanel.SetActive(false);
			controlPanel.SetActive(true);

			Debug.LogWarning("PhotonTest/Launcher: OnDisconnectedFromPhoton() was called by PUN");
		}

		public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
		{
			Debug.Log("PhotonTest/Launcher: OnPhotonJoinRoomFailed() was called by PUN.");
			//#Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
			PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
		}

		public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
		{
			Debug.Log("PhotonTest/Launcher: OnPhotonJoinRoomFailed() was called by PUN.");
			//#Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
			PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
		}

		public override void OnJoinedRoom()
		{
			Debug.Log("PhotonTest/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

			//#Critical: We only load if we are the first player, else we rely on PhotonNetwork.automaticallySyncScene to sync our instance scene.
			if (PhotonNetwork.isMasterClient)
			{
				Debug.Log("We load the 'lobby'");

				//#Critical
				//Load the Room Level
				PhotonNetwork.LoadLevel("PlayerSelection");
				//PhotonNetwork.LoadLevel("Room for 1");
			}
		}

		#endregion
	}
}