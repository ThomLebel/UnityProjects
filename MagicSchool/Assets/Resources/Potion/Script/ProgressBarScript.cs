using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarScript : MonoBehaviour
{

	public float value = 0f; //current progress

	private SpriteRenderer progressBar;           // Reference to the sprite renderer of the health bar.
	private Vector3 progressScale;                // The local scale of the health bar initially (with full health).

	private void Awake()
	{
		progressBar = gameObject.GetComponentInChildren<SpriteRenderer>();
		progressScale = progressBar.transform.localScale;
	}

	private void Update()
	{
		// Set the scale of the health bar to be proportional to the player's health.
		progressBar.transform.localScale = new Vector3(progressScale.x + value, 1, 1);
		transform.localScale = transform.parent.localScale;

		if (value == 0f)
		{
			ToggleVisibility(false);
		}
		else
		{
			ToggleVisibility(true);
		}
	}

	private void ToggleVisibility(bool pRendered)
	{
		// toggles the visibility of this gameobject and all it's children
		SpriteRenderer[] renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer r in renderers)
		{
			r.enabled = pRendered;
		}
	}
}