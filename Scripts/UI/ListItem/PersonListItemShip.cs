using DevionGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PersonListItemShip : MonoBehaviour, IPointerEnterHandler
{
    public Texture plus;
    public Texture minus;

    private string action;
    private Text nameAge;
    private Text productivity;
    private Text sick;
    private Person person;
    private Ship ship;
    private IslandScript island;

    private PersonDetailsScript details;
    private RawImage image;

    public delegate void ListChanged();
    public static event ListChanged OnListChanged;

    private static AlertBoxScript alert;

    // Start is called before the first frame update
    void Awake()
    {
        nameAge = gameObject.FindChild("NameAge", true).GetComponent<Text>();
        productivity = gameObject.FindChild("Productivity", true).GetComponent<Text>();
        sick = gameObject.FindChild("Sick", true).GetComponent<Text>();
        image = gameObject.FindChild("Button", true).GetComponent<RawImage>();
        if(alert == null)
            alert = GameObject.Find("Canvas").FindChild("AlertBox", true).GetComponent<AlertBoxScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(Person p, Ship s, IslandScript i, string menuName)
    {
        nameAge.text = p.name + " - " + p.age;
        person = p;
        ship = s;
        island = i;

        switch(menuName)
        {
            case "Passengers":
                details = GameObject.Find("Ship Interface").FindChild("Passengers", true).FindChild("Details", true).GetComponent<PersonDetailsScript>();
                image.texture = minus;
                action = "RemovePassenger";
                break;
            case "AddPassenger":
                details = GameObject.Find("Ship Interface").FindChild("AddPerson", true).FindChild("Details", true).GetComponent<PersonDetailsScript>();
                image.texture = plus;
                action = "AddPassenger";
                break;
            case "AddManager":
                details = GameObject.Find("Ship Interface").FindChild("AddPerson", true).FindChild("Details", true).GetComponent<PersonDetailsScript>();
                image.texture = plus;
                action = "AddManager";
                break;
            case "AddEmployee":
                details = GameObject.Find("Ship Interface").FindChild("AddPerson", true).FindChild("Details", true).GetComponent<PersonDetailsScript>();
                image.texture = plus;
                action = "AddEmployee";
                break;
            case "Employees":
                details = GameObject.Find("Ship Interface").FindChild("Employees", true).FindChild("Details", true).GetComponent<PersonDetailsScript>();
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

        if (p.isSick)
            sick.gameObject.SetActive(true);
        else
            sick.gameObject.SetActive(false);
    }

    private void SetDetails()
    {
        details.SetDetails(person);
    }

    private void ResetDetails()
    {
        details.ResetDetails();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetDetails();
    }

    public void Action()
    {
        switch(action)
        {
            case "AddPassenger":
                AddPassenger();
                break;
            case "RemovePassenger":
                RemovePassenger();
                break;
            case "AddEmployee":
                AddEmployee();
                break;
            case "RemoveEmployee":
                RemoveEmployee();
                break;
        }
    }

    private bool CheckIsCloseToHarbor()
    {
        bool result = ship.CheckIsCloseToHarbor(island);
        if (!result)
            alert.Alert("Ship is not close enough to the harbor!");
        return result;
    }

    private void AddPassenger()
    {
        if (!CheckIsCloseToHarbor())
            return;

        if (ship.AddPassenger(person))
        {
            island.people.Remove(person);
            person.home = ship.shipGameObject;
            person.job = null;
            person.island = null;
            ResetDetails();
        }
        OnListChanged?.Invoke();
        Destroy(this.gameObject);
    }

    private void RemovePassenger()
    {
        if (!CheckIsCloseToHarbor())
            return;

        if (ship.RemovePassenger(person))
        {
            island.people.Add(person);
            person.home = null;
            person.job = null;
            person.island = island;

            if(ship.employees.Contains(person))
                ship.RemoveEmployee(person);

            ResetDetails();
        }
        OnListChanged?.Invoke();
		Destroy(this.gameObject);
	}

    private void AddEmployee()
    {
        if(ship.AddEmployee(person))
        {
            person.job = ship.shipGameObject;
            ship.passengers.Remove(person);
            ship.Calculate();
        }
        OnListChanged?.Invoke();
		Destroy(this.gameObject);
	}

    private void RemoveEmployee()
    {
        if (ship.passengers.Count >= ship.passengersCapacity)
            return;

        if(ship.RemoveEmployee(person))
        {
            person.job = null;
            ship.passengers.Add(person);
            ship.Calculate();
        }
        
        OnListChanged?.Invoke();
		Destroy(this.gameObject);
	}
}
