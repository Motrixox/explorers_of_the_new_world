using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class BuildingSerialized
{
    public int buildingID;
    public Vector3 position;
    public List<Vector2> coords;
    public List<string> peopleIDs;
}
