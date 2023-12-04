using Assets.Scripts.Extensions;
using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Person
{
    static readonly string[] genders = { "Male", "Female" };
    static readonly string[] maleNames = { "John", "Paul", "Dave", "Oscar", "Silas", "James", "Jack", "William", "Julian", "Ricky" };
    static readonly string[] femaleNames = { "Lucy", "Daisy", "Scarlett", "Jane", "Juliet", "Evelyn", "Lena", "Annie", "Rosie", "Maggie" };
    static readonly string[] foods = { "Fish", "Meat", "Fruit", "Bread", "Vegetable" };
    static readonly string[] goods = { "Cotton", "Jewelry", "Spice", "Wine", "Herb", "Clothes" };
    static readonly string[] buildings = { "Church", "Baths", "Theater", "Pub", "Brothel", "School", "Hospital", "Market" };
    static readonly string[] classes = { "Working class", "Middle class", "Upper class" };
    //static readonly string[] professions = { "Woodcutter", "Hunter", "Mason", "Miner", "Fisherman", "Collector", "Farmer", "Miller", 
    //                                        "Baker", "Herbalist", "Goldsmith", "Mint", "Waiter", "Actor", "Priest", "Bath worker", "Prostitute", 
    //                                        "Salesman", "Warehouseman", "Merchant", "Teacher", "Guard", "Doctor", "Ship builder", "Firefighter", "Sailor" };
    static readonly string[] wcProfessions = { "Woodcutter", "Hunter", "Mason", "Fisherman", "Collector", "Farmer",  
                                              "Miner", "Prostitute", "Warehouseman",  "Ship builder",  "Sailor" };
    static readonly string[] mcProfessions = { "Miller", "Baker", "Herbalist", "Salesman", "Guard", "Firefighter", "Waiter", "Bath worker", "Weaver" };
    static readonly string[] ucProfessions = { "Merchant", "Doctor", "Priest", "Actor", "Goldsmith", "Teacher", "Mintsmith" };


    public string id { get; set; }
    public string name { get; private set; }
    public string gender { get; private set; }
    public string _class { get; private set; }
    public int age { get; set; }
    public int ageWeeks { get; set; }

    public string favFood { get; private set; }
    public string favGood { get; private set; }
    public string favBuilding { get; private set; }

    public bool hasFood { get; private set; }
    public bool hasFavFood { get; private set; }
    public bool hasFavGood { get; private set; }
    public bool hasFavBuilding { get; private set; }
    public bool isSick { get; set; } = false;
    public int sicknessWeeksLeft { get; set; } = 0;

    public int morale { get; private set; }
    public int productivityNoJob { get; private set; }
    public int productivity { get; private set; }

    public GameObject job { get; set; }
    public GameObject home { get; set; }
    public IslandScript island { get; set; }

    public List<string> learnedProfessions;
    public int professionsCapacity { get; private set; } = 3;
    public int professionProgress { get; set; } = 0;
    public string professionInProgress { get; set; } = string.Empty;

    private float bonusMorale = 0;
    protected static GameState gameState;

    public Person(int seed, string profession)
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();

        id = Guid.NewGuid().ToString();

        var random = new System.Random(seed);

        gender = genders[random.Next(0, 2)];
        if(gender.Equals("Male"))
            name = maleNames[random.Next(0, maleNames.Length)];
        else
            name = femaleNames[random.Next(0, femaleNames.Length)];
        age = random.Next(25, 50);
        ageWeeks = random.Next(1, 53);
        favFood = foods[random.Next(0, foods.Length)];
        favGood = goods[random.Next(0, goods.Length)];
        favBuilding = buildings[random.Next(0, buildings.Length)];
        morale = 70;
        productivity = 30;

        var c = random.Next(1, 101);
        if(c <= 5)
        {
            _class = classes[2];
            learnedProfessions = new List<string>
            {
                ucProfessions[random.Next(0, ucProfessions.Length)]
            };
        }
        else if (c <= 30)
        {
            _class = classes[1];
            learnedProfessions = new List<string>
            {
                mcProfessions[random.Next(0, mcProfessions.Length)]
            };
        }
        else if (c <= 100)
        {
            _class = classes[0];
            var ran1 = random.Next(0, wcProfessions.Length);
            var ran2 = random.Next(0, wcProfessions.Length);

            while(ran1 == ran2) { ran2 = random.Next(0, wcProfessions.Length); }

            learnedProfessions = new List<string>
            {
                wcProfessions[ran1],
                wcProfessions[ran2]
            };
        }

        if(!profession.Equals(string.Empty))
        {
            learnedProfessions.Add(profession);
        }
    }

    public void CalculateProductivity()
    {
        if(isSick)
        {
            productivity = 0;
            return;
        }

        var _base = 30;

        if (home != null) _base += 20;
        if (hasFood) _base += 20;
        if (hasFavBuilding) _base += 10;
        if (hasFavFood) _base += 10; 
        if (hasFavGood) _base += 10;

        productivityNoJob = _base;

        if (job == null)
        {
            productivity = _base;
            return;
        }

        job.gameObject.TryGetComponent<ShipController>(out var ship);

        if (ship != null)
        {
            if (!learnedProfessions.Contains("Sailor"))
            {
                _base = _base / 2;
                bonusMorale -= 1.0f;
            }
        }
        else if (!learnedProfessions.Contains(((IBuildingInfo)job.gameObject.GetComponent<Building>()).GetBuildingInfo().profession))
        {
            _base = _base / 2;
            if(!job.gameObject.GetComponent<Building>().GetType().Equals(typeof(School)))
                bonusMorale -= 1.0f;
        }

        productivity = _base;
    }

    public void Consume(Products products, Products productsToBeAdded)
    {
        ConsumeFood(products, productsToBeAdded);
        ConsumeGood(products, productsToBeAdded);
        CheckFavouriteBuilding();
        CalculateProductivity();
        CalculateMorale();
    }

    private void CalculateMorale()
    {
        var _base = 30;
        if (home != null) _base += 20;
        if (hasFood) _base += 20;
        if (hasFavBuilding) _base += 10;
        if (hasFavFood) _base += 10;
        if (hasFavGood) _base += 10;

        var diff = _base - morale;

        morale += (int)((diff / 10.0f) + bonusMorale);

        if (morale > 100)
            morale = 100;
        else if (morale < 0)
            morale = 0;
    }

    private void ConsumeGood(Products products, Products productsToBeAdded)
    {
        if (products.GetQuantity(favGood.ToLower()) > 4)
        {
            products.RemoveQuantity(favGood.ToLower(), 5);
            hasFavGood = true;
        }
        else if (productsToBeAdded.GetQuantity(favGood.ToLower()) > 4)
        {
            productsToBeAdded.RemoveQuantity(favGood.ToLower(), 5);
            hasFavGood = true;
        }
        else
        {
            hasFavGood = false;
        }


        int i = 0;
        int iterations = 5;

        if (!hasFavFood)
        {
            iterations = 10;
        }

        bonusMorale = 0f;

        while (i < iterations)
        {
            foreach (var key in Products.good.Shuffle())
            {
                if (products.GetQuantity(key) > 0)
                {
                    products.RemoveQuantity(key, 1);
                    bonusMorale += 0.05f;
                    break;
                }
                else if (productsToBeAdded.GetQuantity(key) > 0)
                {
                    productsToBeAdded.RemoveQuantity(key, 1);
                    bonusMorale += 0.05f;
                    break;
                }
            }
            i++;
        }
    }

    private void ConsumeFood(Products products, Products productsToBeAdded)
    {
        if(products.sumOfFood < 10 && productsToBeAdded.sumOfFood < 10)
        {
            hasFavFood = false;
            hasFood = false;

            return;
        }

        if(products.GetQuantity(favFood.ToLower()) > 5)
        {
            products.RemoveQuantity(favFood.ToLower(), 6);
            hasFavFood = true;
        }
        else if(productsToBeAdded.GetQuantity(favFood.ToLower()) > 5)
        {
            productsToBeAdded.RemoveQuantity(favFood.ToLower(), 6);
            hasFavFood = true;
        }
        else
        {
            hasFavFood = false;
        }

        int i = 0;
        int iterations = 4;

        if(!hasFavFood)
        {
            iterations = 10;
        }

        while(i < iterations)
        {
            foreach (var key in Products.food.Shuffle())
            {
                if(products.GetQuantity(key) > 0)
                {
                    products.RemoveQuantity(key, 1);
                    break;
                }
                else if(productsToBeAdded.GetQuantity(key) > 0)
                {
                    productsToBeAdded.RemoveQuantity(key, 1);
                    break;
                }
            }
            i++;
        }

        hasFood = true;
    }

    private void CheckFavouriteBuilding()
    {
        Products p = new Products();

        if (home != null && home.gameObject.CompareTag("Ship"))
        {
            Ship ship = null;
            foreach (var s in gameState.shipList)
            {
                if (s.shipGameObject == home.gameObject)
                {
                    ship = s;
                    break;
                }
            }

            if (ship != null)
            {
                var harbor = ship.GetCloseHarbor();
                if(harbor != null)
                {
                    HasFavouriteBuilding(harbor.island);
                }
            }
        }
        else
        {
            HasFavouriteBuilding();
        }
    }

    private void HasFavouriteBuilding()
    {
        hasFavBuilding = false;
        foreach (var b in island.buildings)
        {
            if (b is IBuildingInfo info && b is IWorkingBuilding working)
            {
                if (info.GetBuildingInfo().buildingName.Equals(favBuilding) && working.isWorking)
                {
                    hasFavBuilding = true;
                    break;
                }
            }
        }
    }

    private void HasFavouriteBuilding(IslandScript island)
    {
        hasFavBuilding = false;
        foreach (var b in island.buildings)
        {
            if (b is IBuildingInfo info && b is IWorkingBuilding working)
            {
                if (info.GetBuildingInfo().buildingName.Equals(favBuilding) && working.isWorking)
                {
                    hasFavBuilding = true;
                    break;
                }
            }
        }
    }

    public void KillPerson()
    {
        if(job != null && !job.gameObject.CompareTag("Ship"))
        {
            var b = job.GetComponent<Building>();

            if(b.GetType().Equals(typeof(School)))
            {
                School school = (School)b;
                if (school.students.Keys.Contains(this))
                {
                    school.RemoveStudent(this);
                }
            }

            //if (((IEmployees)b).manager == this)
            //    ((IEmployees)b).RemoveManager();
            if(((IEmployees)b).employees.Contains(this))
                ((IEmployees)b).RemoveEmployee(this);
            job = null;
        }

        if (home != null && home.gameObject.CompareTag("Ship"))
        {
            Ship ship = null;
            foreach (var s in gameState.shipList)
            {
                if (s.shipGameObject == home.gameObject)
                {
                    ship = s;
                    break;
                }
            }

            if (ship != null)
            {
                //if (ship.manager == this)
                //    ship.RemoveManager();
                if (ship.employees.Contains(this))
                    ship.employees.Remove(this);
                else if (ship.passengers.Contains(this))
                    ship.passengers.Remove(this);
            }
        }
        else if(home != null)
        {
            var b = home.GetComponent<Building>();
            ((House)b).RemoveResident(this);
            home = null;
        }
       
        if(island != null)
        {
            island.people.Remove(this);
            island = null;
        }
    }

    public Vector3 GetPosition()
    {
        if(job != null)
            return job.transform.position;

        if(home != null)
            return home.transform.position;

        if(island != null)
        {
            var harbor = island.GetBuildingsOfType(typeof(Harbor)).FirstOrDefault();
            if (harbor != null)
                return harbor.transform.position;

            return island.GetPosition();
        }

        return Vector3.zero;
    }


    public PersonSerialized GetSerialized()
    {
        PersonSerialized person = new PersonSerialized
        {
            id = id,
            name = name,
            gender = gender,
            _class = _class,
            age = age,
            ageWeeks = ageWeeks,

            favFood = favFood,
            favGood = favGood,
            favBuilding = favBuilding,

            hasFood = hasFood,
            hasFavFood = hasFavFood,
            hasFavGood = hasFavGood,
            hasFavBuilding = hasFavBuilding,
            isSick = isSick,
            sicknessWeeksLeft = sicknessWeeksLeft,
            morale = morale,
            productivityNoJob = productivityNoJob,
            productivity = productivity,

            learnedProfessions = learnedProfessions,
            professionProgress = professionProgress,
            professionInProgress = professionInProgress
};

        return person;
    }

    public Person(PersonSerialized person)
    {
        id = person.id;
        name = person.name;
        gender = person.gender;
        _class = person._class;
        age = person.age;
        ageWeeks = person.ageWeeks;

        favFood = person.favFood;
        favGood = person.favGood;
        favBuilding = person.favBuilding;

        hasFood = person.hasFood;
        hasFavFood = person.hasFavFood;
        hasFavGood = person.hasFavGood;
        hasFavBuilding = person.hasFavBuilding;
        isSick = person.isSick;
        sicknessWeeksLeft = person.sicknessWeeksLeft;
        morale = person.morale;
        productivityNoJob = person.productivityNoJob;
        productivity = person.productivity;

        learnedProfessions = person.learnedProfessions;
        professionProgress = person.professionProgress;
        professionInProgress = person.professionInProgress;
    }
}
