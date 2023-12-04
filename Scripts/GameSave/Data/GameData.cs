using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // world generation
    public bool useSetSeed;
    public int seed;

    public int goldAmount;
    public int[][] worldSurface;
    public List<ShipSerialized> shipList;
    public List<MerchantSerialized> merchantList;
    public IslandSerialized[] islands;
    public float timeUntilNextWeek;
    public float timeSinceStart;
    public int nextWeekSeed;
    public Vector3 cameraPos;

    public GameData()
    {
        useSetSeed = false;
        seed = 0;

        goldAmount = 10000;
        worldSurface = new int[400][];

        for (int i = 0; i < 400; i++)
        {
            worldSurface[i] = new int[400];
        }

        islands = new IslandSerialized[16];
        cameraPos = new Vector3(200,0,200);
        shipList = new List<ShipSerialized>();
        merchantList = new List<MerchantSerialized>();
    }

    public static int[,] LoadArray(int[][] input)
    {
        int[,] result = new int[400, 400];

        for (int i = 0; i < 400; i++)
        {
            for (int j = 0; j < 400; j++)
            {
                result[i, j] = input[i][j];
            }
        }

        return result;
    }

    public static int[][] SaveArray(int[,] input)
    {
        int[][] result = new int[400][];

        for (int i = 0; i < 400; i++)
        {
            result[i] = new int[400];
            for (int j = 0; j < 400; j++)
            {
                result[i][j] = input[i,j];
            }
        }

        return result;
    }
}