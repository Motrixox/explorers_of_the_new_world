using DevionGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandInformation : MonoBehaviour
{
    public GameObject goodListItem;

    private GameState gameState;

    private GameObject resources;
    private GameObject goods;
    private GameObject info;
    private GameObject title;

    // Start is called before the first frame update
    void Awake()
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        resources = gameObject.FindChild("Top", true).FindChild("Resources", true);
        goods = gameObject.FindChild("Top", true).FindChild("Goods", true);
        info = gameObject.FindChild("Top", true).FindChild("Info", true);
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

    public void ActivateMenu(string menuName)
    {
        DeactivateMenu();

        if (menuName.Equals("Resources"))
            resources.SetActive(true);
        else if (menuName.Equals("Goods"))
            goods.SetActive(true);
        else if (menuName.Equals("Info"))
            info.SetActive(true);
    }

    private void DeactivateMenu()
    {
        resources.SetActive(false);
        goods.SetActive(false);
        info.SetActive(false);
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
