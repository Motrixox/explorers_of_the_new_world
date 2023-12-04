using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpicePlantation : ProductionBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 19,
        buildingName = "Spice Plantation",
        costWood = 800,
        costStone = 400,
        costIron = 500,
        costGold = 1000,
        profession = "Farmer",
        buildingImagePath = "building_icons/farm",
        prefabPath = "Prefabs/Buildings/spicePlantation"
    };

    new void Awake()
    {
        base.Awake();

        productionFieldId = 8; // spice
        numOfFieldsPerEmployee = 3;
        profitPerField = 20;
    }
    private void Start()
    {
        productionRatio = island.resourcesRatio[6]; // spice
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
        products.AddQuantity("spice", production);
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
