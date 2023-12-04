using Assets.Scripts.Extensions;
using Assets.Scripts.Interfaces;
using DevionGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.XR;

public class IslandScript : MonoBehaviour
{
    public static readonly List<string> names = new List<string>
    {
        "Ceto Bay", "Itotaki Rock", "Yasumi Isles", "Howlers Tropic",
        "Damona Bay", "Banana Island", "Shadow Haven", "Luana Shores",
        "Dogon Land", "Howlers Bay", "Tua Bay", "Nerina Sanctuary",
        "Bakunawa Retreat", "Luana Land", "Yam-Yam Retreat", "Mizuka Rock"
    };

    // Ryby - 0
    // Zwierzyna - 1
    // Owoce - 2
    // Zio³a - 3
    // Pszenica - 4
    // Winogrona - 5
    // Przyprawy - 6
    // Z³oto - 7
    // Cotton - 8
    // ¯elazo - 9

    public delegate void IslandUpdate();
    public static event IslandUpdate OnIslandUpdate;

    public GameObject rubble;

    public string islandName { get; set; }

    public int productCapacity { get; set; }
    public int transportCapacity { get; set; }

    public Products products { get; set; }
    public Products productsToBeAdded { get; set; }

    public List<Building> buildings { get; set; }
    public List<Person> people { get; set; }

    public int[] resourcesRatio { get; set; } = new int[10];

    public bool isDrought { get; set; } = false;
    public int droughtWeeksLeft { get; set; } = 0;
    public bool isAnimalPlague { get; set; } = false;
    public int animalPlagueWeeksLeft { get; set; } = 0;
    public bool isFishPlague { get; set; } = false;
    public int fishPlagueWeeksLeft { get; set; } = 0;

    protected static GameState gameState;

    private BuildingInterface buildingInterface;

    public string GetResourcesRatio()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Fishes: " + resourcesRatio[0] + "%");
        sb.AppendLine("Animals: " + resourcesRatio[1] + "%");
        sb.AppendLine("Fruits: " + resourcesRatio[2] + "%");
        sb.AppendLine("Herbs: " + resourcesRatio[3] + "%");
        sb.AppendLine("Farms: " + resourcesRatio[4] + "%");
        sb.AppendLine("Grapes: " + resourcesRatio[5] + "%");
        sb.AppendLine("Spices: " + resourcesRatio[6] + "%");
        sb.AppendLine("Gold: " + resourcesRatio[7] + "%");
        sb.AppendLine("Cotton: " + resourcesRatio[8] + "%");
        sb.AppendLine("Iron: " + resourcesRatio[9] + "%");

        return sb.ToString();
    }

    public string[] GetResources()
    {
        string[] sb = new string[3];
        sb[0] = products.GetQuantity("wood") + " wood";
        sb[1] = products.GetQuantity("stone") + " stone";
        sb[2] = products.GetQuantity("iron") + " iron";

        return sb;
    }

    public string GetProductCapacity()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(products.sumOfProducts);
        sb.Append(" / ");
        sb.Append(productCapacity);

        return sb.ToString();
    }

    public void AddProducts()
    {
        CheckProductCapacity();

        if(productsToBeAdded.sumOfProducts == 0)
        {
            productsToBeAdded = new Products();
            return;
        }

        if(products.sumOfProducts >= (productCapacity - 5))
        {
            gameState.gameLog.Log(islandName + ": warehouses are full.", GetPosition(), "Buildings");
            productsToBeAdded = new Products();
            return;
        }

        if(transportCapacity <= 0)
        {
            gameState.gameLog.Log(islandName + ": warehouses are not transporting products.", GetPosition(), "Buildings");
            productsToBeAdded = new Products();
            return;
        }

        if (productsToBeAdded.sumOfProducts + products.sumOfProducts <= productCapacity && productsToBeAdded.sumOfProducts <= transportCapacity)
        {
            products += productsToBeAdded;
        }
        else if (productsToBeAdded.sumOfProducts + products.sumOfProducts > productCapacity && productsToBeAdded.sumOfProducts <= transportCapacity)
        {
            float factor = (productCapacity - products.sumOfProducts) / 1.0f / productsToBeAdded.sumOfProducts;
            products += productsToBeAdded * factor;
            gameState.gameLog.Log(islandName + ": production was limited due to warehouses capacity", GetPosition(), "Buildings");
        }
        else if((productsToBeAdded.sumOfProducts + products.sumOfProducts) <= productCapacity && productsToBeAdded.sumOfProducts > transportCapacity)
        {
            float factor = transportCapacity / 1.0f / productsToBeAdded.sumOfProducts;
            products += productsToBeAdded * factor;
            gameState.gameLog.Log(islandName + ": production was limited due to warehouses transport capabilities", GetPosition(), "Buildings");
        }
        else
        {
            float factor;
            if (productCapacity - products.sumOfProducts > transportCapacity)
            {
                factor = transportCapacity / 1.0f / productsToBeAdded.sumOfProducts;
                gameState.gameLog.Log(islandName + ": production was limited due to warehouses transport capabilities", GetPosition(), "Buildings");
            }
            else
            {
                factor = (productCapacity - products.sumOfProducts) / 1.0f / productsToBeAdded.sumOfProducts;
                gameState.gameLog.Log(islandName + ": production was limited due to warehouses capacity", GetPosition(), "Buildings");
            }
            products += productsToBeAdded * factor;
        }

        productsToBeAdded = new Products();
        RefreshBuildings();
    }

    public void AddBuilding(Building building)
    {
        buildings.Add(building);
        building.island = this;
        
        //DEBUG
        //if (building.GetType().IsSubclassOf(typeof(ProductionBuilding)))
        //{
        //    ((ProductionBuilding)building).DEBUGAddEmployees();
        //    ((ProductionBuilding)building).Calculate();
        //}
        //if (building.GetType().IsSubclassOf(typeof(ProcessingBuilding)))
        //{
        //    ((ProcessingBuilding)building).DEBUGAddEmployees();
        //    ((ProcessingBuilding)building).Calculate();
        //}
        //if (building.GetType().IsSubclassOf(typeof(ServiceBuilding)))
        //{
        //    ((ServiceBuilding)building).DEBUGAddEmployees();
        //    ((ServiceBuilding)building).Calculate();
        //}
        //DEBUG
    }

    public void RemoveBuilding(GameObject buildingGameObject)
    {
        var b = buildingGameObject.GetComponent<Building>();

        buildings.Remove(b);

        //foreach (var item in buildings)
        //{
        //    if(buildingGameObject == item.gameObject)
        //    {
        //        buildings.Remove(item);
        //        break;
        //    }
        //}
    }

    public void StartRevolt(Building b)
    {
        KillPeopleInBuilding(b, "revolt");
        var r = gameState.random.Next(0, 2);
        if(r == 0)
            DestroyBuilding(b);
    }

    public void SetFire(bool hasWorkingFireStation)
    {
        if(hasWorkingFireStation)
        {
            var b = buildings.PickRandom();

            if(b.GetType().Equals(typeof(Harbor)))
                b = buildings.PickRandom();

            if(b.GetType().Equals(typeof(Harbor)))
                return;

            gameState.gameLog.Log(((IBuildingInfo) b).GetBuildingInfo().buildingName + ": there was a fire", b.transform.position, "Disasters");

            KillPeopleInBuilding(b, "fire");
            DestroyBuilding(b);
        }
        else
        {
            var b = buildings.PickRandom();

            if (b.GetType().Equals(typeof(Harbor)))
                b = buildings.PickRandom();

            if (b.GetType().Equals(typeof(Harbor)))
                return;

            gameState.gameLog.Log(((IBuildingInfo)b).GetBuildingInfo().buildingName + ": there was a fire", b.transform.position, "Disasters");

            KillPeopleInBuilding(b, "fire");
            DestroyBuilding(b);

            foreach (var building in GetSurroundingBuildings(b))
            {
                if (building.GetType().Equals(typeof(Harbor)))
                    continue;

                KillPeopleInBuilding(building, "fire");
                DestroyBuilding(building);
            }
        }
    }

    private List<Building> GetSurroundingBuildings(Building b)
    {
        var list = new List<Building>();
        var pos1 = b.transform.position;
        foreach (var building in buildings)
        {
            var pos2 = building.transform.position;
            var xLength = pos2.x - pos1.x;
            var zLength = pos2.z - pos1.z;

            if ((xLength * xLength + zLength * zLength) < 9)
            {
                list.Add(building);
            }
        }

        return list;
    }

    private void DestroyBuilding(Building b)
    {
        foreach (var c in b.coords)
        {
            gameState.worldSurface[(int)c.x, (int)c.y] = 99;
            var r = Instantiate(rubble);
            r.transform.SetParent(gameObject.transform);
            r.transform.position = new Vector3((int)c.x, -0.5f, (int)c.y);
        }
        buildings.Remove(b);

        if (buildingInterface.building != null && buildingInterface.building.gameObject == b.gameObject)
            buildingInterface.gameObject.SetActive(false);

        Destroy(b.gameObject);
    }

    private void KillPeopleInBuilding(Building b, string reason)
    {
        if (b is IEmployees employees)
        {
            var emp = new List<Person>(employees.employees);

            foreach (var e in emp)
            {
                if (gameState.random.Next(0, 2) == 0)
                {
                    gameState.gameLog.Log(e.name + " died in a " + reason, b.transform.position, "People");
                    e.KillPerson();
                }
            }

            //if (gameState.random.Next(0, 2) == 0 && employees.manager != null)
            //{
            //    gameState.gameLog.Log(employees.manager.name + " died in a " + reason, b.transform.position, "People");
            //    employees.manager.KillPerson();
            //}
        }
        else if (b.GetType().Equals(typeof(House)))
        {
            var res = new List<Person>(((House)b).residents);

            foreach (var r in res)
            {
                if (gameState.random.Next(0, 2) == 0)
                {
                    gameState.gameLog.Log(r.name + " died in a " + reason, b.transform.position, "People");
                    r.KillPerson();
                }
            }
        }

        if (b.GetType().Equals(typeof(School)))
        {
            var res = new List<Person>(((School)b).students.Keys.ToList());

            foreach (var r in res)
            {
                if (gameState.random.Next(0, 2) == 0)
                {
                    gameState.gameLog.Log(r.name + " died in a " + reason, b.transform.position, "People");
                    r.KillPerson();
                }
            }
        }
    }

    public List<Building> GetBuildingsOfType(Type type)
    {
        var list = new List<Building>();

        foreach(var b in buildings) 
        { 
            if(b.GetType().Equals(type))
                list.Add(b);
        }

        return list;
    }

    private void CheckProductCapacity()
    {
        //productCapacity = 1000000; // debug only
        productCapacity = 0;
        transportCapacity = 0;

        foreach (var b in buildings)
        {
            if (b.GetType().Equals(typeof(Harbor)))
            {
                productCapacity += ((Harbor)b).productCapacity;
            }
            else if (b.GetType().Equals(typeof(Warehouse)))
            {
                productCapacity += ((Warehouse)b).productCapacity;
                transportCapacity += ((Warehouse)b).transportCapacity;
            }
        }
    }

    private List<Harbor> GetHarbors()
    {
        var list = new List<Harbor>();

        foreach (var b in buildings)
        {
            if (b.GetType().Equals(typeof(Harbor)))
            {
                list.Add((Harbor)b);
            }
        }

        return list;
    }

    public void RefreshBuildings()
    {
        CheckProductCapacity();
        OnIslandUpdate?.Invoke();
    }

    public List<Person> GetUnemployed()
    {
        List<Person> list = new List<Person>();

        foreach (var p in people)
        {
            if (p.job == null)
                list.Add(p);
        }

        return list;
    }

    public List<Person> GetPotentialStudents()
    {
        List<Person> list = new List<Person>();

        foreach (var p in people)
        {
            if (p.job == null && p.learnedProfessions.Count < p.professionsCapacity)
                list.Add(p);
        }

        return list;
    }

    public List<Person> GetHomeless()
    {
        List<Person> list = new List<Person>();

        foreach (var p in people)
        {
            if (p.home == null)
                list.Add(p);
        }

        return list;
    }

    public Vector3 GetPosition()
    {
        var list = GetBuildingsOfType(typeof(Harbor));

        if(list.Count > 0)
        {
            return list.FirstOrDefault().transform.position;
        }

        var pos = transform.position;
        pos.x += 50f;
        pos.z += 50f;
        return pos;
    }

    public void CheckFood()
    {
        if (products.sumOfFood < 10 && people.Count > 0)
        {
            gameState.gameLog.Log(islandName + " ran out of food.", GetPosition(), "Disasters");
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        //islandName = names.PickRandom();
        //names.Remove(islandName);

        buildingInterface = GameObject.Find("Canvas").FindChild("Building Interface", true).GetComponent<BuildingInterface>();
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        products = new Products();
        //products.AddQuantity("wood", 2000);
        //products.AddQuantity("stone", 2000);
        //products.AddQuantity("iron", 2000);
        //products.AddQuantity("meat", 200);
        //products.AddQuantity("vegetable", 200);
        //products.AddQuantity("bread", 200);
        //products.AddQuantity("fruit", 200);
        //products.AddQuantity("fish", 200);
        //products.AddQuantity("spice", 200);
        //products.AddQuantity("cotton", 200);
        //products.AddQuantity("jewelry", 200);

        //foreach (var key in Products.keys)
        //{
        //    products.AddQuantity(key, 10000);
        //}

        //foreach (var key in Products.food)
        //{
        //    products.RemoveQuantity(key, 10000);
        //}
        //products.AddQuantity("fish", 20000);

        //foreach (var key in Products.good)
        //{
        //    products.RemoveQuantity(key, 10000);
        //}

        buildings = new List<Building>();
        people = new List<Person>();
        //{
        //    new Person(Guid.NewGuid().GetHashCode()),
        //    new Person(Guid.NewGuid().GetHashCode()),
        //    new Person(Guid.NewGuid().GetHashCode())
        //};
        productsToBeAdded = new Products();

        //foreach (var p in people)
        //{
        //    p.island = this;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
