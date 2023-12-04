using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Sawmill : ProductionBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 11,
        buildingName = "Sawmill",
        costWood = 500,
        costStone = 300,
        costIron = 200,
        costGold = 400,
        profession = "Woodcutter",
        buildingImagePath = "building_icons/sawmill",
        prefabPath = "Prefabs/Buildings/sawmill"
    };

    new void Awake()
    {
        base.Awake();

        productionFieldId = 1; //tree

        numOfFieldsPerEmployee = 3;
        profitPerField = 50;
    }

    public override void Produce()
    {
        Calculate();
        
        Products products = new Products();
        products.AddQuantity("wood", production);
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
