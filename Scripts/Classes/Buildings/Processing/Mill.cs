using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Mill : ProcessingBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 24,
        buildingName = "Mill",
        costWood = 1200,
        costStone = 400,
        costIron = 400,
        costGold = 1500,
        profession = "Miller",
        buildingImagePath = "building_icons/mill",
        prefabPath = "Prefabs/Buildings/mill"
    };

    new void Awake()
    {
        base.Awake();

        productIn = "wheat"; 
        processedAmountPerEmployee = 30;
        processingRate = 90;
    }

    public override void Produce()
    {
        Calculate();

        island.products.RemoveQuantity(productIn, amountIn);

        Products products = new Products();
        products.AddQuantity("flour", production);
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
