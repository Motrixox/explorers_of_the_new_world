using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Hunter : ProductionBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 14,
        buildingName = "Hunter",
        costWood = 500,
        costStone = 200,
        costIron = 300,
        costGold = 600,
        profession = "Hunter",
        buildingImagePath = "building_icons/hunter",
        prefabPath = "Prefabs/Buildings/hunter"
    };

    new void Awake()
    {
        base.Awake();

        productionFieldId = 1; //tree
        numOfFieldsPerEmployee = 3;
        profitPerField = 20;
    }

    private void Start()
    {
        productionRatio = island.resourcesRatio[1]; // animals
    }

    public override void CalculateProduction()
    {
        var msg = GetBuildingInfo().buildingName + " cannot produce anything during animal plague";
        ManageLog(msg, island.isAnimalPlague);

        if (island.isAnimalPlague)
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
        products.AddQuantity("meat", production);
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
