using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brothel : ServiceBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingName = "Brothel",
        costWood = 1200,
        costStone = 600,
        costIron = 600,
        costGold = 3000,
        buildingID = 35,
        profession = "Prostitute",
        buildingImagePath = "building_icons/brothel",
        prefabPath = "Prefabs/Buildings/brothel"
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
