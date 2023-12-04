using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class PersonSerialized
{
    public string id;
    public string name;
    public string gender;
    public string _class;
    public int age;
    public int ageWeeks;

    public string favFood;
    public string favGood;
    public string favBuilding;

    public bool hasFood;
    public bool hasFavFood;
    public bool hasFavGood;
    public bool hasFavBuilding;
    public bool isSick;
    public int sicknessWeeksLeft;
    public int morale;
    public int productivityNoJob;
    public int productivity;

    public List<string> learnedProfessions;
    public int professionProgress;
    public string professionInProgress;
}
