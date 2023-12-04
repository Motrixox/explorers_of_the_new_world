using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baths : ServiceBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingName = "Baths",
        costWood = 800,
        costStone = 800,
        costIron = 600,
        costGold = 3000,
        buildingID = 33,
        profession = "Bath worker",
        buildingImagePath = "building_icons/baths",
        prefabPath = "Prefabs/Buildings/baths"
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
