using Assets.Scripts.Static;
using DevionGames;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class TradeWindow : MonoBehaviour
{
    public GameObject tradeListItem;
    public GameObject personListItem;

    private GameObject buy;
    private GameObject sell;
    private GameObject pick;
    private GameObject personDetails;
    private GameObject tradeDetails;

    private Products productsToSell;
    private Products productsToBuy;
    public List<Person> people;
    private MerchantShip merchantShip;
    private IslandScript island;

    private GameState gameState;

    // Start is called before the first frame update
    void Awake()
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        buy = gameObject.FindChild("Buy", true).FindChild("Slots", true);
        sell = gameObject.FindChild("Sell", true).FindChild("Slots", true);
        pick = gameObject.FindChild("Pick", true).FindChild("Slots", true);
        personDetails = gameObject.FindChild("PersonDetails", true);
        tradeDetails = gameObject.FindChild("TradeDetails", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        PersonListItemTrade.OnListChanged += UpdateLists;
    }

    private void OnDisable()
    {
        PersonListItemTrade.OnListChanged -= UpdateLists;
        gameState.PersonEscape(island);
    }

    public void DetailsMode(bool isPersonDetails)
    {
        if(isPersonDetails)
        {
            tradeDetails.SetActive(false);
            personDetails.SetActive(true);
        }
        else
        {
            personDetails.SetActive(false);
            tradeDetails.SetActive(true);
        }
    }

    public void SetItems(Products productsToSell, Products productsToBuy, List<Person> people, IslandScript island, MerchantShip ship, Harbor harbor)
    {
        this.productsToSell = productsToSell;
        this.productsToBuy = productsToBuy;
        this.people = people;
        this.merchantShip = ship;
        this.island = island;

        foreach (var key in Products.keys)
        {
            if (productsToSell.GetQuantity(key) > 0)
            {
                var price = ProductPrices.prices.GetQuantity(key);
                var discount = harbor.tradeDiscount;
                var item = Instantiate(tradeListItem);
                item.transform.SetParent(sell.transform);
                item.GetComponent<TradeListItem>().SetData("sell", key, productsToSell.GetQuantity(key), price, discount, island, this, ship);
            }
            if (productsToBuy.GetQuantity(key) > 0)
            {
                var price = ProductPrices.prices.GetQuantity(key);
                var discount = harbor.tradeDiscount;
                var item = Instantiate(tradeListItem);
                item.transform.SetParent(buy.transform);
                item.GetComponent<TradeListItem>().SetData("buy", key, productsToBuy.GetQuantity(key), price, discount, island, this, ship);
            }
        }

        UpdateLists();
    }

    private void UpdateLists()
    {
        ClearLists();
        ListPeople();
    }

    private void ListPeople()
    {
        foreach (var person in people)
        {
            var item = Instantiate(personListItem);
            item.transform.SetParent(pick.transform);
            item.GetComponent<PersonListItemTrade>().SetData(person, island, this, merchantShip);
        }
        
    }

    private void ClearLists()
    {
        var listGameObject = pick;

        var c = listGameObject.transform.childCount;

        for (int i = 0; i < c; i++)
        {
            Destroy(listGameObject.transform.GetChild(i).gameObject);
        }
    }

    public void FinishTrade()
    {
        gameState.SetTimeScale(1);
        Destroy(gameObject);
    }
}
