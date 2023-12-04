using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames;
using Assets.Scripts.Interfaces;

public class House : Building, IWorkingBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 23,
        buildingName = "House",
        costWood = 500,
        costStone = 300,
        costIron = 100,
        costGold = 400,
        profession = "",
        buildingImagePath = "building_icons/house",
        prefabPath = "Prefabs/Buildings/house"
    };

    public int peopleCapacity { get; protected set; } = 10;
    public List<Person> residents { get; private set; }
    public bool isWorking { get; set; }

    protected new void Awake()
    {
        base.Awake();

        residents = new List<Person>();
    }
    public void CheckIsWorking()
    {
        CheckHarborConnection();

        if (isConnectedToHarbor)
            isWorking = true;
        else
            isWorking = false;
    }

    public bool AddResident(Person p)
    {
        if (residents.Count >= peopleCapacity)
        {
            alert.Alert("Cannot add more people!");
            return false;
        }
        residents.Add(p);
        return true;
    }
    public bool RemoveResident(Person p)
    {
        if (residents.IndexOf(p) < 0)
        {
            alert.Alert("Person not found in residents list!");
            return false;
        }

        residents.Remove(p);
        return true;
    }
    public override BuildingInfo GetBuildingInfo()
    {
        return buildingInfo;
    }
}
