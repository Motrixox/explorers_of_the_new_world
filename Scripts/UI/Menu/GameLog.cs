using DevionGames;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameLog : MonoBehaviour
{
    public GameObject logListItem;

    private GameObject disasters;
    private GameObject buildings;
    private GameObject people;

    private GameObject disastersNotification;
    private GameObject buildingsNotification;
    private GameObject peopleNotification;

    private GameObject bottomPanelNotification;

    // Start is called before the first frame update
    void Awake()
    {
        disasters = gameObject.FindChild("Top", true).FindChild("Disasters", true);
        buildings = gameObject.FindChild("Top", true).FindChild("Buildings", true);
        people = gameObject.FindChild("Top", true).FindChild("People", true);

        disastersNotification = gameObject.FindChild("Bottom", true).FindChild("Disasters", true).FindChild("Notification", true);
        buildingsNotification = gameObject.FindChild("Bottom", true).FindChild("Buildings", true).FindChild("Notification", true);
        peopleNotification = gameObject.FindChild("Bottom", true).FindChild("People", true).FindChild("Notification", true);

        bottomPanelNotification = GameObject.Find("Canvas").FindChild("Bottom Panel", true).FindChild("GameLogButton", true).FindChild("Notification", true);

        disastersNotification.SetActive(false);
        buildingsNotification.SetActive(false);
        peopleNotification.SetActive(false);

        ActivateMenu("Disasters");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        ActivateMenu("Disasters");
    }

    public void ActivateMenu(string menuName)
    {
        DeactivateMenu();

        if (menuName.Equals("Disasters"))
        {
            disastersNotification.SetActive(false);
            disasters.SetActive(true);
        }
        else if (menuName.Equals("Buildings"))
        {
            buildingsNotification.SetActive(false);
            buildings.SetActive(true);
        }
        else if (menuName.Equals("People"))
        {
            peopleNotification.SetActive(false);
            people.SetActive(true);
        }
    }

    private void DeactivateMenu()
    {
        disasters.SetActive(false);
        buildings.SetActive(false);
        people.SetActive(false);
    }

    public void Log(string message, Vector3 position, string type)
    {
        GameObject slots = null;

        if (type.Equals("Disasters"))
        {
            disastersNotification.SetActive(true);
            slots = disasters.FindChild("Slots", true);
        }
        else if (type.Equals("Buildings"))
        {
            buildingsNotification.SetActive(true);
            slots = buildings.FindChild("Slots", true);
        }
        else if (type.Equals("People"))
        {
            peopleNotification.SetActive(true);
            slots = people.FindChild("Slots", true);
        }

        var item = Instantiate(logListItem);
        item.transform.SetParent(slots.transform);
        item.transform.localScale = Vector3.one;
        item.GetComponent<LogListItem>().SetData(message, position);

        if (!gameObject.activeInHierarchy)
        {
            bottomPanelNotification.SetActive(true);
        }
    }

    public void ClearLogs()
    {
        ClearLogs(disasters.FindChild("Slots", true));
        ClearLogs(buildings.FindChild("Slots", true));
        ClearLogs(people.FindChild("Slots", true));
    }

    private void ClearLogs(GameObject slots)
    {
        var c = slots.transform.childCount;

        for (int i = 0; i < c; i++)
        {
            Destroy(slots.transform.GetChild(i).gameObject);
        }
    }
}
