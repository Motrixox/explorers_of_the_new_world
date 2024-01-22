using Assets.Scripts.Extensions;
using DevionGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class Ship
{
    public static readonly string prefabPath = "Prefabs/Ship";

    private List<string> names = new List<string> 
    {
        "Emperor",
        "The Castlereagh",
        "Maxton",
        "Imperieuse",
        "Majestic",
        "Ekins",
        "Hughes",
        "Julian",
        "The Recovery",
        "The Spitfire"
    };


    public string shipName;
    public int productCapacity;
    public int employeeCapacity;
    public int passengersCapacity;

    public Products products { get; private set; }
    public GameObject shipGameObject { get; set; }
    public List<Person> employees { get; private set; }
    public List<Person> passengers { get; private set; }

    public int productivity = 50;
    public float speed = 3.5f;
    public float maxSpeed = 3.5f;
    public float angularSpeed = 120;
    public float maxAngularSpeed = 120;

    private IslandScript currentIsland;

    public int radius = 5;
    private static AlertBoxScript alert;
    private static GameState gameState;

    public Ship(bool firstShip, GameObject ship)
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        alert = GameObject.Find("Canvas").FindChild("AlertBox", true).GetComponent<AlertBoxScript>();
        shipName = names.PickRandom();
        shipGameObject = ship;

        productCapacity = 20000;
        employeeCapacity = 8;
        passengersCapacity = 20;
        employees = new List<Person>();
        passengers = new List<Person>();
        products = new Products();

        if (!firstShip)
            return;

        products.AddQuantity("wood", 6500);
        products.AddQuantity("stone", 6500);
        products.AddQuantity("iron", 3500);

        foreach (var key in Products.food)
        {
            products.AddQuantity(key, 700);
        }

        for (int i = 0; i < 8; i++)
        {
            var e = PersonFactory.CreatePerson(gameState.random.Next(), "Sailor");
            e.job = shipGameObject;
            e.home = shipGameObject;
            e.island = null;
            e.CalculateProductivity();
            employees.Add(e);
        }

        for (int i = 0; i < 20; i++)
        {
            var e = PersonFactory.CreatePerson(gameState.random.Next());
            e.job = null;
            e.home = shipGameObject;
            e.island = null;
            e.CalculateProductivity();
            passengers.Add(e);
        }
    }


    public void Calculate()
    {
        var x = 0;
        foreach (var employee in employees)
        {
            employee.CalculateProductivity();
            x += employee.productivity;
        }

        productivity = x / employeeCapacity;

        speed = maxSpeed * productivity * 0.01f;
        angularSpeed = maxAngularSpeed * productivity * 0.01f;

        if(speed < 1f)
            speed = 1f;

        if(angularSpeed < 40)
            angularSpeed = 40;

        shipGameObject.GetComponent<ShipController>().SetSpeed(speed, angularSpeed);
    }

    public void AddProduct(string name, IslandScript currentIsland)
    {
        var activeShip = this;

        name = name.ToLower();

        if (currentIsland.products.GetQuantity(name) >= 100 && activeShip.products.sumOfProducts + 100 <= activeShip.productCapacity)
        {
            currentIsland.products.RemoveQuantity(name, 100);
            activeShip.products.AddQuantity(name, 100);
        }
        else if (activeShip.products.sumOfProducts + 100 > activeShip.productCapacity) alert.Alert("Cannot add more resources!");
        else alert.Alert("Not enough resources!");
    }
    public void ReleaseProduct(string name, IslandScript currentIsland)
    {
        var activeShip = this;

        name = name.ToLower();

        if (activeShip.products.GetQuantity(name) >= 100 && currentIsland.products.sumOfProducts + 100 <= currentIsland.productCapacity)
        {
            currentIsland.products.AddQuantity(name, 100);
            activeShip.products.RemoveQuantity(name, 100);
        }
        else if (currentIsland.products.sumOfProducts + 100 > currentIsland.productCapacity) alert.Alert("Island cannot take more resources!");
        else alert.Alert("Not enough resources!");
    }
    public bool AddEmployee(Person p)
    {
        if (employees.Count >= employeeCapacity)
        {
            alert.Alert("Cannot add more employees!");
            return false;
        }

        employees.Add(p);
        return true;
    }
    public bool RemoveEmployee(Person p)
    {
        if (employees.IndexOf(p) < 0)
        {
            alert.Alert("Employee not found in employees list!");
            return false;
        }

        employees.Remove(p);
        return true;
    }
    public bool AddPassenger(Person p)
    {
        if (passengers.Count >= passengersCapacity)
        {
            alert.Alert("Cannot add more passengers!");
            return false;
        }

        passengers.Add(p);
        return true;
    }
    public bool RemovePassenger(Person p)
    {
        if (passengers.IndexOf(p) < 0)
        {
            alert.Alert("Passenger not found in passenger list!");
            return false;
        }

        passengers.Remove(p);
        return true;
    }

    public List<Person> GetUnemployed()
    {
        List<Person> list = new List<Person>();

        foreach (Person p in passengers)
        {
            if (p.job == null) 
                list.Add(p);
        }
        return list;
    }

    public bool CheckIsCloseToHarbor(IslandScript currentIsland)
    {
        var coords = shipGameObject.transform.position;

        foreach (var building in currentIsland.buildings)
        {
            if (building.GetType().Equals(typeof(Harbor)))
            {
                var buildingCoords = building.gameObject.transform.position;

                var xLength = coords.x - buildingCoords.x;
                var zLength = coords.z - buildingCoords.z;

                if ((xLength * xLength + zLength * zLength) <= (radius * radius))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void CheckCurrentIsland()
    {
        foreach (var island in gameState.islands)
        {
            if (shipGameObject.transform.position.x >= island.transform.position.x &&
                shipGameObject.transform.position.x < island.transform.position.x + 100 &&
                shipGameObject.transform.position.z >= island.transform.position.z &&
                shipGameObject.transform.position.z < island.transform.position.z + 100)
            {
                if (currentIsland == island.GetComponent<IslandScript>())
                {
                    break;
                }

                currentIsland = island.GetComponent<IslandScript>();

                break;
            }
        }
    }

    public Harbor GetCloseHarbor()
    {
        CheckCurrentIsland();

        var coords = shipGameObject.transform.position;

        foreach (var building in currentIsland.buildings)
        {
            if (building.GetType().Equals(typeof(Harbor)))
            {
                var buildingCoords = building.gameObject.transform.position;

                var xLength = coords.x - buildingCoords.x;
                var zLength = coords.z - buildingCoords.z;

                if ((xLength * xLength + zLength * zLength) <= (radius * radius))
                {
                    return (Harbor)building;
                }
            }
        }
        return null;
    }
    public void CheckFood()
    {
        if (products.sumOfFood < 10)
        {
            var pos = shipGameObject.transform.position;
            pos.y = 0;
            gameState.gameLog.Log(shipName + " ran out of food.", pos, "Disasters");
        }
    }

    public ShipSerialized GetSerialized()
    {
        ShipSerialized ship = new ShipSerialized
        {
            productivity = productivity,
            angularSpeed = angularSpeed,
            employeeCapacity = employeeCapacity,
            productCapacity = productCapacity,
            passengersCapacity = passengersCapacity,
            speed = speed,
            shipName = shipName,
            destination = shipGameObject.GetComponent<ShipController>().destinationPos,
            position = shipGameObject.transform.position,
            rotation = shipGameObject.transform.rotation,
            products = products,
            passengers = new List<PersonSerialized>(),
            employees = new List<PersonSerialized>(),
        };

        foreach (var p in passengers)
        {
            ship.passengers.Add(p.GetSerialized());
        }
        foreach (var e in employees)
        {
            ship.employees.Add(e.GetSerialized());
        }

        return ship;
    }

    public Ship(ShipSerialized ship)
    {
        productivity = ship.productivity;
        angularSpeed = ship.angularSpeed;
        employeeCapacity = ship.employeeCapacity;
        productCapacity = ship.productCapacity;
        passengersCapacity = ship.passengersCapacity;
        speed = ship.speed;
        shipName = ship.shipName;
        products = ship.products;
        passengers = new List<Person>();
        employees = new List<Person>();

        foreach (var p in ship.passengers) 
        {
            var person = new Person(p);
            passengers.Add(person);
        }
        foreach (var e in ship.employees) 
        {
            var person = new Person(e);
            employees.Add(person);
        }
    }

    public void SetJobHome()
    {
        foreach (var e in employees)
        {
            e.job = shipGameObject;
            e.home = shipGameObject;
        }
        foreach (var p in passengers)
        {
            p.home = shipGameObject;
        }
    }
}
