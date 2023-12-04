using DevionGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfessionListItem : MonoBehaviour
{
    private Text professionName;
    private Text estimatedTime;

    private School schoolBuilding;
    private Person person;
    private BuildingInterface buildingInterface;

    // Start is called before the first frame update
    void Awake()
    {
        professionName = gameObject.FindChild("Name", true).GetComponent<Text>();
        estimatedTime = gameObject.FindChild("Time", true).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(Person p, School s, string name, BuildingInterface bi)
    {
        person = p;
        schoolBuilding = s;
        professionName.text = name;
        buildingInterface = bi;

        var prod = s.productivity * (s.employeeCapacity + 1) * (p.productivityNoJob * 0.01f);

        if (prod == 0)
            estimatedTime.text = "999+ weeks";
        else
        {
            var time = (int)(School.professions.GetValueOrDefault(name) / prod) + 1;
            estimatedTime.text = time + " weeks";
        }
    }

    public void Action()
    {
        if (schoolBuilding.AddStudent(person, professionName.text))
            person.job = schoolBuilding.gameObject;
        buildingInterface.ActivateMenu("Students");
    }
}
