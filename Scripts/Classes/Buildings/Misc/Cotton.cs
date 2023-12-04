using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames;
using Assets.Scripts.Interfaces;

public class Cotton : Building
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 9,
        buildingName = "Cotton",
        costWood = 0,
        costStone = 0,
        costIron = 0,
        costGold = 50,
        profession = "",
        buildingImagePath = "building_icons/cotton",
        prefabPath = "Prefabs/Buildings/cotton"
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
