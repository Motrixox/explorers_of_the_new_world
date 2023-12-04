using Assets.Scripts.Interfaces;
using DevionGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PersonListItemTrade : MonoBehaviour, IPointerEnterHandler
{
    public Texture plus;

    private Text nameAge;
    private Person person;
    private IslandScript island;
    private TradeWindow tradeWindow;
    private MerchantShip merchantShip;

    private PersonDetailsScript details;
    private RawImage image;

    public delegate void ListChanged();
    public static event ListChanged OnListChanged;

    // Start is called before the first frame update
    void Awake()
    {
        nameAge = gameObject.FindChild("NameAge", true).GetComponent<Text>();
        image = gameObject.FindChild("Button", true).GetComponent<RawImage>();
        details = GameObject.Find("Trade Window(Clone)").FindChild("PersonDetails", true).GetComponent<PersonDetailsScript>();
        image.texture = plus;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(Person p, IslandScript i, TradeWindow tw, MerchantShip ship)
    {
        nameAge.text = p.name + " - " + p.age;
        person = p;
        island = i;
        tradeWindow = tw;
        merchantShip = ship;

        
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
        tradeWindow.DetailsMode(true);
        SetDetails();
    }

    public void TakePerson()
    {
        person.island = island;
        island.people.Add(person);
        tradeWindow.people.Remove(person);
        merchantShip.people.Remove(person);
        OnListChanged?.Invoke();
    }
}
