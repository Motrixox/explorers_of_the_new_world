using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bakery : ProcessingBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 25,
        buildingName = "Bakery",
        costWood = 600,
        costStone = 500,
        costIron = 300,
        costGold = 700,
        profession = "Baker",
        buildingImagePath = "building_icons/bakery",
        prefabPath = "Prefabs/Buildings/bakery"
    };

    new void Awake()
    {
        base.Awake();

        productIn = "flour";
        processedAmountPerEmployee = 30;
        processingRate = 90;
    }

    public override void Produce()
    {
        Calculate();

        island.products.RemoveQuantity(productIn, amountIn);

        Products products = new Products();
        products.AddQuantity("bread", production);
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
