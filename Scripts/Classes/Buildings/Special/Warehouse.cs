using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warehouse : ServiceBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingName = "Warehouse",
        costWood = 800,
        costStone = 1000,
        costIron = 700,
        costGold = 1000,
        buildingID = 37,
        profession = "Warehouseman",
        buildingImagePath = "building_icons/warehouse",
        prefabPath = "Prefabs/Buildings/warehouse"
    };


    public int productCapacity { get; set; } = 5000;
    public int transportCapacity { get; set; } = 0;
    public static int maxTransportCapacity { get; private set; } = 2000;


    public override void Calculate()
    {
        base.Calculate();
        CalculateTransport();
    }

    public void CalculateTransport()
    {
        transportCapacity = Int32.Parse((maxTransportCapacity * (0.01f * productivity)).ToString());
    }

    new void Awake()
    {
        base.Awake();
    }
    public override BuildingInfo GetBuildingInfo()
    {
        return buildingInfo;
    }
}
