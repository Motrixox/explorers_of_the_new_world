using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Mason : ProductionBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 12,
        buildingName = "Mason",
        costWood = 400,
        costStone = 300,
        costIron = 500,
        costGold = 800,
        profession = "Mason",
        buildingImagePath = "building_icons/mason",
        prefabPath = "Prefabs/Buildings/mason"
    };

    new void Awake()
    {
        base.Awake();

        productionFieldId = 2; //stone

        numOfFieldsPerEmployee = 2;
        profitPerField = 50;
    }

    public override void Produce()
    {
        Calculate();
        
        Products products = new Products();
        products.AddQuantity("stone", production);
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
