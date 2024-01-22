using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Extensions;

public class WorldGeneratingScript : MonoBehaviour, IDataPersistence
{
    public GameObject[] islands = new GameObject[16];
    int islandNum = 0;

    public GameObject island;
    public GameObject ground;
    public GameObject waterBlock;
    public GameObject water;
    public GameObject sandSide;
    public GameObject sandInner;
    public GameObject sandOuter;
    public GameObject tree;
    public GameObject stone;
    public GameObject iron;
    public GameObject gold;
    public GameObject ship;

    private static int seed = -2070312515;
    private static bool useSetSeed = false;

    private static int worldSizeX = 400;
    private static int worldSizeZ = 400;

    private static int islandStartX = 0;
    private static int islandStartZ = 0;

    private static int islandSizeX = 100;
    private static int islandSizeZ = 100;

    private static int worldBorderX = 20;
    private static int worldBorderZ = 20;

    private static int initialSquareSizeX = 20;
    private static int initialSquareSizeZ = 20;

    private static int resourceBlocksPerSource = 6;

    private static GameObject navMesh;

    private GameState gameState;

    //private static int numberOfRandomIterations = 30000;

    // world = 0 water
    // world = 1 grass
    // world = 2 sand
    //private int[,] worldBase = new int[worldSizeX, worldSizeZ];
    //private int[,] worldSurface = new int[worldSizeX, worldSizeZ];
    private int[,] worldBase = new int[worldSizeX, worldSizeZ];
    private int[,] worldSurface = new int[worldSizeX, worldSizeZ];

    private System.Random random;


    public bool CheckBorders(int x, int z)
    {
        if (worldBase[x + 1, z] == 1) 
            return true;
        else if (worldBase[x - 1, z] == 1) 
            return true;
        else if (worldBase[x, z + 1] == 1) 
            return true;
        else if (worldBase[x, z - 1] == 1) 
            return true;

        return false;
    }
    
    public int CheckSandType(int x, int z)
    {
        int countSide = 0;
        int countCorner = 0;

        if (worldBase[x + 1, z] == 1)
            countSide++;
        if (worldBase[x - 1, z] == 1)
            countSide++;
        if (worldBase[x, z + 1] == 1)
            countSide++;
        if (worldBase[x, z - 1] == 1)
            countSide++;
        if (worldBase[x - 1, z - 1] == 1)
            countCorner++;
        if (worldBase[x + 1, z - 1] == 1)
            countCorner++;
        if (worldBase[x + 1, z + 1] == 1)
            countCorner++;
        if (worldBase[x - 1, z + 1] == 1)
            countCorner++;

        if (countSide == 1)
            return 2; // regular sand
        else if (countCorner == 1 && countSide == 0)
            return 3; // outer corner
        else if (countSide > 1)
            return 4; // inner corner
        

        return 0; // no sand
    }

    public bool CheckAllBorders(int x, int z, int condition)
    {
        int count = 0;

        if (worldBase[x + 1, z] == 1)
            count++;
        if (worldBase[x - 1, z] == 1)
            count++;
        if (worldBase[x, z + 1] == 1)
            count++;
        if (worldBase[x, z - 1] == 1)
            count++;
        if (worldBase[x - 1, z - 1] == 1)
            count++;
        if (worldBase[x + 1, z - 1] == 1)
            count++;
        if (worldBase[x + 1, z + 1] == 1)
            count++;
        if (worldBase[x - 1, z + 1] == 1)
            count++;

        if (count >= condition)
            return true;

        return false;
    }

    public void removeWaterInside(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            for (int x = worldBorderX; x < islandSizeX - worldBorderX; x++)
            {
                for (int z = worldBorderZ; z < islandSizeZ - worldBorderZ; z++)
                {
                    if (CheckAllBorders(x, z, 5))
                        worldBase[x, z] = 1;
                }
            }
        }
    }

    private void setRandomizer()
    {
        var randomSeed = Guid.NewGuid().GetHashCode();

        if (useSetSeed)
        {
            random = new System.Random(seed);
            Debug.Log(seed);
        }
        else
        {
            seed = randomSeed;
            random = new System.Random(randomSeed);
            Debug.Log(randomSeed);
        }
    }

    private void placeInitialSquare()
    {
        initialSquareSizeX = random.Next(5,25);
        initialSquareSizeZ = 30 - initialSquareSizeX;

        for (int x = 50 - (initialSquareSizeX / 2) + islandStartX; x < 50 + (initialSquareSizeX / 2) + islandStartX; x++)
        {
            for (int z = 50 - (initialSquareSizeZ / 2) + islandStartZ; z < 50 + (initialSquareSizeZ / 2) + islandStartZ; z++)
            {
                worldBase[x, z] = 1;
            }
        }
    }

    private void generateNewGroundBlocks(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            var x = random.Next(worldBorderX + islandStartX, islandSizeX - worldBorderX);
            var z = random.Next(worldBorderZ + islandStartZ, islandSizeZ - worldBorderZ);

            if (worldBase[x, z] == 1) // optymalizacja?
                continue;

            if (CheckBorders(x, z))
                worldBase[x, z] = 1;
        }
    }

    private void instantiateBlocks(int startX, int endX, int startZ, int endZ)
    {
        islands[islandNum] = Instantiate(island);
        islands[islandNum].transform.position = new Vector3(startX, 0, startZ);
        islands[islandNum].name = "island" + islandNum;
        islands[islandNum].tag = "Island";

        for (int x = startX; x < endX; x++)
        {
            for (int z = startZ; z < endZ; z++)
            {
                GameObject newCube = null;

                if (worldBase[x, z] == 1)
                {
                    newCube = Instantiate(ground);
                    newCube.transform.parent = islands[islandNum].transform;
                    newCube.transform.position = new Vector3(x, -0.65f, z);
                }
                else if (worldBase[x, z] == 2)
                {
                    newCube = Instantiate(sandSide);
                    newCube.transform.parent = islands[islandNum].transform;
                    newCube.transform.position = new Vector3(x, -1.15f, z);
                    rotateSand(x, z, newCube);
                }
                else if (worldBase[x, z] == 3)
                {
                    newCube = Instantiate(sandOuter);
                    newCube.transform.parent = islands[islandNum].transform;
                    newCube.transform.position = new Vector3(x, -1.15f, z);
                    rotateOuterSand(x, z, newCube);
                }
                else if (worldBase[x, z] == 4)
                {
                    newCube = Instantiate(sandInner);
                    newCube.transform.parent = islands[islandNum].transform;
                    newCube.transform.position = new Vector3(x, -1.15f, z);
                    rotateInnerSand(x, z, newCube);
                }


                if (worldSurface[x, z] == 1)
                {
                    newCube = Instantiate(tree);
                    newCube.transform.parent = islands[islandNum].transform;
                    newCube.transform.position = new Vector3(x, -0.4f, z);
                }
                else if (worldSurface[x, z] == 2)
                {
                    newCube = Instantiate(stone);
                    newCube.transform.parent = islands[islandNum].transform;
                    newCube.transform.position = new Vector3(x, -0.4f, z);
                }
                else if (worldSurface[x, z] == 3)
                {
                    newCube = Instantiate(iron);
                    newCube.transform.parent = islands[islandNum].transform;
                    newCube.transform.position = new Vector3(x, -0.4f, z);
                }
                else if (worldSurface[x, z] == 4)
                {
                    newCube = Instantiate(gold);
                    newCube.transform.parent = islands[islandNum].transform;
                    newCube.transform.position = new Vector3(x, -0.4f, z);
                }
            }
        }
        islandNum++;
    }

    private void rotateOuterSand(int x, int z, GameObject newCube)
    {
        if (worldBase[x + 1, z + 1] == 1)
            newCube.transform.Rotate(new Vector3(0, 0, 270));
        else if (worldBase[x + 1, z - 1] == 1)
            newCube.transform.Rotate(new Vector3(0, 0, 0));
        else if (worldBase[x - 1, z + 1] == 1)
            newCube.transform.Rotate(new Vector3(0, 0, 180));
        else if (worldBase[x - 1, z - 1] == 1)
            newCube.transform.Rotate(new Vector3(0, 0, 90));
    }

    private void rotateInnerSand(int x, int z, GameObject newCube)
    {
        if (worldBase[x + 1, z] == 1 && worldBase[x, z + 1] == 1)
            newCube.transform.Rotate(new Vector3(0, 0, 90));
        else if (worldBase[x + 1, z] == 1 && worldBase[x, z - 1] == 1)
            newCube.transform.Rotate(new Vector3(0, 0, 0));
        else if (worldBase[x - 1, z] == 1 && worldBase[x, z + 1] == 1)
            newCube.transform.Rotate(new Vector3(0, 0, 180));
        else if (worldBase[x - 1, z] == 1 && worldBase[x, z - 1] == 1)
            newCube.transform.Rotate(new Vector3(0, 0, 270));
    }

    private void rotateSand(int x, int z, GameObject newCube)
    {
        if (worldBase[x + 1, z] == 1)
            newCube.transform.Rotate(new Vector3(0, 0, 270));
        else if (worldBase[x - 1, z] == 1)
            newCube.transform.Rotate(new Vector3(0, 0, 90));
        else if (worldBase[x, z + 1] == 1)
            newCube.transform.Rotate(new Vector3(0, 0, 180));
        else if (worldBase[x, z - 1] == 1)
            newCube.transform.Rotate(new Vector3(0, 0, 0));
    }

    private void generateSand()
    {
        for (int i = 1; i < worldSizeX - 1; i++)
        {
            for (int j = 1; j < worldSizeZ - 1; j++)
            {
                if (worldBase[i, j] == 0)
                    worldBase[i, j] = CheckSandType(i, j);
            }
        }
    }

    private void generateIsland()
    {
        placeInitialSquare();

        int iterations = random.Next(5000, 30000);

        //generateNewGroundBlocks(numberOfRandomIterations);
        generateNewGroundBlocks(iterations);

        removeWaterInside(5);

        generateSand();

        generateTrees();

        generateResources((iterations / 10000) + 1, 2); // 2 = stone
        generateResources((iterations / 10000) + 1, 3); // 3 = iron
        generateResources(random.Next(0,2), 4); // 4 = gold
    }

    private void generateResources(int numOfSources, int type)
    {
        for (int i = 0; i < numOfSources; i++)
        {
            bool isGround = false;
            int sourceX = 0;
            int sourceZ = 0;
            int ret = 0;

            while (!isGround)
            {
                ret++;
                if (ret > 100) return; // anti crash mechanism

                sourceX = random.Next(islandStartX, islandSizeX);
                sourceZ = random.Next(islandStartZ, islandSizeZ);

                if (worldBase[sourceX, sourceZ] == 1)
                { 
                    isGround = true;
                    worldSurface[sourceX, sourceZ] = type;
                }
            }

            for (int j = random.Next(0, 3); j < resourceBlocksPerSource; j++)
            {
                isGround = false;
                ret = 0;

                while (!isGround)
                {
                    ret++;
                    if (ret > 100) return; // anti crash mechanism

                    int blockX = sourceX + random.Next(-2, 3);
                    int blockZ = sourceZ + random.Next(-2, 3);

                    if (worldBase[blockX, blockZ] == 1 && worldSurface[blockX, blockZ] <= 1) // tree or empty
                    {
                        isGround = true;
                        worldSurface[blockX, blockZ] = type;
                    }
                }
            }

        }
    }

    private void generateTrees()
    {
        for (int i = islandStartX; i < islandSizeX; i++)
        {
            for (int j = islandStartZ; j < islandSizeZ; j++)
            {
                if (worldBase[i,j] == 1)
                {
                    if(random.Next(1, 4) == 1)
                    {
                        worldSurface[i, j] = 1; // tree
                    }
                }
            }
        }
    }

    private void placeWater()
    {
        water = Instantiate(water);
        for (int x = -100; x < 500; x += 50)
        {
            for (int z = -100; z < 500; z += 50)
            {
                GameObject newWater = null;

                newWater = Instantiate(waterBlock);
                newWater.transform.parent = water.transform;
                newWater.transform.localScale = Vector3.one + new Vector3(0.01f, 0, 0.01f);
                newWater.transform.position = new Vector3(x, 0, z);
            }
        }
    }

    private void setResources()
    {
        foreach( var island in islands)
        {
            int[] ratio = new int[10];

            for (int i = 0; i < 10; i++)
            {
                ratio[i] = random.Next(30, 101);
            }

            island.GetComponent<IslandScript>().resourcesRatio = ratio;
        }
    }

    private void placeShip()
    {
        GameObject newShip = null;

        newShip = Instantiate(ship);
        newShip.transform.position = new Vector3(200f, -0.8f, 200f);
        newShip.GetComponent<NavMeshAgent>().enabled = true;

        Ship shipInstance = new Ship(true, newShip);
        gameState.shipList.Add(shipInstance);
        shipInstance.Calculate();
    }

    private void SetWaterSurface()
    {
        for (int i = 0; i < worldSizeX; i++)
        {
            for (int j = 0; j < worldSizeZ; j++)
            {
                if (worldBase[i, j] == 0)
                    worldSurface[i, j] = -1; // water
            }
        }
    }

    private void SetIslandNames()
    {
        var names = new List<string>(IslandScript.names);

        foreach (var island in islands)
        {
            var n = names.PickRandom();
            island.GetComponent<IslandScript>().islandName = n;
            names.Remove(n);
        }
    }

    void Awake()
    {
        Application.targetFrameRate = 0;

        gameState = GameObject.Find("GameState").GetComponent<GameState>();

        navMesh = GameObject.Find("NavMesh");

        var s = DataPersistenceManager.instance.GetSeed();
        if(s == 0)
        {
            useSetSeed = false;
        }
        else
        {
            useSetSeed = true;
            seed = s;
        }

        setRandomizer();

        for (int i = 0; i < worldSizeX; i+=100)
        {
            for (int j = 0; j < worldSizeZ; j+=100)
            {
                islandStartX = i;
                islandStartZ = j;
                islandSizeX = i + 100;
                islandSizeZ = j + 100;
                generateIsland();
                instantiateBlocks(i, i + 100, j, j + 100);
            }
        }

        SetIslandNames();

        setResources();

        placeWater();

        gameState.random = random;
        placeShip();

        SetWaterSurface();

        navMesh.GetComponent<Unity.AI.Navigation.NavMeshSurface>().BuildNavMesh();

        gameState.worldBase = worldBase;
        gameState.worldSurface = worldSurface;
        gameState.islands = islands;
        gameState.nextWeekSeed = random.Next();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void LoadData(GameData data)
    {
        useSetSeed = data.useSetSeed;
        if(data.seed != 0)
            seed = data.seed;
    }

    public void SaveData(GameData data)
    {
        data.useSetSeed = true;
        Debug.Log("koniec " + seed);
        data.seed = seed;
    }
}
