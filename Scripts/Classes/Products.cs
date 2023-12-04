using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Products
{
    [SerializeField]
    private Dictionary<string, int> productQuantities;

    public static List<string> keys { get; private set; } = new List<string> {
        "wood",
        "stone",
        "iron",
        "goldore",
        "fish",
        "wheat",
        "bread",
        "fruit",
        "flour",
        "herb",
        "wine",
        "cotton",
        "spice",
        "jewelry",
        "meat",
        "vegetable",
        "clothes"
    };
    public static List<string> good { get; private set; } = new List<string> {
        "herb",
        "wine",
        "cotton",
        "spice",
        "jewelry",
        "clothes"
    };
    public static List<string> food { get; private set; } = new List<string> {
        "fish",
        "bread",
        "fruit",
        "meat",
        "vegetable"
    };

    public Products()
    {
        productQuantities = new Dictionary<string, int>();
        foreach (string key in keys)
        {
            productQuantities[key] = 0;
        }
    }

    public int sumOfProducts
    {
        get
        {
            int sum = 0;
            foreach (var quantity in productQuantities.Values)
            {
                sum += quantity;
            }
            return sum;
        }
    }

    public int sumOfFood
    {
        get
        {
            int sum = 0;
            foreach (var key in Products.food)
            {
                sum += GetQuantity(key);
            }
            return sum;
        }
    }

    public int GetQuantity(string productName)
    {
        if (productQuantities.TryGetValue(productName, out int quantity))
        {
            return quantity;
        }
        return 0;
    }

    public void SetQuantity(string productName, int quantity)
    {
        productQuantities[productName] = quantity;
    }

    public void AddQuantity(string productName, int quantity)
    {
        if (productQuantities.ContainsKey(productName))
        {
            productQuantities[productName] += quantity;
        }
        else
        {
            productQuantities[productName] = quantity;
        }
    }

    public void RemoveQuantity(string productName, int quantity)
    {
        if (productQuantities.ContainsKey(productName))
        {
            productQuantities[productName] -= quantity;
        }
        else
        {
            productQuantities[productName] = 0;
        }
    }

    public static Products operator +(Products a, Products b)
    {
        Products result = new Products();
        foreach (var key in a.productQuantities.Keys)
        {
            result.SetQuantity(key, a.GetQuantity(key) + b.GetQuantity(key));
        }
        return result;
    }

    public static Products operator *(Products a, float f)
    {
        Products result = new Products();
        foreach (var key in a.productQuantities.Keys)
        {
            result.SetQuantity(key, (int)(a.GetQuantity(key) * f));
        }
        return result;
    }
}
