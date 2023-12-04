using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Market : ServiceBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingName = "Market",
        costWood = 1000,
        costStone = 500,
        costIron = 300,
        costGold = 2000,
        buildingID = 36,
        profession = "Salesman",
        buildingImagePath = "building_icons/market",
        prefabPath = "Prefabs/Buildings/market"
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
