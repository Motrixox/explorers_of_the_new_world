using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GoldMine : ProductionBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 16,
        buildingName = "Gold Mine",
        costWood = 1000,
        costStone = 800,
        costIron = 1000,
        costGold = 2000,
        profession = "Miner",
        buildingImagePath = "building_icons/goldmine",
        prefabPath = "Prefabs/Buildings/goldMine"
    };

    new void Awake()
    {
        base.Awake();

        productionFieldId = 4; //gold

        numOfFieldsPerEmployee = 1;
        profitPerField = 20;
    }
    private void Start()
    {
        productionRatio = island.resourcesRatio[7]; // gold
    }

    public override void Produce()
    {
        Calculate();
        
        Products products = new Products();
        products.AddQuantity("goldore", production);
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
