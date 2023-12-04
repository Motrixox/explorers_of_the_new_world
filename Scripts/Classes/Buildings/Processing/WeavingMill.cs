using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WeavingMill : ProcessingBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 28,
        buildingName = "WeavingMill",
        costWood = 700,
        costStone = 300,
        costIron = 500,
        costGold = 900,
        profession = "Mintsmith",
        buildingImagePath = "building_icons/weavingMill",
        prefabPath = "Prefabs/Buildings/weavingMill"
    };

    new void Awake()
    {
        base.Awake();

        productIn = "cotton";
        processedAmountPerEmployee = 30;
        processingRate = 80;
    }

    public override void Produce()
    {
        Calculate();

        island.products.RemoveQuantity(productIn, amountIn);

        Products products = new Products();
        products.AddQuantity("clothes", production);
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
