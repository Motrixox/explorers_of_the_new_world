using DevionGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoodListItem : MonoBehaviour
{
    private string _name = string.Empty;
    private GameObject image;
    private Text nameText;
    private Text amountText;

    // Start is called before the first frame update
    void Awake()
    {
        image = gameObject.FindChild("Image", true);
        nameText = gameObject.FindChild("Name", true).GetComponent<Text>();
        amountText = gameObject.FindChild("Amount", true).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetData(string name, int amount)
    {
        _name = name;
        nameText.text = name;
        amountText.text = amount.ToString();

        image.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("icons/" + name);
    }
}
