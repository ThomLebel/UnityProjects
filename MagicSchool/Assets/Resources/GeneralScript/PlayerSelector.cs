using Com.OniriqueStudio.MagicSchool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelector : MonoBehaviour
{
	public List<Text> playerLetters;
	public GameObject playerNameObj;
	public GameObject skin;
	public GameObject team;
	public GameObject arrows;
	public Text textAction;

	public string activateCharacterPhrase = "Press Action to Join";
	public string confirmCharacterPhrase = "Press Action to Validate";

	[Tooltip("Numero du contrôleur du joueur")]
	public int playerController;
	[Tooltip("Numero du joueur")]
	public int playerID;
	[Tooltip("speed of the sliders")]
	public float slideRate = 0.5f;

	private string playerName = "";
	private int playerTeam;
	private float nextSlide;
	private float nextSlider;
	private int idSlider = 0;
	private int currentSprite = 0;
	private GameObject currentSlider;
	private List<Sprite> currentSpriteList;

	private bool validate = false;
	private bool active = false;

	// Start is called before the first frame update
	void Start()
    {
	}

    // Update is called once per frame
    void Update()
    {
		if (!active)
		{
			return;
		}
		if (Input.GetButtonUp("Fire1_P" + playerController) && !validate)
		{
			validate = true;
			//Debug.Log("On valide notre choix de skin ! " + CharacterSelector.Instance.spriteList[currentSprite].name);
			for (var i=0; i< playerLetters.Count; i++)
			{
				playerName += playerLetters[i].text;
			}
			CharacterSelector.Instance.ConfigurePlayer(playerName, playerID, playerController, CharacterSelector.Instance.teamSpriteList.IndexOf(team.GetComponent<Image>().sprite) + 1, skin.GetComponent<Image>().sprite);
		}
		if (!validate)
		{
			CheckSlider();
			CheckSlide();

			if (Input.GetButtonUp("Fire2_P" + playerController))
			{
				CharacterSelector.Instance.DestroyCarroussel(playerID);
				DeactivateSelector();
			}
		}
		else
		{
			if (Input.GetButtonUp("Fire2_P" + playerController))
			{
				validate = false;

				CharacterSelector.Instance.DestroyPlayerPrefab(playerID);
			}
		}
	}

	private void CheckSlider()
	{
		if (Time.time > nextSlider)
		{
			if (Input.GetAxisRaw("Vertical_P" + playerController) > 0.2)
			{
				Debug.Log("Change slider for player " + playerID);
				ChangeSlider(1);
			}
			if (Input.GetAxisRaw("Vertical_P" + playerController) < -0.2)
			{
				Debug.Log("Change slider for player " + playerID);
				ChangeSlider(-1);
			}
		}
	}

	private void ChangeSlider(int dir)
	{
		idSlider += dir;
		if (idSlider < 0)
		{
			idSlider = 1;
		}
		if (idSlider > 1)
		{
			idSlider = 0;
		}

		currentSpriteList = new List<Sprite>();

		if (idSlider == 0)
		{
			currentSlider = skin;
			currentSpriteList = CharacterSelector.Instance.skinList;
		}
		else
		{
			currentSlider = team;
			currentSpriteList = CharacterSelector.Instance.teamSpriteList;
		}

		currentSprite = currentSpriteList.IndexOf(currentSlider.GetComponent<Image>().sprite);
		arrows.GetComponent<RectTransform>().position = new Vector2(arrows.GetComponent<RectTransform>().position.x, currentSlider.GetComponent<RectTransform>().position.y);

		nextSlider = Time.time + slideRate;
	}

	private void CheckSlide()
	{
		if (Time.time > nextSlide)
		{
			if (Input.GetAxisRaw("Horizontal_P" + playerController) > 0.2)
			{
				Debug.Log("Slide right for player " + playerID);
				Slide(1);
			}
			if (Input.GetAxisRaw("Horizontal_P" + playerController) < -0.2)
			{
				Debug.Log("Slide left for player " + playerID);
				Slide(-1);
			}
		}
	}

	private void Slide(int dir)
	{
		currentSprite += dir;
		if (currentSprite >= currentSpriteList.Count)
		{
			currentSprite = 0;
		}
		if (currentSprite < 0)
		{
			currentSprite = currentSpriteList.Count;
		}

		currentSlider.GetComponent<Image>().sprite = currentSpriteList[currentSprite];
		currentSlider.GetComponent<Image>().SetNativeSize();

		nextSlide = Time.time + slideRate;
	}

	public void ActivateSelector(int id, int controller)
	{
		playerID = id;
		playerController = controller;

		textAction.text = confirmCharacterPhrase;
		playerNameObj.SetActive(true);
		skin.SetActive(true);
		team.SetActive(true);
		arrows.SetActive(true);

		idSlider = 0;
		currentSlider = skin;
		currentSpriteList = CharacterSelector.Instance.skinList;

		active = true;

		Debug.Log("Character Carroussel : ID : " + playerID + "; Controller : " + playerController);
	}

	public void DeactivateSelector()
	{
		textAction.text = activateCharacterPhrase;
		playerNameObj.SetActive(false);
		skin.SetActive(false);
		team.SetActive(false);
		arrows.SetActive(false);

		currentSlider = null;
		currentSpriteList = null;

		active = false;
	}
}
