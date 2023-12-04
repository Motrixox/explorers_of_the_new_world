using DevionGames;
using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanelController : MonoBehaviour
{
    private GameState gameState;

    private Text wood;
    private Text stone;
    private Text iron;
    private Text gold;
    private Text time;

    private UIWidget gameMenu;
    private UIWidget controls;
    private bool IsGameMenuOpen = false;

    // Start is called before the first frame update
    void Awake()
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        wood = gameObject.FindChild("WoodAmount", true).GetComponent<Text>();
        stone = gameObject.FindChild("StoneAmount", true).GetComponent<Text>();
        iron = gameObject.FindChild("IronAmount", true).GetComponent<Text>();
        gold = gameObject.FindChild("GoldAmount", true).GetComponent<Text>();
        time = gameObject.FindChild("Time", true).GetComponent<Text>();
        gameMenu = GameObject.Find("Canvas").FindChild("Game Menu", true).GetComponent<UIWidget>();
        controls = GameObject.Find("Canvas").FindChild("Controls Menu", true).GetComponent<UIWidget>();
    }

    private void Start()
    {
        gameMenu.Toggle();
        controls.Toggle();
    }

    // Update is called once per frame
    void Update()
    {
        SetTime();

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ToggleGameMenu();
        }
    }

    public void ToggleGameMenu()
    {
        gameMenu.Toggle();
        IsGameMenuOpen = !IsGameMenuOpen;

        if (IsGameMenuOpen)
        {
            gameState.SetTimeScale(-1);
        }
        else
        {
            gameState.SetTimeScale(1);
        }
    }

    private void IslandChanged()
    {
        SetText();
    }

    private void SetTime()
    {
        string t = "week ";
        t += ((int)(Time.timeSinceLevelLoadAsDouble + gameState.timeSinceStart)) / 60;
        t += ", ";
        t += ((int)(Time.timeSinceLevelLoadAsDouble + gameState.timeSinceStart)) / 60;
        t += ":";
        if(((int)(Time.timeSinceLevelLoadAsDouble + gameState.timeSinceStart)) % 60 < 10)
            t += "0";
        t += ((int)(Time.timeSinceLevelLoadAsDouble + gameState.timeSinceStart)) % 60;

        time.text = t;
    }

    private void SetText()
    {
        var resources = gameState.currentIsland.GetComponent<IslandScript>().GetResources();

        wood.text = resources[0];
        stone.text = resources[1];
        iron.text = resources[2];

        gold.text = gameState.goldAmount + " gold";

    }

    private void OnEnable()
    {
        GameState.OnIslandChanged += IslandChanged;
        IslandScript.OnIslandUpdate += IslandChanged;
        ShipInterfaceController.OnProductsChanged += IslandChanged;
        TradeListItem.OnListChanged += IslandChanged;

        if (gameState.currentIsland != null)
            IslandChanged();
    }

    private void OnDisable()
    {
        GameState.OnIslandChanged -= IslandChanged;
        IslandScript.OnIslandUpdate -= IslandChanged;
        ShipInterfaceController.OnProductsChanged -= IslandChanged;
        TradeListItem.OnListChanged -= IslandChanged;
    }

    public void RemoveNotification()
    {
        gameObject.FindChild("GameLogButton", true).FindChild("Notification", true).SetActive(false);
    }
}
