using DevionGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TradeListItem : MonoBehaviour, IPointerEnterHandler
{
    public Texture plus;
    public Texture minus;

    private RawImage image;
    private Text nameText;
    private Text amountText;
    private Text priceText;
    private Text discountPriceText;
    private RawImage buttonImage;

    private string action;
    private int price;
    private float discount;
    private int discountPrice;
    private int amount;

    private IslandScript island;
    private TradeWindow tradeWindow;
    private MerchantShip merchant;
    private static GameState gameState;
    private static AlertBoxScript alert;
    private GameObject details;

    public delegate void ListChanged();
    public static event ListChanged OnListChanged;

    // Start is called before the first frame update
    void Awake()
    {
        details = GameObject.Find("Trade Window(Clone)").FindChild("TradeDetails", true);
        image = gameObject.FindChild("Image", true).GetComponent<RawImage>();
        nameText = gameObject.FindChild("Name", true).GetComponent<Text>();
        amountText = gameObject.FindChild("Amount", true).GetComponent<Text>();
        priceText = gameObject.FindChild("Price", true).GetComponent<Text>();
        discountPriceText = gameObject.FindChild("DiscountPrice", true).GetComponent<Text>();
        buttonImage = gameObject.FindChild("Button", true).GetComponent<RawImage>();

        if(gameState == null)
            gameState = GameObject.Find("GameState").GetComponent<GameState>();

        if(alert == null)
            alert = GameObject.Find("Canvas").FindChild("AlertBox", true).GetComponent<AlertBoxScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(string action, string name, int amount, int price, float discount, IslandScript island, TradeWindow tw, MerchantShip merchant)
    {
        this.amount = amount;
        this.price = price;
        this.discountPrice = price;
        this.discount = discount;
        this.tradeWindow = tw;
        this.island = island;
        this.action = action;
        this.merchant = merchant;

        UpdateItem();

        if (action.Equals("buy"))
        {
            this.price = price * 11 / 10;
            this.discountPrice = (int)(price * (11 - discount) / 10);
            buttonImage.texture = plus;
        }
        else if (action.Equals("sell"))
        {
            this.price = price * 9 / 10;
            this.discountPrice = (int)(price * (9 + discount) / 10);
            buttonImage.texture = minus;
        }

        image.texture = Resources.Load<Texture2D>("icons/" + name);
        this.nameText.text = name;
        this.priceText.text = this.price.ToString();
        this.discountPriceText.text = discountPrice.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tradeWindow.DetailsMode(false);
        SetDetails();
    }

    private void SetDetails()
    {
        if (action.Equals("buy"))
        {
            details.FindChild("In", true).FindChild("ProductAmount", true).GetComponent<Text>().text = discountPrice.ToString();
            details.FindChild("In", true).FindChild("ProductName", true).GetComponent<Text>().text = "Gold";
            details.FindChild("Out", true).FindChild("ProductAmount", true).GetComponent<Text>().text = "100";
            details.FindChild("Out", true).FindChild("ProductName", true).GetComponent<Text>().text = nameText.text;
        }
        else if (action.Equals("sell"))
        {
            details.FindChild("In", true).FindChild("ProductAmount", true).GetComponent<Text>().text = "100";
            details.FindChild("In", true).FindChild("ProductName", true).GetComponent<Text>().text = nameText.text;
            details.FindChild("Out", true).FindChild("ProductAmount", true).GetComponent<Text>().text = discountPrice.ToString();
            details.FindChild("Out", true).FindChild("ProductName", true).GetComponent<Text>().text = "Gold";
        }

        details.FindChild("Discount", true).FindChild("Amount", true).GetComponent<Text>().text = discount.ToString();
    }

    private void UpdateItem()
    {
        amountText.text = amount.ToString();
    }

    public void Action()
    {
        if (action.Equals("buy"))
            Buy();
        else if (action.Equals("sell"))
            Sell();

        OnListChanged?.Invoke();
        UpdateItem();
    }

    private void Buy()
    {
        if(gameState.goldAmount < discountPrice)
        {
            alert.Alert("Not enough gold to buy");
            return;
        }

        if(island.products.sumOfProducts + 100 > island.productCapacity)
        {
            alert.Alert("Not enough capacity");
            return;
        }

        gameState.goldAmount -= discountPrice;
        island.products.AddQuantity(nameText.text, 100);
        merchant.productsToBuy.RemoveQuantity(nameText.text, 100);
        amount -= 100;

        if (amount <= 0)
            Destroy(gameObject);

    }

    private void Sell()
    {
        if(island.products.GetQuantity(nameText.text) < 100)
        {
            alert.Alert("Not enough products to sell");
            return;
        }

        gameState.goldAmount += discountPrice;
        island.products.RemoveQuantity(nameText.text, 100);
        merchant.productsToSell.RemoveQuantity(nameText.text, 100);
        amount -= 100;

        if (amount <= 0)
            Destroy(gameObject);
    }
}
