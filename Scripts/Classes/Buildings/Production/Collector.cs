using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Collector : ProductionBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 15,
        buildingName = "Collector",
        costWood = 500,
        costStone = 200,
        costIron = 100,
        costGold = 500,
        profession = "Collector",
        buildingImagePath = "building_icons/collector",
        prefabPath = "Prefabs/Buildings/collector"
    };

    new void Awake()
    {
        base.Awake();

        productionFieldId = 1; //tree
        numOfFieldsPerEmployee = 3;
        profitPerField = 30;
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
        productionRatio = island.resourcesRatio[2];
        Calculate();
        
        Products products = new Products();
        products.AddQuantity("fruit", production);

        //productionRatio = island.resourcesRatio[3];
        //CalculateProduction();

        //products.AddQuantity("herb", production);
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
