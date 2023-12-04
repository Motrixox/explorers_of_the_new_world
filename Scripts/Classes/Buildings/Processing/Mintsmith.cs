using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Mintsmith : ProcessingBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingID = 27,
        buildingName = "Mintsmith",
        costWood = 500,
        costStone = 400,
        costIron = 700,
        costGold = 2000,
        profession = "Mintsmith",
        buildingImagePath = "building_icons/goldsmith",
        prefabPath = "Prefabs/Buildings/mintsmith"
    };

    new void Awake()
    {
        base.Awake();

        productIn = "goldore";
        processedAmountPerEmployee = 10;
        processingRate = 500;
    }

    public override void Produce()
    {
        Calculate();

        island.products.RemoveQuantity(productIn, amountIn);

        gameState.goldAmount += production;
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
