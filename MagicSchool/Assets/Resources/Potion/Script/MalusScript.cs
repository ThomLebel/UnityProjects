using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MalusScript : MonoBehaviour
{
	public delegate void MyDelegate(GameObject target);
	public List<MyDelegate> malusList = new List<MyDelegate>();

	private void Start()
	{
		malusList.Add(EmptyPlayerCauldron);
		malusList.Add(InvertPlayerDirection);
		malusList.Add(SlowDownPlayer);
		malusList.Add(StunPlayer);
		malusList.Add(BurnPlayerCauldron);
		malusList.Add(BlockPlayerSpell);
		malusList.Add(BlockPlayerJump);
		malusList.Add(PreventPickUpIngredient);
		malusList.Add(PreventPlayerCraft);
	}

	private void EmptyPlayerCauldron(GameObject target)
	{
		Debug.Log("Empty the cauldron of this player");
		int playerID = target.GetComponent<PlayerInfo>().playerID;

		//Find the player's cauldron
		GameObject[] cauldrons = GameObject.FindGameObjectsWithTag("chaudron");
		GameObject cauldron = null;

		foreach (GameObject caul in cauldrons)
		{
			int cauldronID = caul.GetComponent<AffiliableScript>().teamID;

			if (cauldronID == playerID)
			{
				cauldron = caul;
				break;
			}
		}

		cauldron.GetComponent<ChaudronScript>().Empty();
	}

	private void BurnPlayerCauldron(GameObject target)
	{
		Debug.Log("Burn the cauldron of this player");
		int playerID = target.GetComponent<PlayerInfo>().playerID;

		//Find the player's cauldron
		GameObject[] cauldrons = GameObject.FindGameObjectsWithTag("chaudron");
		GameObject cauldron = null;

		foreach (GameObject caul in cauldrons)
		{
			int cauldronID = caul.GetComponent<AffiliableScript>().teamID;

			if (cauldronID == playerID)
			{
				cauldron = caul;
				break;
			}
		}

		cauldron.GetComponent<ChaudronScript>().IncreaseFire();
	}

	private void InvertPlayerDirection(GameObject target)
	{
		Debug.Log("Invert directions of this player");
		PlayerPotion targetScript = target.GetComponent<PlayerPotion>();

		targetScript.InvertDirection();
	}

	private void SlowDownPlayer(GameObject target)
	{
		Debug.Log("Slow movement of this player");
		PlayerPotion targetScript = target.GetComponent<PlayerPotion>();

		targetScript.SlowSpeed();
	}

	private void StunPlayer(GameObject target)
	{
		Debug.Log("Stun this player");
		PlayerPotion targetScript = target.GetComponent<PlayerPotion>();

		targetScript.SpellHit(Vector3.zero);
	}

	private void BlockPlayerSpell(GameObject target)
	{
		Debug.Log("Block spells of this player");
		PlayerPotion targetScript = target.GetComponent<PlayerPotion>();

		targetScript.SilencePlayer();
	}

	private void BlockPlayerJump (GameObject target)
	{
		Debug.Log("Block Jump of this player");
		PlayerPotion targetScript = target.GetComponent<PlayerPotion>();

		targetScript.HeavyPlayer();
	}

	private void PreventPickUpIngredient(GameObject target)
	{
		Debug.Log("Prevent this player og picking up ingredient");
		PlayerPotion targetScript = target.GetComponent<PlayerPotion>();

		targetScript.PlayerCantCarry();
	}

	private void PreventPlayerCraft(GameObject target)
	{
		Debug.Log("Prevent this player of crafting ingredient");
		PlayerPotion targetScript = target.GetComponent<PlayerPotion>();

		targetScript.PlayerCantCraft();
	}
}
