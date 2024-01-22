using Assets.Scripts.Extensions;
using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Person
{
    public string id { get; set; }
    public string name { get; set; }
    public string gender { get; set; }
    public string _class { get; set; }
    public int age { get; set; }
    public int ageWeeks { get; set; }

    public string favFood { get; set; }
    public string favGood { get; set; }
    public string favBuilding { get; set; }

    public bool hasFood { get; set; }
    public bool hasFavFood { get; set; }
    public bool hasFavGood { get; set; }
    public bool hasFavBuilding { get; set; }
    public bool isSick { get; set; } = false;
    public int sicknessWeeksLeft { get; set; } = 0;

    public int morale { get; set; }
    public int productivityNoJob { get; set; }
    public int productivity { get; set; }

    public GameObject job { get; set; }
    public GameObject home { get; set; }
    public IslandScript island { get; set; }

    public List<string> learnedProfessions;
    public int professionsCapacity { get; set; } = 3;
    public int professionProgress { get; set; } = 0;
    public string professionInProgress { get; set; } = string.Empty;

    private float bonusMorale = 0;
    protected static GameState gameState;

    static Person()
    {
		gameState = GameObject.Find("GameState").GetComponent<GameState>();
	}

    public Person()
    {

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
