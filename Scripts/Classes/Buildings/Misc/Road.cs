using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames;
using Assets.Scripts.Interfaces;

public class Road : Building
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 6,
        buildingName = "Road",
        costWood = 0,
        costStone = 50,
        costIron = 0,
        costGold = 50,
        profession = "",
        buildingImagePath = "building_icons/road",
        prefabPath = "Prefabs/Buildings/road"
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
