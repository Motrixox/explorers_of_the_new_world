using Assets.Scripts.Interfaces;
using DevionGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PersonListItemBuilding : MonoBehaviour, IPointerEnterHandler
{
    public Texture plus;
    public Texture minus;

    public BuildingInterface buildingInterface;

    private string action;
    private Text nameAge;
    private Text productivity;
    private Text sick;
    private Text progress;
    private Person person;
    private Building building;
    private IEmployees employeesBuilding;
    private House houseBuilding;
    private School schoolBuilding;
    private IslandScript island;

    private PersonDetailsScript details;
    private RawImage image;

    public delegate void ListChanged();
    public static event ListChanged OnListChanged;

    public delegate void ManagerAdded();
    public static event ManagerAdded OnManagerAdded;

    // Start is called before the first frame update
    void Awake()
    {
        nameAge = gameObject.FindChild("NameAge", true).GetComponent<Text>();
        productivity = gameObject.FindChild("Productivity", true).GetComponent<Text>();
        sick = gameObject.FindChild("Sick", true).GetComponent<Text>();
        progress = gameObject.FindChild("Progress", true).GetComponent<Text>();
        //details = gameObject.transform.parent.parent.parent.parent.parent.gameObject.FindChild("Details", true).GetComponent<PersonDetailsScript>();
        image = gameObject.FindChild("Button", true).GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(Person p, Building b, IslandScript i, string menuName)
    {
        nameAge.text = p.name + " - " + p.age;
        person = p;
        building = b;
        island = i;

        if (building is IEmployees)
        {
            employeesBuilding = (IEmployees)building;

            switch (menuName)
            {
                case "AddManager":
                    details = GameObject.Find("Building Interface").FindChild("AddPerson", true).FindChild("Details", true).GetComponent<PersonDetailsScript>();
                    image.texture = plus;
                    action = "AddManager";
                    break;
                case "AddEmployee":
                    details = GameObject.Find("Building Interface").FindChild("AddPerson", true).FindChild("Details", true).GetComponent<PersonDetailsScript>();
                    image.texture = plus;
                    action = "AddEmployee";
                    break;
                case "Employees":
                    details = GameObject.Find("Building Interface").FindChild("Employees", true).FindChild("Details", true).GetComponent<PersonDetailsScript>();
                    image.texture = minus;
                    action = "RemoveEmployee";
                    break;
            }

            productivity.gameObject.SetActive(true);
            p.CalculateProductivity();
            if (p.job == null)
            {
                productivity.text = p.productivityNoJob + "%";
            }
            else
            {
                productivity.text = p.productivity + "%";
            }
        }
        else if (building.GetType().Equals(typeof(House)))
        {
            houseBuilding = (House)building;

            switch (menuName)
            {
                case "AddResident":
                    details = GameObject.Find("Building Interface").FindChild("AddPerson", true).FindChild("Details", true).GetComponent<PersonDetailsScript>();
                    image.texture = plus;
                    action = "AddResident";
                    break;
                case "Residents":
                    details = GameObject.Find("Building Interface").FindChild("Residents", true).FindChild("Details", true).GetComponent<PersonDetailsScript>();
                    image.texture = minus;
                    action = "RemoveResident";
                    break;
            }
        }
        if (building.GetType().Equals(typeof(School)))
        {
            schoolBuilding = (School)building;

            switch (menuName)
            {
                case "AddStudent":
                    details = GameObject.Find("Building Interface").FindChild("AddPerson", true).FindChild("Details", true).GetComponent<PersonDetailsScript>();
                    image.texture = plus;
                    action = "AddStudent";
                    break;
                case "Students":
                    details = GameObject.Find("Building Interface").FindChild("Students", true).FindChild("Details", true).GetComponent<PersonDetailsScript>();
                    image.texture = minus;
                    action = "RemoveStudent";
                    break;
            }

            if(schoolBuilding.students.ContainsKey(p))
            {
                productivity.gameObject.SetActive(false);
                progress.gameObject.SetActive(true);
                progress.text = p.professionProgress * 100 / School.professions.GetValueOrDefault(p.professionInProgress) + "%";
            }
        }

        if (p.isSick)
            sick.gameObject.SetActive(true);
        else
            sick.gameObject.SetActive(false);
    }

    //private void SetDetails()
    //{
    //    if (details == null)
    //        return;

    //    string professions = "";

    //    foreach(var prof in person.learnedProfessions)
    //    {
    //        professions += prof;
    //        professions += ", ";
    //    }
    //    professions = professions.Remove(professions.Length - 2);

    //    details.FindChild("NameAge", true).GetComponent<Text>().text = nameAge.text;
    //    details.FindChild("Food", true).GetComponent<Text>().text = person.favFood;
    //    details.FindChild("Good", true).GetComponent<Text>().text = person.favGood;
    //    details.FindChild("Building", true).GetComponent<Text>().text = person.favBuilding;
    //    details.FindChild("Professions", true).GetComponent<Text>().text = professions;
    //    details.FindChild("Morale", true).GetComponent<Text>().text = person.morale.ToString();
    //    details.FindChild("Productivity", true).GetComponent<Text>().text = person.productivity.ToString();

    //    if (person.hasFavFood)
    //        details.FindChild("Food", true).GetComponent<Text>().color = Color.green;
    //    else
    //        details.FindChild("Food", true).GetComponent<Text>().color = Color.red;

    //    if (person.hasFavGood)
    //        details.FindChild("Good", true).GetComponent<Text>().color = Color.green;
    //    else
    //        details.FindChild("Good", true).GetComponent<Text>().color = Color.red;

    //    if (person.hasFavBuilding)
    //        details.FindChild("Building", true).GetComponent<Text>().color = Color.green;
    //    else
    //        details.FindChild("Building", true).GetComponent<Text>().color = Color.red;
    //}

    //private void ResetDetails()
    //{
    //    details.FindChild("NameAge", true).GetComponent<Text>().text = "";
    //    details.FindChild("Food", true).GetComponent<Text>().text = "";
    //    details.FindChild("Good", true).GetComponent<Text>().text = "";
    //    details.FindChild("Building", true).GetComponent<Text>().text = "";
    //    details.FindChild("Food", true).GetComponent<Text>().color = Color.white;
    //    details.FindChild("Good", true).GetComponent<Text>().color = Color.white;
    //    details.FindChild("Building", true).GetComponent<Text>().color = Color.white;
    //    details.FindChild("Professions", true).GetComponent<Text>().text = "";
    //    details.FindChild("Morale", true).GetComponent<Text>().text = "";
    //    details.FindChild("Productivity", true).GetComponent<Text>().text = "";
    //}

    public void OnPointerEnter(PointerEventData eventData)
    {
        details.SetDetails(person);
    }

    public void Action()
    {
        switch(action)
        {
            //case "AddManager":
            //    AddManager();
            //    break;
            case "AddEmployee":
                AddEmployee();
                break;
            case "RemoveEmployee":
                //if (person == employeesBuilding.manager)
                //{
                //    RemoveManager();
                //}
                //else
                //{
                    RemoveEmployee();
                //}
                break;
            case "AddResident":
                AddResident();
                break;
            case "RemoveResident":
                RemoveResident();
                break;
            case "AddStudent":
                AddStudent();
                break;
            case "RemoveStudent":
                RemoveStudent();
                break;
        }

        if (building is IEmployees)
            ((IEmployees)building).Calculate();
    }

    //private void AddManager()
    //{
    //    if(employeesBuilding.AddManager(person))
    //        person.job = building.gameObject;
    //    OnManagerAdded?.Invoke();
    //}

    //private void RemoveManager()
    //{
    //    if(employeesBuilding.RemoveManager())
    //        person.job = null;
    //    OnManagerAdded?.Invoke();
    //}

    private void AddEmployee()
    {
        if(employeesBuilding.AddEmployee(person))
        {
            person.job = building.gameObject;
            person.CalculateProductivity();
        }
        OnListChanged?.Invoke();
    }

    private void RemoveEmployee()
    {
        if(employeesBuilding.RemoveEmployee(person))
        {
            person.job = null;
            person.CalculateProductivity();
        }
        OnListChanged?.Invoke();
    }

    private void AddResident()
    {
        if(houseBuilding.AddResident(person))
        {
            person.home = building.gameObject;
            person.CalculateProductivity();
        }
        OnListChanged?.Invoke();
    }

    private void RemoveResident()
    {
        if(houseBuilding.RemoveResident(person))
        {
            person.home = null;
            person.CalculateProductivity();
        }
        OnListChanged?.Invoke();
    }

    private void AddStudent()
    {
        buildingInterface.ListProfessions(person);
        buildingInterface.ActivateMenu("Professions");
    }

    private void RemoveStudent()
    {
        if(schoolBuilding.RemoveStudent(person))
            person.job = null;
        OnListChanged?.Invoke();
    }
}
