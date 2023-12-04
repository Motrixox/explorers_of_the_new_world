using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames;
using Assets.Scripts.Interfaces;
using Unity.VisualScripting;

public abstract class Building : MonoBehaviour, IBuildingInfo
{
    public IslandScript island { get; set; }

    protected static GameState gameState;
    public static GameObject buildingInterface;
    protected static AlertBoxScript alert;

    public bool isConnectedToHarbor { get; protected set; } = false;
    public List<Vector2> coords;
    protected List<string> LogMessage;

    protected void Awake()
    {
        LogMessage = new List<string>();
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        buildingInterface = GameObject.Find("Canvas").FindChild("Building Interface", true);
        alert = GameObject.Find("Canvas").FindChild("AlertBox", true).GetComponent<AlertBoxScript>();
        coords = new List<Vector2>();
    }

    public void CheckHarborConnection()
    {
        isConnectedToHarbor = false;

        int[,] surface = (int[,])gameState.worldSurface.Clone();

        foreach (var coord in coords)
        {

            Recursion(surface, (int)coord.x, (int)coord.y);
        }

        var msg = GetBuildingInfo().buildingName + " is not connected to harbor with road";
        ManageLog(msg, !isConnectedToHarbor);
    }

    protected void Recursion(int[,] surface, int x, int z)
    {
        if (isConnectedToHarbor)
            return;

        if (surface[x, z] == 22)
        {
            isConnectedToHarbor = true;
            return;
        }

        surface[x, z] = 0;

        if (surface[x + 1, z] == 22) // 22 = harbor
            isConnectedToHarbor = true;
        else if (surface[x - 1, z] == 22)
            isConnectedToHarbor = true;
        else if (surface[x, z + 1] == 22)
            isConnectedToHarbor = true;
        else if (surface[x, z - 1] == 22)
            isConnectedToHarbor = true;
        else
        {
            if (surface[x + 1, z] == 6) // 6 = road
                Recursion(surface, x + 1, z);
            if (surface[x - 1, z] == 6) // 6 = road
                Recursion(surface, x - 1, z);
            if (surface[x, z + 1] == 6) // 6 = road
                Recursion(surface, x, z + 1);
            if (surface[x, z - 1] == 6) // 6 = road
                Recursion(surface, x, z - 1);
        }
    }

    public virtual void Log()
    {
        foreach (var msg in LogMessage)
        {
            gameState.gameLog.Log(msg, transform.position, "Buildings");
            LogMessage = new List<string>();
        }
    }

    protected void ManageLog(string msg, bool condition)
    {
        if (condition)
        {
            if (!LogMessage.Contains(msg))
            {
                LogMessage.Add(msg);
            }
        }
        else
        {
            if (LogMessage.Contains(msg))
            {
                LogMessage.Remove(msg);
            }
        }
    }

    public abstract BuildingInfo GetBuildingInfo();

    //public void SetActive(bool a)
    //{
    //    active = a;
    //    buildingInterface.SetActive(a);

    //    if (active)
    //    {
    //        buildingGameObject.FindChild("Active", true).GetComponent<MeshRenderer>().enabled = true;
    //    }
    //    else
    //    {
    //        buildingGameObject.FindChild("Active", true).GetComponent<MeshRenderer>().enabled = false;
    //    }
    //}
}
