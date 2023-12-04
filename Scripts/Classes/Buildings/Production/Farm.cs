using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Farm : ProductionBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 21,
        buildingName = "Farm",
        costWood = 600,
        costStone = 300,
        costIron = 400,
        costGold = 600,
        profession = "Farmer",
        buildingImagePath = "building_icons/farm",
        prefabPath = "Prefabs/Buildings/farm"
    };

    new void Awake()
    {
        base.Awake();

        productionFieldId = 10; //farmland
        numOfFieldsPerEmployee = 3;
        profitPerField = 30;
    }
    private void Start()
    {
        productionRatio = island.resourcesRatio[4]; // wheat
    }

    public override void CalculateProduction()
    {
        var msg = GetBuildingInfo().buildingName + " cannot produce anything during drought";
        ManageLog(msg, island.isDrought);

        if (island.isDrought)
        {
            production = 0;
            maxProduction = 0;
            productivity = 0;
            activeFields = 0;
        }
        else
        {
            base.CalculateProduction();
        }
    }

    public override void Produce()
    {
        Calculate();
        
        Products products = new Products();
        products.AddQuantity("wheat", production / 2);
        products.AddQuantity("vegetable", production / 2);
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
