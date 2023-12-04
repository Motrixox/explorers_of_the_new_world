using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class IslandSerialized
{
    public string islandName;
    public int productCapacity;
    public int transportCapacity;

    public Products products;
    public List<BuildingSerialized> buildings;
    public List<ShipyardSerialized> shipyards;
    public List<PersonSerialized> people;

    public bool isDrought;
    public int droughtWeeksLeft;
    public bool isAnimalPlague;
    public int animalPlagueWeeksLeft;
    public bool isFishPlague;
    public int fishPlagueWeeksLeft;
}

