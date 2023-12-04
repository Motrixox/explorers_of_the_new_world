using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStation : ServiceBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingName = "Fire Station",
        costWood = 1200,
        costStone = 600,
        costIron = 800,
        costGold = 2000,
        buildingID = 41,
        profession = "Firefighter",
        buildingImagePath = "building_icons/police",
        prefabPath = "Prefabs/Buildings/fireStation"
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
