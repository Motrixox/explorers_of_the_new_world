using Assets.Scripts.Interfaces;
using DevionGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BuildingMenuController : MonoBehaviour
{
    private Texture2D destroyTexture;

    private PlayerActionController playerActionController;

    private GameObject production;
    private GameObject processing;
    private GameObject domestics;
    private GameObject special;
    private GameObject misc;
    private GameObject buildingImage;
    private GameObject buildingName;
    private GameObject buildingResources;

    // Start is called before the first frame update
    void Awake()
    {
        playerActionController = GameObject.FindWithTag("MainCamera").GetComponent<PlayerActionController>();

        production = gameObject.FindChild("Bottom", true).FindChild("Production", true);
        processing = gameObject.FindChild("Bottom", true).FindChild("Processing", true);
        domestics = gameObject.FindChild("Bottom", true).FindChild("Domestics", true);
        special = gameObject.FindChild("Bottom", true).FindChild("Special", true);
        misc = gameObject.FindChild("Bottom", true).FindChild("Misc", true);

        buildingImage = gameObject.FindChild("Top", true).FindChild("BuildingImage", true);
        buildingName = gameObject.FindChild("Top", true).FindChild("BuildingName", true);
        buildingResources = gameObject.FindChild("Top", true).FindChild("BuildingResources", true);

        destroyTexture = Resources.Load<Texture2D>("Images/Cancel");

        DeactivateBottom();

        production.SetActive(true);

        LoadBuildingInfos();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadBuildingInfos()
    {
        Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.ExportedTypes.Contains(typeof(Building))).FirstOrDefault();

        Type[] types = assembly.GetTypes();

        foreach (Type type in types)
        {
            if (typeof(IBuildingInfo).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
        }
    }


    public void ActivateMenu(string menuName)
    {
        DeactivateBottom();

        if (menuName.Equals("Production"))
            production.SetActive(true);
        else if (menuName.Equals("Processing"))
            processing.SetActive(true);
        else if (menuName.Equals("Domestics"))
            domestics.SetActive(true);
        else if (menuName.Equals("Special"))
            special.SetActive(true);
        else 
            misc.SetActive(true);
    }

    private void DeactivateBottom()
    {
        production.SetActive(false);
        processing.SetActive(false);
        domestics.SetActive(false);
        special.SetActive(false);
        misc.SetActive(false);
    }

    private void SetName(string name)
    {
        buildingName.GetComponent<Text>().text = name;
    }

    private void SetImage(Texture2D tex)
    {
        buildingImage.GetComponent<RawImage>().texture = tex;
    }

    private void SetResources(int wood, int iron, int stone, int gold)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(wood);
        sb.Append(" wood ");
        sb.Append(iron);
        sb.Append(" iron \n");
        sb.Append(stone);
        sb.Append(" stone ");
        sb.Append(gold);
        sb.Append(" gold");

        buildingResources.GetComponent<Text>().text = sb.ToString();
    }

    public void Build(int buildingID)
    {
        BuildingInfo building = null;

        foreach (var info in BuildingInfo.buildingInfos)
        {
            if(info.buildingID == buildingID)
            {
                building = info;
                break;
            }
        }

        if (building.buildingImage == null)
            building.LoadBuildingInfo();

        SetName(building.buildingName);
        SetImage(building.buildingImage);
        SetResources(building.costWood, building.costIron, building.costStone, building.costGold);

        playerActionController.SetStateBuild(buildingID);
    }

    public void Destroy()
    {
        SetName("Destroy");
        SetImage(destroyTexture);
        SetResources(0, 0, 0, 0);

        playerActionController.SetStateDestroy();
    }
}
