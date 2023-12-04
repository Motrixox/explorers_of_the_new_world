using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Goldsmith : ProcessingBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 26,
        buildingName = "Goldsmith",
        costWood = 500,
        costStone = 400,
        costIron = 700,
        costGold = 2000,
        profession = "Goldsmith",
        buildingImagePath = "building_icons/goldsmith",
        prefabPath = "Prefabs/Buildings/goldsmith"
    };

    new void Awake()
    {
        base.Awake();

        productIn = "goldore";
        processedAmountPerEmployee = 10;
        processingRate = 50;
    }

    public override void Produce()
    {
        Calculate();

        island.products.RemoveQuantity(productIn, amountIn);

        Products products = new Products();
        products.AddQuantity("jewelry", production);
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
