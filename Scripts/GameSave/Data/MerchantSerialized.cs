using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class MerchantSerialized
{
    public Products productsToSell;
    public Products productsToBuy;
    public List<PersonSerialized> people;

    public Queue<Harbor> destinations;
    public Vector3 destinationPos;
    public bool lastDestination;

    public Vector3 position;
}

