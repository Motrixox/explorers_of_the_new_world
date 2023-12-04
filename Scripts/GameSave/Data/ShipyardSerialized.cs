using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class ShipyardSerialized : BuildingSerialized
{
    public int shipWood = 0;
    public int shipStone = 0;
    public int shipIron = 0;
    public int shipCotton = 0;
    public int shipGold = 0;
    public int shipProgress = 0;
    public bool shipInProgress = false;
}
