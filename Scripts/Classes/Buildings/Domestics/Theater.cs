using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Theater : ServiceBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingName = "Theater",
        costWood = 1500,
        costStone = 700,
        costIron = 700,
        costGold = 3000,
        buildingID = 31,
        profession = "Actor",
        buildingImagePath = "building_icons/theater",
        prefabPath = "Prefabs/Buildings/theater"
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
