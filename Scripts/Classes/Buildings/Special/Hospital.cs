using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hospital : ServiceBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingName = "Hospital",
        costWood = 1500,
        costStone = 800,
        costIron = 700,
        costGold = 3000,
        buildingID = 40,
        profession = "Doctor",
        buildingImagePath = "building_icons/hospital",
        prefabPath = "Prefabs/Buildings/hospital"
    };

    new void Awake()
    {
        base.Awake();
    }

    public override void CheckIsWorking()
    {
        CheckHarborConnection();

        bool hasHerbs = island.products.GetQuantity("herb") > 0;

        var msg = GetBuildingInfo().buildingName + " does not have enough employees to work";
        ManageLog(msg, productivity < 10);

        msg = GetBuildingInfo().buildingName + " does not have enough herbs to work";
        ManageLog(msg, !hasHerbs);

        if (productivity >= 10 && isConnectedToHarbor && hasHerbs)
            isWorking = true;
        else
            isWorking = false;
    }

    public override BuildingInfo GetBuildingInfo()
    {
        return buildingInfo;
    }
}
