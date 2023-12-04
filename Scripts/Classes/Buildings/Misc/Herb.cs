using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames;
using Assets.Scripts.Interfaces;

public class Herb : Building
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 43,
        buildingName = "Herb",
        costWood = 0,
        costStone = 0,
        costIron = 0,
        costGold = 50,
        profession = "",
        buildingImagePath = "building_icons/herb",
        prefabPath = "Prefabs/Buildings/herb"
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
