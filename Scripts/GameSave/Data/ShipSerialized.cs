using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class ShipSerialized
{

    public string shipName;
    public int productCapacity;
    public int employeeCapacity;
    public int passengersCapacity;

    public Products products;
    public List<PersonSerialized> employees;
    public List<PersonSerialized> passengers;

    public int productivity;
    public float speed;
    public float angularSpeed;
    public Vector3 destination;
    public Vector3 position;
    public Quaternion rotation;
}

