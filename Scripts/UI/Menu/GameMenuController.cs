using DevionGames;
using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    private UIWidget controls;
    private UIWidget saves;
    private BottomPanelController bottom;

    // Start is called before the first frame update
    void Awake()
    {
        controls = GameObject.Find("Canvas").FindChild("Controls Menu", true).GetComponent<UIWidget>();
        controls.Close();
        if(GameObject.Find("Canvas").FindChild("SaveSlotsMenu", true) != null)
        {
            saves = GameObject.Find("Canvas").FindChild("SaveSlotsMenu", true).GetComponent<UIWidget>();
            saves.Close();
        }
        if(GameObject.Find("Canvas").FindChild("Bottom Panel", true) != null)
        {
            bottom = GameObject.Find("Canvas").FindChild("Bottom Panel", true).GetComponent<BottomPanelController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        saves.Toggle();
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

    public void Controls()
    {
        controls.Toggle();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
