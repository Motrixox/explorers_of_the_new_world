using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pub : ServiceBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingName = "Pub",
        costWood = 1000,
        costStone = 500,
        costIron = 300,
        costGold = 2000,
        buildingID = 34,
        profession = "Waiter",
        buildingImagePath = "building_icons/pub",
        prefabPath = "Prefabs/Buildings/pub"
    };

    new void Awake()
    {
        base.Awake();
    }
    public override BuildingInfo GetBuildingInfo()
    {
        return buildingInfo;
    }
}
