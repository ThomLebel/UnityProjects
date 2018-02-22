using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionManager : MonoBehaviour {

	public List<Transform> SpawnPositionsList;

	// Use this for initialization
	void Start () {
		GameObject[] _players =  GameObject.FindGameObjectsWithTag("Player");
		int i = 0;
		foreach (GameObject player in _players)
		{
			Debug.Log("Init a player : "+player);
			Debug.Log("Init a player : ",player);
			if (PhotonNetwork.connected)
			{
				player.GetComponent<PhotonView>().RPC("Init", PhotonTargets.All, SpawnPositionsList[i].position);
			}
			else
			{
				player.GetComponent<PlayerInfo>().Init(SpawnPositionsList[i].position);
			}
			i++;
		}
	}
}
