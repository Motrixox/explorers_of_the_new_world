using DevionGames;
using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    private BottomPanelController bottom;

    private GameObject main;
    private GameObject saves;
    private GameObject controls;
    private GameObject options;

    private Dictionary<string, GameData> profiles;

    // Start is called before the first frame update
    void Awake()
    {
        if (SceneManager.GetActiveScene().name.Equals("MainMenu"))
        {
			main = gameObject.FindChild("MainMenu", true);
			profiles = DataPersistenceManager.instance.GetAllProfilesGameData();
			UpdateSavesMenu();
		}
        else
        {
			main = gameObject.FindChild("GameMenu", true);
		}

		saves = gameObject.FindChild("SaveSlots", true);
		controls = gameObject.FindChild("Controls", true);
		options = gameObject.FindChild("Options", true);

        gameObject.FindChild("GameMenu", true).SetActive(false);
        gameObject.FindChild("MainMenu", true).SetActive(false);


		if (GameObject.Find("Canvas").FindChild("Bottom Panel", true) != null)
        {
            bottom = GameObject.Find("Canvas").FindChild("Bottom Panel", true).GetComponent<BottomPanelController>();
        }

        ActivateMenu("Main");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateSavesMenu()
    {
        DeactivateDelete();

		foreach (var save in profiles)
        {
            if(save.Key.Equals("0"))
            {
                gameObject.FindChild("0", true).GetComponent<Text>().text = "Week: " + ((int)(save.Value.timeSinceStart / 60)).ToString();
                gameObject.FindChild("Delete0", true).SetActive(true);
			}
            if(save.Key.Equals("1"))
            {
                gameObject.FindChild("1", true).GetComponent<Text>().text = "Week: " + ((int)(save.Value.timeSinceStart / 60)).ToString();
				gameObject.FindChild("Delete1", true).SetActive(true);
			}
            if(save.Key.Equals("2"))
            {
                gameObject.FindChild("2", true).GetComponent<Text>().text = "Week: " + ((int)(save.Value.timeSinceStart / 60)).ToString();
				gameObject.FindChild("Delete2", true).SetActive(true);
			}
            if(save.Key.Equals("3"))
            {
                gameObject.FindChild("3", true).GetComponent<Text>().text = "Week: " + ((int)(save.Value.timeSinceStart / 60)).ToString();
				gameObject.FindChild("Delete3", true).SetActive(true);
			}
            if(save.Key.Equals("4"))
            {
                gameObject.FindChild("4", true).GetComponent<Text>().text = "Week: " + ((int)(save.Value.timeSinceStart / 60)).ToString();
				gameObject.FindChild("Delete4", true).SetActive(true);
			}
        }
    }

    private void DeactivateDelete()
    {
		gameObject.FindChild("Delete0", true).SetActive(false);
		gameObject.FindChild("Delete1", true).SetActive(false);
		gameObject.FindChild("Delete2", true).SetActive(false);
		gameObject.FindChild("Delete3", true).SetActive(false);
		gameObject.FindChild("Delete4", true).SetActive(false);
	}

    public void DeleteProfile(string profileId)
    {
		DataPersistenceManager.instance.DeleteProfile(profileId);
		gameObject.FindChild(profileId, true).GetComponent<Text>().text = "New game";
		gameObject.FindChild("Delete" + profileId, true).SetActive(false);
	}

    public void ActivateMenu(string name)
    {
        DeactivateMenu();

        switch (name)
        {
            case "Main":
				main.SetActive(true);
				break;
            case "Saves":
				saves.SetActive(true);
				break;
            case "Controls":
				controls.SetActive(true);
				break;
            case "Options":
				options.SetActive(true);
				break;
        }
    }

    public void DeactivateMenu()
    {
        main.SetActive(false);
        saves.SetActive(false);
        controls.SetActive(false);
        options.SetActive(false);
    }

    public void NewGame()
    {
        DataPersistenceManager.instance.NewGame();
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void Continue()
    {
        bottom.ToggleGameMenu();
    }

    public void Restart()
    {
        var scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
