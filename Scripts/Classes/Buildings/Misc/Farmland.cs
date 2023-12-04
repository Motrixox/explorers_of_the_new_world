using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames;
using Assets.Scripts.Interfaces;

public class Farmland : Building
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 10,
        buildingName = "Farmland",
        costWood = 0,
        costStone = 0,
        costIron = 0,
        costGold = 30,
        profession = "",
        buildingImagePath = "building_icons/farmland",
        prefabPath = "Prefabs/Buildings/farmland"
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
