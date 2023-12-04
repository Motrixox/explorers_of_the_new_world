using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IronMine : ProductionBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 17,
        buildingName = "Iron Mine",
        costWood = 800,
        costStone = 600,
        costIron = 800,
        costGold = 1500,
        profession = "Miner",
        buildingImagePath = "building_icons/goldmine",
        prefabPath = "Prefabs/Buildings/ironMine"
    };

    new void Awake()
    {
        base.Awake();

        productionFieldId = 3; //iron

        numOfFieldsPerEmployee = 1;
        profitPerField = 30;
    }
    private void Start()
    {
        productionRatio = island.resourcesRatio[9]; // iron
    }

    public override void Produce()
    {
        Calculate();
        
        Products products = new Products();
        products.AddQuantity("iron", production);
        island.productsToBeAdded += products;
    }

    protected override void Upgrade()
    {
        throw new System.NotImplementedException();
    }
    public override BuildingInfo GetBuildingInfo()
    {
        return buildingInfo;
    }
}
