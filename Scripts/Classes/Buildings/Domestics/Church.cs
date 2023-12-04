using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Church : ServiceBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingName = "Church",
        costWood = 1200,
        costStone = 900,
        costIron = 700,
        costGold = 3000,
        buildingID = 32,
        profession = "Priest",
        buildingImagePath = "building_icons/church",
        prefabPath = "Prefabs/Buildings/church"
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
