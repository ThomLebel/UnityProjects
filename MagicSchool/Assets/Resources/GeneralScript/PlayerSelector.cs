using Com.OniriqueStudio.MagicSchool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelector : MonoBehaviour
{
	public bool active = false;

	public List<Text> playerLetters;
	public GameObject playerNameObj;
	public GameObject skin;
	public GameObject team;
	public GameObject arrows;
	public GameObject nameArrows;
	public Text textAction;

	public string activateCharacterPhrase = "Press Action to Join";
	public string confirmCharacterPhrase = "Press Action to Validate";

	[Tooltip("Numero du contrôleur du joueur")]
	public int playerController;
	[Tooltip("Numero du joueur")]
	public int playerID;
	[Tooltip("speed of the sliders")]
	public float slideRate = 0.2f;

	private string playerName = "";
	private int playerTeam;
	private float nextSlide;
	private float nextSlider;
	private int currentID = 0;
	private int maxID = 0;
	private int idSlider = 0;
	private int currentSprite = 0;
	private GameObject currentSlider;
	private List<Sprite> currentSpriteList;
	private List<char> charList = new List<char>();
	private int charID;
	private Text currentLetter;
	private int currentLetterSlider = 0;

	private bool skinLocked = false;
	private bool validate = false;
	private bool nameSelection = false;

	// Start is called before the first frame update
	void Start()
    {
		charList.Add((char)32);
		for (int i=0; i<2; i++)
		{
			for (char letter = 'A'; letter <= 'Z'; letter++)
			{
				if (i < 1)
					charList.Add(letter);
				else
					charList.Add(char.ToLower(letter));
			}
		}
		for (int nbr = 48; nbr < 58; nbr++)
		{
			charList.Add((char)nbr);
		}
	}

    // Update is called once per frame
    void Update()
    {
		if (!active)
		{
			return;
		}
		if (Input.GetButtonUp("Fire1_P" + playerController))
		{
			if (!skinLocked)
			{
				skinLocked = true;
				CharacterSelector.Instance.RemoveSpriteFromList(CharacterSelector.Instance.tempSkinList, skin.GetComponent<Image>().sprite, gameObject);
				ActivateNameSlider();
			}
			else if(skinLocked && !validate)
			{
				validate = true;
				arrows.SetActive(false);
				nameArrows.SetActive(false);
				Debug.Log("On valide notre personnage ! ");

				playerName = "";
				for (var i = 0; i < playerLetters.Count; i++)
				{
					playerName += playerLetters[i].text;
				}
				CharacterSelector.Instance.ConfigurePlayer(playerName, playerID, playerController, CharacterSelector.Instance.tempTeamSpriteList.IndexOf(team.GetComponent<Image>().sprite) + 1, skin.GetComponent<Image>().sprite, team.GetComponent<Image>().sprite);
			}
		}
		if (!validate)
		{
			CheckSlide();
			CheckSlider();

			if (Input.GetButtonUp("Fire2_P" + playerController))
			{
				if (!skinLocked)
				{
					CharacterSelector.Instance.DestroyCarroussel(playerID);
					DeactivateSelector();
				}
				else
				{
					skinLocked = false;
					nameSelection = false;

					arrows.SetActive(true);
					nameArrows.SetActive(false);

					CharacterSelector.Instance.AddSpriteToList(CharacterSelector.Instance.skinList, CharacterSelector.Instance.tempSkinList, skin.GetComponent<Image>().sprite);

					currentSlider = skin;
					currentSpriteList = CharacterSelector.Instance.tempSkinList;
					currentSprite = currentSpriteList.IndexOf(currentSlider.GetComponent<Image>().sprite);
					currentID = currentSprite;
					maxID = currentSpriteList.Count;

					arrows.GetComponent<RectTransform>().position = new Vector2(arrows.GetComponent<RectTransform>().position.x, currentSlider.GetComponent<RectTransform>().position.y);
				}
				
			}
		}
		else
		{
			if (Input.GetButtonUp("Fire2_P" + playerController))
			{
				validate = false;

				arrows.SetActive(true);
				nameArrows.SetActive(true);

				CharacterSelector.Instance.DestroyPlayerPrefab(playerID);
				ActivateNameSlider();
			}
		}
	}

	private void ActivateNameSlider()
	{
		nameSelection = true;
		nameArrows.SetActive(true);
		nameArrows.GetComponent<RectTransform>().position = new Vector2(playerLetters[0].GetComponent<RectTransform>().position.x, nameArrows.GetComponent<RectTransform>().position.y);
		currentLetter = playerLetters[0];
		charID = charList.IndexOf(char.Parse(currentLetter.GetComponent<Text>().text));
		currentID = charID;
		currentSlider = playerNameObj;
		maxID = charList.Count;
		arrows.GetComponent<RectTransform>().position = new Vector2(arrows.GetComponent<RectTransform>().position.x, currentSlider.GetComponent<RectTransform>().position.y);
	}

	private void CheckSlider()
	{
		if (Time.time > nextSlider)
		{
			if (nameSelection)
			{
				if (Input.GetAxisRaw("Horizontal_P" + playerController) > 0.2)
				{
					ChangeLetterSlider(1);
				}
				if (Input.GetAxisRaw("Horizontal_P" + playerController) < -0.2)
				{
					ChangeLetterSlider(-1);
				}
			}
			else
			{
				if (Input.GetAxisRaw("Vertical_P" + playerController) > 0.2)
				{
					ChangeSlider(-1);
				}
				if (Input.GetAxisRaw("Vertical_P" + playerController) < -0.2)
				{
					ChangeSlider(1);
				}
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
			//Skin Slider
			currentSlider = skin;
			currentSpriteList = CharacterSelector.Instance.tempSkinList;
		}
		else
		{
			//Team Slider
			currentSlider = team;
			currentSpriteList = CharacterSelector.Instance.tempTeamSpriteList;
		}

		currentSprite = currentSpriteList.IndexOf(currentSlider.GetComponent<Image>().sprite);
		currentID = currentSprite;
		maxID = currentSpriteList.Count;

		arrows.GetComponent<RectTransform>().position = new Vector2(arrows.GetComponent<RectTransform>().position.x, currentSlider.GetComponent<RectTransform>().position.y);

		nextSlider = Time.time + slideRate;
	}

	private void ChangeLetterSlider(int dir)
	{
		currentLetterSlider += dir;
		if (currentLetterSlider < 0)
		{
			currentLetterSlider = playerLetters.Count - 1;
		}
		if (currentLetterSlider >= playerLetters.Count)
		{
			currentLetterSlider = 0;
		}

		currentLetter = playerLetters[currentLetterSlider];
		charID = charList.IndexOf(char.Parse(currentLetter.GetComponent<Text>().text));
		currentID = charID;

		nameArrows.GetComponent<RectTransform>().position = new Vector2(currentLetter.GetComponent<RectTransform>().position.x, nameArrows.GetComponent<RectTransform>().position.y);

		nextSlider = Time.time + slideRate;
	}

	private void CheckSlide()
	{
		if (Time.time > nextSlide)
		{
			if (nameSelection)
			{
				if (Input.GetAxisRaw("Vertical_P" + playerController) > 0.2)
				{
					currentLetter.text = charList[Slide(1)].ToString();
				}
				if (Input.GetAxisRaw("Vertical_P" + playerController) < -0.2)
				{
					currentLetter.text = charList[Slide(-1)].ToString();
				}
			}
			else
			{
				if (Input.GetAxisRaw("Horizontal_P" + playerController) > 0.2)
				{
					Debug.Log("Slide right for player " + playerID);
					currentSlider.GetComponent<Image>().sprite = currentSpriteList[Slide(1)];
					currentSlider.GetComponent<Image>().SetNativeSize();
				}
				if (Input.GetAxisRaw("Horizontal_P" + playerController) < -0.2)
				{
					Debug.Log("Slide left for player " + playerID);
					currentSlider.GetComponent<Image>().sprite = currentSpriteList[Slide(-1)];
					currentSlider.GetComponent<Image>().SetNativeSize();
				}
			}
			
		}
	}

	private int Slide(int dir)
	{
		currentID += dir;
		if (currentID >= maxID)
		{
			currentID = 0;
		}
		if (currentID < 0)
		{
			currentID = maxID - 1;
		}

		nextSlide = Time.time + slideRate;

		return currentID;
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
		currentSpriteList = CharacterSelector.Instance.tempSkinList;
		currentSprite = 0;
		currentID = currentSprite;
		maxID = currentSpriteList.Count;

		skin.GetComponent<Image>().sprite = currentSpriteList[0];

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

	public void AdjustSkinList()
	{
		if (currentSlider == skin)
		{
			skin.GetComponent<Image>().sprite = currentSpriteList[currentSprite];
			maxID = currentSpriteList.Count;
		}
	}
}
