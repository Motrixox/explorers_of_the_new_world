using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DevionGames;

public class PersonDetailsScript : MonoBehaviour
{
    private Text nameAge;
    private Text food;
    private Text good;
    private Text building;
    private Text professions;
    private Text morale;
    private Text productivity;
    private Image job;
    private Image home;

    private Person person;
    private static new GameObject camera;

    // Start is called before the first frame update
    void Awake()
    {
        if (camera == null)
            camera = GameObject.FindWithTag("MainCamera");
        nameAge = gameObject.FindChild("NameAge", true).GetComponent<Text>();
        food = gameObject.FindChild("Food", true).GetComponent<Text>();
        good = gameObject.FindChild("Good", true).GetComponent<Text>();
        building = gameObject.FindChild("Building", true).GetComponent<Text>();
        professions = gameObject.FindChild("Professions", true).GetComponent<Text>();
        morale = gameObject.FindChild("Morale", true).GetComponent<Text>();
        productivity = gameObject.FindChild("Productivity", true).GetComponent<Text>();
        job = gameObject.FindChild("Job", true).GetComponent<Image>();
        home = gameObject.FindChild("Home", true).GetComponent<Image>();

        ResetDetails();
    }

    private void OnDisable()
    {
        ResetDetails();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDetails(Person person)
    {
        this.person = person;
        
        string profs = "";

        foreach (var prof in person.learnedProfessions)
        {
            profs += prof;
            profs += ", ";
        }
        profs = profs.Remove(profs.Length - 2);

        nameAge.text = person.name + " - " + person.age;
        food.text = person.favFood;
        good.text = person.favGood;
        building.text = person.favBuilding;
        professions.text = profs;
        morale.text = person.morale.ToString() + " Morale";
        productivity.text = person.productivity.ToString() + " Productivity";

        if (person.hasFavFood)
            food.color = Color.green;
        else
            food.color = Color.red;

        if (person.hasFavGood)
            good.color = Color.green;
        else
            good.color = Color.red;

        if (person.hasFavBuilding)
            building.color = Color.green;
        else
            building.color = Color.red;

        if (person.job == null)
            job.color = Color.red;
        else 
            job.color = Color.green;

        if (person.home == null)
            home.color = Color.red;
        else
            home.color = Color.green;
    }

    public void ResetDetails()
    {
        nameAge.text = "";
        food.text = "";
        good.text = "";
        building.text = "";
        food.color = Color.white;
        good.color = Color.white;
        building.color = Color.white;
        professions.text = "";
        morale.text = "";
        productivity.text = "";

        person = null;
    }

    public void MoveCameraToJob()
    {
        if (person == null)
            return;

        if (person.job == null)
            return;

        var pos = person.job.gameObject.transform.position;
        pos.y = 0f;
        camera.transform.position = pos;
    }

    public void MoveCameraToHome()
    {
        if (person == null)
            return;

        if (person.home == null)
            return;

        var pos = person.home.gameObject.transform.position;
        pos.y = 0f;
        camera.transform.position = pos;
    }
}
