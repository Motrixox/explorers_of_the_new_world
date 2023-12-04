using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Herbalist : ProductionBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 44,
        buildingName = "Herbalist",
        costWood = 500,
        costStone = 200,
        costIron = 100,
        costGold = 500,
        profession = "Herbalist",
        buildingImagePath = "building_icons/farm",
        prefabPath = "Prefabs/Buildings/herbalist"
    };

    new void Awake()
    {
        base.Awake();

        productionFieldId = 43; //herb
        numOfFieldsPerEmployee = 3;
        profitPerField = 20;
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
        productionRatio = island.resourcesRatio[3];
        Calculate();
        
        Products products = new Products();
        products.AddQuantity("herb", production);

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
