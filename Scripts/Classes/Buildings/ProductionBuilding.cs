using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProductionBuilding : Building, IEmployees, IWorkingBuilding, IMorale
{
    public int employeeCapacity { get; set; } = 8;
    public int production { get; protected set; } = 0;
    public int maxProduction { get; protected set; } = 0;
    public int productivity { get; protected set; } = 50;
    public bool isWorking { get; set; } = false;
    protected int upgradeLevel = 1;

    protected int productionFieldId = 0;
    protected int fieldsInRange = 0;
    public int activeFields { get; protected set; } = 0;
    protected int productionRatio = 100;

    protected static int radius = 5;
    protected static int numOfFieldsPerEmployee = 3;
    protected static int profitPerField = 50;

    //public Person manager { get; set; }
    public List<Person> employees { get; set; }

    protected new void Awake()
    {
        base.Awake();
        employees = new List<Person>();
    }

    public void Calculate()
    {
        CalculateProduction();
        CheckIsWorking();
    }

    public void CheckIsWorking()
    {
        CheckHarborConnection();

        var msg = GetBuildingInfo().buildingName + " does not have enough employees to work";
        ManageLog(msg, productivity < 10);

        if (productivity >= 10 && isConnectedToHarbor)
            isWorking = true;
        else
            isWorking = false;
    }

    public virtual void CalculateProduction()
    {
        FindFieldsInRange(productionFieldId);

        var empCount = employees.Count;

        //var msg = "Manager is needed for the " + GetBuildingInfo().buildingName + " to work";
        //ManageLog(msg, manager == null);

        //if (empCount == 0 || manager == null)
        if (empCount == 0)
        {
            production = 0;
            maxProduction = 0;
            productivity = 0;
            activeFields = 0;
            return;
        }

        var x = 0;
        foreach (var employee in employees)
        {
            employee.CalculateProductivity();
            x += employee.productivity;
        }
        //manager.CalculateProductivity();
        //x += manager.productivity;

        //var avgProd = x / 100.0 / (empCount + 1);
        //productivity = x / (employeeCapacity + 1);
        var avgProd = x / 100.0 / empCount;
        productivity = x / employeeCapacity;

        //activeFields = (int)(numOfFieldsPerEmployee * (empCount + 1) * avgProd);
        activeFields = (int)(numOfFieldsPerEmployee * empCount * avgProd);

        if (activeFields > fieldsInRange)
            activeFields = fieldsInRange;

        production = (int)(activeFields * profitPerField * (productionRatio / 100.0));
        //maxProduction = (int)(numOfFieldsPerEmployee * (empCount + 1) * profitPerField * (productionRatio / 100.0));
        maxProduction = (int)(numOfFieldsPerEmployee * empCount * profitPerField * (productionRatio / 100.0));
    }
    public abstract void Produce();
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
    //public bool AddManager(Person p)
    //{
    //    if (manager != null)
    //    {
    //        alert.Alert("Manager is already set!");
    //        return false;
    //    }

    //    manager = p;
    //    return true;
    //}
    //public bool RemoveManager()
    //{
    //    if (manager == null)
    //    {
    //        alert.Alert("There is no manager already!");
    //        return false;
    //    }

    //    manager = null;
    //    return true;
    //}
    protected abstract void Upgrade();

    public void FindFieldsInRange(int surfaceId)
    {
        fieldsInRange = 0;

        var coords = transform.position;
        var worldSurface = gameState.worldSurface;

        for (int x = 0; x < 400; x++)
        {
            for (int z = 0; z < 400; z++)
            {
                if (worldSurface[x, z] == surfaceId)
                {
                    var xLength = Mathf.Abs(x - coords.x);
                    var zLength = Mathf.Abs(z - coords.z);

                    if ((xLength * xLength + zLength * zLength) < radius * radius)
                        fieldsInRange++;
                }
            }
        }

        var msg = GetBuildingInfo().buildingName + " is not surrounded by fields allowing production";
        ManageLog(msg, fieldsInRange == 0);
    }

    public int GetAvgMorale()
    {
        if (employees.Count == 0)
            return 100;

        var moraleSum = 0;
        foreach (var e in employees)
        {
            moraleSum += e.morale;
        }

        return moraleSum / employees.Count;
    }

    public void DEBUGAddEmployees()
    {
        //DEBUG
        employees.Add(new Person(Guid.NewGuid().GetHashCode(), GetBuildingInfo().profession));
        employees.Add(new Person(Guid.NewGuid().GetHashCode(), GetBuildingInfo().profession));
        employees.Add(new Person(Guid.NewGuid().GetHashCode(), GetBuildingInfo().profession));
        employees.Add(new Person(Guid.NewGuid().GetHashCode(), GetBuildingInfo().profession));
        //manager = new Person(Guid.NewGuid().GetHashCode());

        foreach (var e in employees)
        {
            e.job = gameObject;
            e.island = island;
            island.people.Add(e);
        }
        //manager.job = gameObject;
        //manager.island = island;
        //manager.learnedProfessions.Add(GetBuildingInfo().profession);
        //island.people.Add(manager);
        //DEBUG
    }
}
