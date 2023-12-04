using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames;
using Assets.Scripts.Interfaces;

public class Vine : Building
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 7,
        buildingName = "Vine",
        costWood = 0,
        costStone = 0,
        costIron = 0,
        costGold = 50,
        profession = "",
        buildingImagePath = "building_icons/wine",
        prefabPath = "Prefabs/Buildings/vine"
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
