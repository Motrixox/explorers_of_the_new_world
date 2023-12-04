using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceStation : ServiceBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingName = "Police Station",
        costWood = 1000,
        costStone = 800,
        costIron = 500,
        costGold = 2000,
        buildingID = 39,
        profession = "Guard",
        buildingImagePath = "building_icons/police",
        prefabPath = "Prefabs/Buildings/policeStation"
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
