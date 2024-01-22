using Assets.Scripts.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MerchantShip : MonoBehaviour
{
    public static readonly List<Vector3> endpoints = new List<Vector3> {
        new Vector3(250, -0.8f, 450),
        new Vector3(450, -0.8f, 250),
        new Vector3(-50, -0.8f, 250),
        new Vector3(250, -0.8f, -50),
        new Vector3(150, -0.8f, 450),
        new Vector3(450, -0.8f, 150),
        new Vector3(-50, -0.8f, 150),
        new Vector3(150, -0.8f, -50)
    };

    public GameObject tradeWindow;

    public Products productsToSell { get; private set; }
    public Products productsToBuy { get; private set; }
    public List<Person> people { get; private set; }

    private float waitAtHarborTime = 5f;

    private static GameState gameState;
    private Queue<Harbor> destinations;
    private NavMeshAgent navMeshAgent;
    private Vector3 destinationPos;
    private IslandScript currentIsland;
    private Harbor currentHarbor;
    private GameObject cam;

    private bool block = false;
    private bool lastDestination = false;

    // Start is called before the first frame update
    void Awake()
    {
        productsToBuy = new Products();
        productsToSell = new Products();
        people = new List<Person>();
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        cam = GameObject.Find("Main Camera");

        GeneratePeople();
        PickProducts();
        SaveDestinations();
    }

    // Update is called once per frame
    void Update()
    {
        if(CheckIfDestinationReached() && !block)
        {
            block = true;
            Invoke(nameof(SetNextDestination), waitAtHarborTime);

            if (lastDestination)
                return;

            InstantiateTradeWindow();
        }
    }

    private void InstantiateTradeWindow()
    {
        cam.transform.position = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
        var tradeWindow = Instantiate(this.tradeWindow);
        tradeWindow.transform.SetParent(GameObject.Find("Canvas").transform);
        tradeWindow.transform.localPosition = new Vector3(0, 0, 0);
        gameState.SetTimeScale(-1);
        CheckCurrentIsland();
        tradeWindow.GetComponent<TradeWindow>().SetItems(productsToSell, productsToBuy, people, currentIsland, this, currentHarbor);
    }

    private void GeneratePeople()
    {
        var count = gameState.random.Next(3, 9);

        for (int i = 0; i < count; i++)
        {
            people.Add(PersonFactory.CreatePerson(gameState.random.Next()));
        }
    }

    private void PickProducts()
    {
        List<string> strings = new List<string>();

        while(strings.Count < 10)
        {
            int i = gameState.random.Next(0, Products.keys.Count);

            if (strings.Contains(Products.keys[i]))
            {
                continue;
            }
            else
            {
                strings.Add(Products.keys[i]);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            var amount = gameState.random.Next(5, 50) * 100;
            productsToSell.AddQuantity(strings[i], amount);
        }

        for (int i = 5; i < 10; i++)
        {
            var amount = gameState.random.Next(5, 50) * 100;
            productsToBuy.AddQuantity(strings[i], amount);
        }
    }

    private void SaveDestinations()
    {
        var list = new List<Harbor>();

        foreach (var island in gameState.islands)
        {
            var buildings = island.GetComponent<IslandScript>().buildings;
            foreach (var building in buildings)
            {
                if (building.GetType().Equals(typeof(Harbor)))
                {
                    //var pos = building.gameObject.transform.position;
                    //pos.y = -0.8f;
                    list.Add((Harbor)building);
                }
            }
        }

        destinations = new Queue<Harbor>(list.Shuffle());

        if(destinations.Count > 0)
        {
            currentHarbor = destinations.Dequeue();
            destinationPos = currentHarbor.transform.position;
            destinationPos.y = -0.8f;
            navMeshAgent.destination = destinationPos;
        }
        else
        {
            destinationPos = endpoints.PickRandom();
            destinationPos.x += gameState.random.Next(-5, 6);
            destinationPos.z += gameState.random.Next(-5, 6);
            destinationPos.y = -0.8f;
            navMeshAgent.destination = destinationPos;
            lastDestination = true;
        }
        
    }

    private bool CheckIfDestinationReached()
    {
        if (
            transform.position.x > destinationPos.x - 5f &&
            transform.position.x < destinationPos.x + 5f &&
            transform.position.z > destinationPos.z - 5f &&
            transform.position.z < destinationPos.z + 5f)
        {
            return true;
        }

        return false;
    }

    private void SetNextDestination()
    {
        if (lastDestination)
        {
            gameState.merchantList.Remove(gameObject);
            Destroy(gameObject);
            return;
        }

        if (destinations.Count <= 0)
        {
            destinationPos = endpoints.PickRandom();
            destinationPos.x += gameState.random.Next(-5, 6);
            destinationPos.z += gameState.random.Next(-5, 6);
            destinationPos.y = -0.8f;
            navMeshAgent.destination = destinationPos;
            block = false;
            lastDestination = true;
            return;
        }

        currentHarbor = destinations.Dequeue();
        destinationPos = currentHarbor.transform.position;
        destinationPos.y = -0.8f;
        navMeshAgent.destination = destinationPos;
        block = false;
    }

    private void CheckCurrentIsland()
    {
        foreach (var island in gameState.islands)
        {
            if (gameObject.transform.position.x >= island.transform.position.x &&
                gameObject.transform.position.x < island.transform.position.x + 100 &&
                gameObject.transform.position.z >= island.transform.position.z &&
                gameObject.transform.position.z < island.transform.position.z + 100)
            {
                if (currentIsland == island.GetComponent<IslandScript>())
                {
                    break;
                }

                currentIsland = island.GetComponent<IslandScript>();

                break;
            }
        }
    }

    public MerchantSerialized GetSerialized()
    {
        var result = new MerchantSerialized
        {
            productsToBuy = productsToBuy,
            productsToSell = productsToSell,
            people = new List<PersonSerialized>(),
            destinations = destinations,
            destinationPos = destinationPos,
            lastDestination = lastDestination,
            position = gameObject.transform.position
        };

        foreach (var p in people)
        {
            result.people.Add(p.GetSerialized());
        }

        return result;
    }

    public void LoadData(MerchantSerialized ms)
    {
        productsToBuy = ms.productsToBuy;
        productsToSell = ms.productsToSell;
        destinations = ms.destinations;
        destinationPos = ms.destinationPos;
        lastDestination = ms.lastDestination;
        gameObject.transform.position = ms.position;
        people = new List<Person>();

        foreach (var p in ms.people)
        {
            people.Add(new Person(p));
        }
    }
}
