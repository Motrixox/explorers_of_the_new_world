using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harbor : ServiceBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingName = "Harbor",
        costWood = 1500,
        costStone = 1000,
        costIron = 700,
        costGold = 2000,
        buildingID = 22,
        profession = "Merchant",
        buildingImagePath = "building_icons/harbor",
        prefabPath = "Prefabs/Buildings/harbor"
    };

    public int productCapacity { get; private set; }
    public float tradeDiscount { get; private set; }
    protected int transportCapacity;

    new void Awake()
    {
        base.Awake();

        productCapacity = 5000;
        employeeCapacity = 8;
    }

    public override void Calculate()
    {
        base.Calculate();
        CalculateTradeDiscount();
    }

    protected void CalculateTradeDiscount()
    {
        tradeDiscount = productivity / 90.0f;
    }
    protected void CalculateTransport()
    {
        throw new System.NotImplementedException();
    }
    protected void Transport()
    {
        throw new System.NotImplementedException();
    }
    protected void Upgrade()
    {
        throw new System.NotImplementedException();
    }
    public override BuildingInfo GetBuildingInfo()
    {
        return buildingInfo;
    }
}
