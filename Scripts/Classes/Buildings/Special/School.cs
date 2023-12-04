using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class School : ServiceBuilding
{
    public static BuildingInfo buildingInfo = new BuildingInfo
    {
        buildingName = "School",
        costWood = 800,
        costStone = 600,
        costIron = 300,
        costGold = 1500,
        buildingID = 38,
        profession = "Teacher",
        buildingImagePath = "building_icons/school",
        prefabPath = "Prefabs/Buildings/school"
    };

    public static Dictionary<string, int> professions = new Dictionary<string, int>
    {
        {"Woodcutter", 5 * 700 }, //numOfWeeks * 700, where 700 equals max sum of productivity points for school
        {"Hunter", 5 * 700 },
        {"Stonecutter", 5 * 700 },
        {"Fisherman", 5 * 700 },
        {"Collector", 5 * 700 },
        {"Farmer", 5 * 700 },
        {"Miner", 8 * 700 },
        {"Prostitute", 8 * 700 },
        {"Warehouseman", 8 * 700 },
        {"Ship builder", 8 * 700 },
        {"Sailor", 8 * 700 },
        {"Miller", 8 * 700 },
        {"Baker", 8 * 700 },
        {"Herbalist", 8 * 700 },
        {"Salesman", 10 * 700 },
        {"Guard", 10 * 700 },
        {"Firefighter", 10 * 700 },
        {"Waiter", 10 * 700 },
        {"Bath worker", 10 * 700 },
        {"Weaver", 10 * 700 },
        {"Merchant", 15 * 700 },
        {"Doctor", 15 * 700 },
        {"Priest", 15 * 700 },
        {"Actor", 15 * 700 },
        {"Goldsmith", 15 * 700 },
        {"Teacher", 15 * 700 },
        {"Mintsmith", 15 * 700 }
    };

    public Dictionary<Person, string> students { get; set; }
    public int studentsCapacity { get; private set; } = 8;

    new void Awake()
    {
        base.Awake();
        students = new Dictionary<Person, string>();
    }
    public override BuildingInfo GetBuildingInfo()
    {
        return buildingInfo;
    }

    public void Produce()
    {
        Calculate();

        var productivityPoints = productivity * (employees.Count + 1);
        var s = new Dictionary<Person, string>(students);

        foreach (var student in s)
        {
            student.Key.CalculateProductivity();

            student.Key.professionProgress += (int)(productivityPoints * (student.Key.productivityNoJob * 0.01f));
            if (professions.GetValueOrDefault(student.Value) < student.Key.professionProgress)
            {
                student.Key.learnedProfessions.Add(student.Value);
                RemoveStudent(student.Key);
            }
        }
    }

    public bool AddStudent(Person p, string profession)
    {
        if (students.Count >= studentsCapacity)
        {
            alert.Alert("Cannot add more students!");
            return false;
        }
        if (p.learnedProfessions.Count >= p.professionsCapacity)
        {
            alert.Alert("Person cannot learn more professions!");
            return false;
        }

        p.professionInProgress = profession;
        students.Add(p, profession);
        return true;
    }
    public bool RemoveStudent(Person p)
    {
        if (!students.ContainsKey(p))
        {
            alert.Alert("Student not found in students list!");
            return false;
        }

        p.job = null;
        p.professionInProgress = string.Empty;
        students.Remove(p);
        return true;
    }
}
