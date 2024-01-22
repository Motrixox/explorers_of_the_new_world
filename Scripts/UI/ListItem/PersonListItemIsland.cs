using Assets.Scripts.Interfaces;
using DevionGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PersonListItemIsland : MonoBehaviour, IPointerEnterHandler
{
    private Text nameAge;
    private Text sick;
    private Person person;

    private PersonDetailsScript details;

    // Start is called before the first frame update
    void Awake()
    {
        nameAge = gameObject.FindChild("NameAge", true).GetComponent<Text>();
        sick = gameObject.FindChild("Sick", true).GetComponent<Text>();
		details = GameObject.Find("Canvas").FindChild("Island Information", true).FindChild("Top", true).FindChild("People", true).FindChild("Details", true).GetComponent<PersonDetailsScript>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(Person p)
    {
        nameAge.text = p.name + " - " + p.age;
        person = p;

        if (p.isSick)
            sick.gameObject.SetActive(true);
        else
            sick.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        details.SetDetails(person);
    }
}
