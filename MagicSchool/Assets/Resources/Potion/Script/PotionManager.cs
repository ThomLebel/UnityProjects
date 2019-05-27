using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionManager : MonoBehaviour {

	 

	public List<Transform> SpawnPositionsList;

	static public PotionManager Instance;

	// Use this for initialization
	void Start () {
		GameObject[] _players =  GameObject.FindGameObjectsWithTag("Player");
		int i = 0;
		foreach (GameObject player in _players)
		{
			Debug.Log("Init a player : "+player);

			player.GetComponent<PlayerInfo>().Init(SpawnPositionsList[i].position);

			i++;
		}
	}

	public void GenerateRecipe()
	{

	}
}
