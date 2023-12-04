using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ServiceBuilding : Building, IEmployees, IWorkingBuilding, IMorale
{
    public int employeeCapacity { get; set; } = 6;
    public int productivity { get; protected set; } = 50;
    public bool isWorking { get; set; } = false;
    protected int upgradeLevel = 1;

    //public Person manager { get; set; }
    public List<Person> employees { get; set; }

    protected new void Awake()
    {
        base.Awake();
        employees = new List<Person>();
    }

    public virtual void Calculate()
    {
        CalculateProductivity();
        CheckIsWorking();
    }

    public virtual void CheckIsWorking()
    {
        CheckHarborConnection();

        var msg = GetBuildingInfo().buildingName + " does not have enough employees to work";
        ManageLog(msg, productivity < 10);

        if (productivity >= 10 && isConnectedToHarbor)
            isWorking = true;
        else
            isWorking = false;
    }

    protected void CalculateProductivity()
    {
        var empCount = employees.Count;

        //var msg = "Manager is needed for the " + GetBuildingInfo().buildingName + " to work";
        //ManageLog(msg, manager == null);

        //if (empCount == 0 || manager == null)
        if (empCount == 0)
        {
            productivity = 0;
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

        //productivity = x / (employeeCapacity + 1);
        productivity = x / employeeCapacity;
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
    //protected abstract void Upgrade();
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
