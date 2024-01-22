using DevionGames;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IslandInformation : MonoBehaviour
{
    public GameObject goodListItem;
    public GameObject personListItem;

    private GameState gameState;

    private GameObject resources;
    private GameObject goods;
    private GameObject info;
    private GameObject people;
    private GameObject title;

    // Start is called before the first frame update
    void Awake()
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        resources = gameObject.FindChild("Top", true).FindChild("Resources", true);
        goods = gameObject.FindChild("Top", true).FindChild("Goods", true);
        info = gameObject.FindChild("Top", true).FindChild("Info", true);
        people = gameObject.FindChild("Top", true).FindChild("People", true);
        title = gameObject.FindChild("Title", true);

        ActivateMenu("Goods");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void IslandChanged()
    {
        SetResources();
        SetCapacity();
        SetGoods();
        SetInfo();
        SetPeople();
        title.GetComponent<Text>().text = gameState.currentIsland.GetComponent<IslandScript>().islandName;
    }

    private void SetResources()
    {
        var res = resources.FindChild("Value", true).GetComponent<Text>();
        res.text = gameState.currentIsland.GetComponent<IslandScript>().GetResourcesRatio();
    }

    private void SetCapacity()
    {
        var productCapacity = gameObject.FindChild("Middle", true).GetComponent<Text>();
        productCapacity.text = gameState.currentIsland.GetComponent<IslandScript>().GetProductCapacity();
    }

    private void SetGoods()
    {
        var products = gameState.currentIsland.GetComponent<IslandScript>().products;

        var parent = goods.FindChild("Slots", true);

        var c = parent.transform.childCount;

        for (int i = 0; i < c; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }

        foreach (var key in Products.keys)
        {
            var listItem = Instantiate(goodListItem);
            listItem.transform.SetParent(parent.transform);
            listItem.GetComponent<GoodListItem>().SetData(key, products.GetQuantity(key));
        }
    }

    private void SetInfo()
    {
        var islandScript = gameState.currentIsland.GetComponent<IslandScript>();

		info.FindChild("PeopleCount", true).GetComponent<Text>().text = islandScript.people.Count.ToString();
        info.FindChild("BuildingsCount", true).GetComponent<Text>().text = islandScript.buildings.Count.ToString();

        if (islandScript.isDrought)
            info.FindChild("DroughtAnswer", true).GetComponent<Text>().text = "YES";
        else
			info.FindChild("DroughtAnswer", true).GetComponent<Text>().text = "NO";

        if (islandScript.isAnimalPlague)
            info.FindChild("AnimalAnswer", true).GetComponent<Text>().text = "YES";
        else
			info.FindChild("AnimalAnswer", true).GetComponent<Text>().text = "NO";

        if (islandScript.isFishPlague)
            info.FindChild("FishAnswer", true).GetComponent<Text>().text = "YES";
        else
			info.FindChild("FishAnswer", true).GetComponent<Text>().text = "NO";

        if (islandScript.products.sumOfFood > 10)
            info.FindChild("FoodAnswer", true).GetComponent<Text>().text = "YES";
        else
			info.FindChild("FoodAnswer", true).GetComponent<Text>().text = "NO";

	}

    private void ClearList()
    {
		var listGameObject = gameObject.FindChild("People", true).FindChild("Slots", true);

		var c = listGameObject.transform.childCount;

		for (int i = 0; i < c; i++)
		{
			Destroy(listGameObject.transform.GetChild(i).gameObject);
		}
	}

    private void SetPeople()
    {
        ClearList();

		GameObject listGameObject = gameObject.FindChild("People", true).FindChild("Slots", true);
        List<Person> people = gameState.currentIsland.GetComponent<IslandScript>().people;

		foreach (var person in people)
		{
			var listItem = Instantiate(personListItem);
			listItem.transform.SetParent(listGameObject.transform);

			listItem.GetComponent<PersonListItemIsland>().SetData(person);
		}
	}

    public void ActivateMenu(string menuName)
    {
        DeactivateMenu();

        if (menuName.Equals("Resources"))
            resources.SetActive(true);
        else if (menuName.Equals("Goods"))
            goods.SetActive(true);
        else if (menuName.Equals("Info"))
            info.SetActive(true);
        else if (menuName.Equals("People"))
            people.SetActive(true);
    }

    private void DeactivateMenu()
    {
        resources.SetActive(false);
        goods.SetActive(false);
        info.SetActive(false);
        people.SetActive(false);
    }

    private void OnEnable()
    {
        GameState.OnIslandChanged += IslandChanged;
        IslandScript.OnIslandUpdate += IslandChanged;
        ShipInterfaceController.OnProductsChanged += IslandChanged;
        TradeListItem.OnListChanged += IslandChanged;

        if (gameState.currentIsland != null)
            Invoke(nameof(IslandChanged), 0.01f);
    }

    private void OnDisable()
    {
        GameState.OnIslandChanged -= IslandChanged;
        IslandScript.OnIslandUpdate -= IslandChanged;
        ShipInterfaceController.OnProductsChanged -= IslandChanged;
        TradeListItem.OnListChanged -= IslandChanged;
    }
}
