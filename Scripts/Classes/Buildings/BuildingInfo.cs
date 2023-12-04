using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BuildingInfo
{
    public static List<BuildingInfo> buildingInfos = new List<BuildingInfo>();

    public int buildingID { get; set; }
    public string buildingName { get; set; }

    public int costWood { get; set; }
    public int costStone { get; set; }
    public int costIron { get; set; }
    public int costGold { get; set; }

    public string profession { get; set; }

    public Texture2D buildingImage { get; set; }
    public GameObject prefab { get; set; }

    public string buildingImagePath { get; set; }
    public string prefabPath { get; set; }

    public BuildingInfo()
    {
        buildingInfos.Add(this);
    }
    public void LoadBuildingInfo()
    {
        buildingImage = Resources.Load<Texture2D>(buildingImagePath);
        prefab = Resources.Load<GameObject>(prefabPath);
    }
}

