using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames;
using Assets.Scripts.Interfaces;

public class Spice : Building
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 8,
        buildingName = "Spice",
        costWood = 0,
        costStone = 0,
        costIron = 0,
        costGold = 50,
        profession = "",
        buildingImagePath = "building_icons/spice",
        prefabPath = "Prefabs/Buildings/spice"
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
