using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Fishing : ProductionBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 13,
        buildingName = "Fishing cottage",
        costWood = 800,
        costStone = 200,
        costIron = 100,
        costGold = 300,
        profession = "Fisherman",
        buildingImagePath = "building_icons/fishing",
        prefabPath = "Prefabs/Buildings/fishing"
    };

    new void Awake()
    {
        base.Awake();

        productionFieldId = -1; //water
        numOfFieldsPerEmployee = 2;
        profitPerField = 30;
    }

    private void Start()
    {
        productionRatio = island.resourcesRatio[0]; // fishes
    }

    public override void CalculateProduction()
    {
        var msg = GetBuildingInfo().buildingName + " cannot produce anything during fish plague";
        ManageLog(msg, island.isFishPlague);

        if (island.isFishPlague)
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
        products.AddQuantity("fish", production);
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
