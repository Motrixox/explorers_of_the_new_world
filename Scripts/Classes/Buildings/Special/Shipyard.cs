using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Shipyard : ServiceBuilding
{
    private static GameObject shipPrefab;

    public static int shipCostWood { get; private set; } = 5000;
    public static int shipCostStone { get; private set; } = 500;
    public static int shipCostIron { get; private set; } = 1500;
    public static int shipCostCotton { get; private set; } = 2500;
    public static int shipCostGold { get; private set; } = 5000;
    public static int shipTargetProgress { get; private set; } = 5000;

    public int shipWood = 0;
    public int shipStone = 0;
    public int shipIron = 0;
    public int shipCotton = 0;
    public int shipGold = 0;
    public int shipProgress = 0;

    public bool shipInProgress = false;


    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingName = "Shipyard",
        costWood = 800,
        costStone = 700,
        costIron = 1500,
        costGold = 2300,
        buildingID = 42,
        profession = "Ship builder",
        buildingImagePath = "building_icons/shipyard",
        prefabPath = "Prefabs/Buildings/shipyard"
    };

    new void Awake()
    {
        base.Awake();

        if (shipPrefab == null)
            shipPrefab = Resources.Load<GameObject>("Prefabs/Ship");
    }
    public override BuildingInfo GetBuildingInfo()
    {
        return buildingInfo;
    }

    public void Produce()
    {
        Calculate();

        if (!isWorking || !shipInProgress)
            return;

        var productivitySum = productivity * employeeCapacity;

        if(productivitySum + shipProgress > shipTargetProgress)
            productivitySum = shipTargetProgress - shipProgress;

        if (island.products.GetQuantity("wood") < productivitySum)
            productivitySum = island.products.GetQuantity("wood");
        if (island.products.GetQuantity("stone") / 10 < productivitySum)
            productivitySum = island.products.GetQuantity("stone") / 10;
        if (island.products.GetQuantity("iron") * 3 / 10 < productivitySum)
            productivitySum = island.products.GetQuantity("iron") * 3 / 10;
        if (island.products.GetQuantity("cotton") * 5 / 10 < productivitySum)
            productivitySum = island.products.GetQuantity("cotton") * 5 / 10;
        if (gameState.goldAmount < productivitySum)
            productivitySum = gameState.goldAmount;

        island.products.RemoveQuantity("wood", productivitySum);
        island.products.RemoveQuantity("stone", productivitySum / 10);
        island.products.RemoveQuantity("iron", productivitySum * 3 / 10);
        island.products.RemoveQuantity("cotton", productivitySum * 5 / 10);
        gameState.goldAmount -= productivitySum;

        shipWood += productivitySum;
        shipStone += productivitySum / 10;
        shipIron += productivitySum * 3 / 10;
        shipCotton += productivitySum * 5 / 10;
        shipGold += productivitySum;
        shipProgress += productivitySum;

        if(shipProgress >= shipTargetProgress)
        {
            SpawnShip();
            shipWood = 0;
            shipStone = 0;
            shipIron = 0;
            shipCotton = 0;
            shipGold = 0;
            shipProgress = 0;

            shipInProgress = false;
        }    
    }

    private void SpawnShip()
    {
        GameObject newShip = null;

        newShip = Instantiate(shipPrefab);
        newShip.transform.position = FindCoords();
        newShip.GetComponent<NavMeshAgent>().enabled = true;

        Ship shipInstance = new Ship(false, newShip);
        gameState.shipList.Add(shipInstance);
        shipInstance.Calculate();
    }

    private Vector3 FindCoords()
    {
        var c = coords.FirstOrDefault();
        int x = (int)c.x;
        int z = (int)c.y;

        var worldBase = gameState.worldBase;
        int c1 = 0, c2 = 0, c3 = 0, c4 = 0;

        Vector3 result = new Vector3(200, -0.8f, 200);

        for (int i = 0; i < 100; i++)
        {
            if (worldBase[x + i, z] == 0)
                c1++;
            else
                c1 = 0;
            if (worldBase[x - i, z] == 0)
                c2++;
            else
                c2 = 0;
            if (worldBase[x, z + i] == 0)
                c3++;
            else
                c3 = 0;
            if (worldBase[x, z - i] == 0)
                c4++;
            else
                c4 = 0;

            if (c1 >= 4)
            {
                result = new Vector3(x + i - 1, -0.8f, z);
                break;
            }
            if (c2 >= 4)
            {
                result = new Vector3(x - i + 1, -0.8f, z);
                break;
            }
            if (c3 >= 4)
            {
                result = new Vector3(x, -0.8f, z + i - 1);
                break;
            }
            if (c4 >= 4)
            {
                result = new Vector3(x, -0.8f, z - i + 1);
                break;
            }
        }
        return result;
    }

    public void StartBuildingShip()
    {
        shipInProgress = true;
    }
}
