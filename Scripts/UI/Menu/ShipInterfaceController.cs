using Assets.Scripts.Interfaces;
using DevionGames;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ShipInterfaceController : MonoBehaviour
{
    public GameObject personListItem;
    public GameObject loadListItem;

    private GameState gameState;
    private Ship activeShip;
    private IslandScript currentIsland;
    private PlayerActionController playerActionController;

    private GameObject employees;
    private GameObject load;
    private GameObject passengers;
    private GameObject addPerson;
    private GameObject info;
    private GameObject title;

    private string currentMenu = "Load";
    private static AlertBoxScript alert;

    public delegate void ProductsChanged();
    public static event ProductsChanged OnProductsChanged;

    // Start is called before the first frame update
    void Start()
    {
        playerActionController = GameObject.FindWithTag("MainCamera").GetComponent<PlayerActionController>();
        alert = GameObject.Find("Canvas").FindChild("AlertBox", true).GetComponent<AlertBoxScript>();
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        employees = gameObject.FindChild("Top", true).FindChild("Employees", true);
        load = gameObject.FindChild("Top", true).FindChild("Load", true);
        passengers = gameObject.FindChild("Top", true).FindChild("Passengers", true);
        addPerson = gameObject.FindChild("Top", true).FindChild("AddPerson", true);
        info = gameObject.FindChild("Top", true).FindChild("Info", true);
        title = gameObject.FindChild("Title", true);

        ActivateMenu(currentMenu);
    }

    // Update is called once per frame
    void Update()
    {
        // this doesn't need to be checked every frame
        // consider checking it in intervals for better performance
        CheckCurrentIsland();
    }

    private void OnEnable()
    {
        PersonListItemShip.OnListChanged += IslandChanged;
        PersonListItemShip.OnManagerAdded += ManagerAdded;

        Start();
        FindActiveShip();
        SetLoad();
        CheckCurrentIsland();
        ClearLists();
        ListPeople();
        UpdateInfo();
    }

    private void OnDisable()
    {
        PersonListItemShip.OnListChanged -= IslandChanged;
        PersonListItemShip.OnManagerAdded -= ManagerAdded;
        ClearLists();
    }

    private void SetLoad()
    {
        var products = activeShip.products;
        var parent = load.FindChild("Slots", true);

        var c = parent.transform.childCount;

        for (int i = 0; i < c; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }

        foreach (var key in Products.keys)
        {
            var listItem = Instantiate(loadListItem);
            listItem.transform.SetParent(parent.transform);
            listItem.GetComponent<LoadListItem>().SetData(key, products.GetQuantity(key), this);
        }

        load.FindChild("Middle", true).GetComponent<Text>().text = activeShip.products.sumOfProducts + "/" + activeShip.productCapacity;

        OnProductsChanged?.Invoke();
    }

    private void UpdateInfo()
    {
        //if (activeShip.manager == null)
        //{
        //    info.FindChild("ManagerCount", true).GetComponent<Text>().color = Color.red;
        //    info.FindChild("ManagerCount", true).GetComponent<Text>().text = "0/1";
        //    employees.FindChild("Manager", true).FindChild("Amount", true).GetComponent<Text>().color = Color.red;
        //    employees.FindChild("Manager", true).FindChild("Amount", true).GetComponent<Text>().text = "0/1";
        //}
        //else
        //{
        //    info.FindChild("ManagerCount", true).GetComponent<Text>().color = Color.green;
        //    info.FindChild("ManagerCount", true).GetComponent<Text>().text = "1/1";
        //    employees.FindChild("Manager", true).FindChild("Amount", true).GetComponent<Text>().color = Color.green;
        //    employees.FindChild("Manager", true).FindChild("Amount", true).GetComponent<Text>().text = "1/1";
        //}

        title.GetComponent<Text>().text = activeShip.shipName;

        info.FindChild("EmployeesCount", true).GetComponent<Text>().text = activeShip.employees.Count + "/" + activeShip.employeeCapacity;
        employees.FindChild("EmployeesList", true).FindChild("Amount", true).GetComponent<Text>().text = activeShip.employees.Count + "/" + activeShip.employeeCapacity;

        if(activeShip.employees.Count * 1.0 / activeShip.employeeCapacity >= 0.5)
        {
            info.FindChild("EmployeesCount", true).GetComponent<Text>().color = Color.green;
            employees.FindChild("EmployeesList", true).FindChild("Amount", true).GetComponent<Text>().color = Color.green;
        }
        else
        {
            info.FindChild("EmployeesCount", true).GetComponent<Text>().color = Color.red;
            employees.FindChild("EmployeesList", true).FindChild("Amount", true).GetComponent<Text>().color = Color.red;
        }

        info.FindChild("PassengersCount", true).GetComponent<Text>().text = activeShip.passengers.Count + "/" + activeShip.passengersCapacity;
        passengers.FindChild("Amount", true).GetComponent<Text>().text = activeShip.passengers.Count + "/" + activeShip.passengersCapacity;


        info.FindChild("ProductsCount", true).GetComponent<Text>().text = activeShip.products.sumOfProducts + "/" + activeShip.productCapacity;
        info.FindChild("ProductivityAmount", true).GetComponent<Text>().text = activeShip.productivity + "%";
        info.FindChild("SpeedAmount", true).GetComponent<Text>().text = activeShip.speed + "/" + activeShip.maxSpeed;

    }

    public void FindActiveShip()
    {
        foreach (var ship in gameState.shipList)
        {
            if(ship.shipGameObject.GetComponent<ShipController>().active == true)
            {
                activeShip = ship;
            }
        }
    }

    public void BuildHarbor()
    {
        playerActionController.SetStateBuildHarborFromShip(activeShip);
    }

    public void ActivateMenu(string menuName)
    {
        DeactivateMenu();

        currentMenu = menuName;

        if (menuName.Equals("Employees"))
            employees.SetActive(true);
        else if (menuName.Equals("Load"))
            load.SetActive(true);
        else if (menuName.Equals("Passengers"))
            passengers.SetActive(true);
        else if (menuName.Equals("AddPassenger"))
            addPerson.SetActive(true);
        else if (menuName.Equals("AddManager"))
            addPerson.SetActive(true);
        else if (menuName.Equals("AddEmployee"))
            addPerson.SetActive(true);
        else if (menuName.Equals("Info"))
            info.SetActive(true);

        ClearLists();
        ListPeople();
    }

    private void DeactivateMenu()
    {
        employees.SetActive(false);
        load.SetActive(false);
        passengers.SetActive(false);
        addPerson.SetActive(false);
        info.SetActive(false);
    }

    public void AddLoad(string name)
    {
        CheckCurrentIsland();
        if(!activeShip.CheckIsCloseToHarbor(currentIsland))
        {
            alert.Alert("Ship is not close enough to the harbor!");
            return;
        }

        activeShip.AddProduct(name, currentIsland);
        SetLoad();
    }

    public void RemoveLoad(string name)
    {
        CheckCurrentIsland();
        if (!activeShip.CheckIsCloseToHarbor(currentIsland))
        {
            alert.Alert("Ship is not close enough to the harbor!");
            return;
        }

        activeShip.ReleaseProduct(name, currentIsland);
        SetLoad();
    }

    private void ManagerAdded()
    {
        ActivateMenu("Employees");
        UpdateInfo();
    }

    public void ListPeople()
    {
        GameObject listGameObject;
        List<Person> people;

        if (currentMenu.Equals("Passengers"))
        {
            listGameObject = gameObject.FindChild("Passengers", true).FindChild("Slots", true);
            people = activeShip.passengers;
        }
        else if (currentMenu.Equals("AddPassenger"))
        {
            listGameObject = gameObject.FindChild("AddPerson", true).FindChild("Slots", true);
            people = currentIsland.GetUnemployed();
        }
        else if (currentMenu.Equals("AddManager"))
        {
            listGameObject = gameObject.FindChild("AddPerson", true).FindChild("Slots", true);
            people = activeShip.GetUnemployed();
        }
        else if (currentMenu.Equals("AddEmployee"))
        {
            listGameObject = gameObject.FindChild("AddPerson", true).FindChild("Slots", true);
            people = activeShip.GetUnemployed();
        }
        else if (currentMenu.Equals("Employees"))
        {
            //if(activeShip.manager != null)
            //{
            //    gameObject.FindChild("Employees", true).FindChild("Manager", true).FindChild("Scroll View", true).SetActive(true);
            //    gameObject.FindChild("Employees", true).FindChild("Manager", true).FindChild("AddManager", true).SetActive(false);

            //    var manager = gameObject.FindChild("Employees", true).FindChild("Manager", true).FindChild("Slots", true);
            //    var listItem = Instantiate(personListItem);
            //    listItem.transform.SetParent(manager.transform);
            //    listItem.GetComponent<PersonListItemShip>().SetData(activeShip.manager, activeShip, currentIsland, currentMenu);
            //}
            //else
            //{
            //    gameObject.FindChild("Employees", true).FindChild("Manager", true).FindChild("Scroll View", true).SetActive(false);
            //    gameObject.FindChild("Employees", true).FindChild("Manager", true).FindChild("AddManager", true).SetActive(true);
            //}

            listGameObject = gameObject.FindChild("Employees", true).FindChild("EmployeesList", true).FindChild("Slots", true);
            people = activeShip.employees;
        }
        else return;

        foreach (var person in people)
        {
            var listItem = Instantiate(personListItem);
            listItem.transform.SetParent(listGameObject.transform);

            listItem.GetComponent<PersonListItemShip>().SetData(person, activeShip, currentIsland, currentMenu);
        }
    }

    private void ClearLists()
    {
        var listGameObject = gameObject.FindChild("AddPerson", true).FindChild("Slots", true);

        var c = listGameObject.transform.childCount;

        for (int i = 0; i < c; i++)
        {
            Destroy(listGameObject.transform.GetChild(i).gameObject);
        }

        listGameObject = gameObject.FindChild("Passengers", true).FindChild("Slots", true);

        c = listGameObject.transform.childCount;

        for (int i = 0; i < c; i++)
        {
            if (listGameObject.transform.GetChild(i).name.Equals("AddPassenger")) continue;

            Destroy(listGameObject.transform.GetChild(i).gameObject);
        }

        //listGameObject = gameObject.FindChild("Employees", true).FindChild("Manager", true).FindChild("Slots", true);

        //c = listGameObject.transform.childCount;

        //for (int i = 0; i < c; i++)
        //{
        //    Destroy(listGameObject.transform.GetChild(i).gameObject);
        //}

        listGameObject = gameObject.FindChild("Employees", true).FindChild("EmployeesList", true).FindChild("Slots", true);

        c = listGameObject.transform.childCount;

        for (int i = 0; i < c; i++)
        {
            if (listGameObject.transform.GetChild(i).name.Equals("AddEmployee")) continue;

            Destroy(listGameObject.transform.GetChild(i).gameObject);
        }
    }

    private void IslandChanged()
    {
        ClearLists();
        ListPeople();
        UpdateInfo();
    }

    private void CheckCurrentIsland()
    {
        foreach (var island in gameState.islands)
        {
            if (activeShip.shipGameObject.transform.position.x >= island.transform.position.x &&
                activeShip.shipGameObject.transform.position.x < island.transform.position.x + 100 &&
                activeShip.shipGameObject.transform.position.z >= island.transform.position.z &&
                activeShip.shipGameObject.transform.position.z < island.transform.position.z + 100)
            {
                if (currentIsland == island.GetComponent<IslandScript>())
                {
                    break;
                }

                currentIsland = island.GetComponent<IslandScript>();
                IslandChanged();

                break;
            }
        }
    }
}
