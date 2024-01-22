using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ProcessingBuilding : Building, IEmployees, IWorkingBuilding, IMorale
{
    public int employeeCapacity { get; set; } = 8;
    public int production { get; protected set; } = 0;
    public int maxProduction { get; protected set; } = 0;
    public int productivity { get; protected set; } = 50;
    public bool isWorking { get; set; } = false;

    protected int upgradeLevel = 1;

    protected int processedAmountPerEmployee = 50;
    protected int processingRate = 100;
    protected string productIn = "";
    protected string productOut = "";
    protected int amountIn = 0;

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

    public void CalculateProduction()
    {
        var empCount = employees.Count;

        if (empCount == 0)
        {
            production = 0;
            maxProduction = 0;
            productivity = 0;
            return;
        }

        var x = 0;
        foreach (var employee in employees)
        {
            employee.CalculateProductivity();
            x += employee.productivity;
        }

        var avgProd = x / 100.0 / empCount;
        productivity = x / employeeCapacity;

        amountIn = (int)(processedAmountPerEmployee * empCount * avgProd);

        int amountInStorage = island.products.GetQuantity(productIn);

        var msg2 = "There are no products to process in " + GetBuildingInfo().buildingName;
        ManageLog(msg2, amountInStorage <= 0);

        if (amountIn > amountInStorage)
            amountIn = amountInStorage;

        production = (int)(amountIn * (processingRate / 100.0));

        maxProduction = (int)(processedAmountPerEmployee * empCount * (processingRate / 100.0));
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

    protected abstract void Upgrade();
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
}
