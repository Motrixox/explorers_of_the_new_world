using Assets.Scripts.Interfaces;
using DevionGames;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static GameState;

public class BuildingInterface : MonoBehaviour
{
    public GameObject personListItem;
    public GameObject professionListItem;
    public Building building;

    private GameObject employees;
    private GameObject addPerson;
    private GameObject info;
    private GameObject infoProduction;
    private GameObject infoProcessing;
    private GameObject infoService;
    private GameObject infoHouse;
    private GameObject residents;
    private GameObject students;
    private GameObject professions;
    private GameObject buildShip;
    private GameObject production;
    private GameObject house;
    private GameObject shipyard;
    private GameObject school;

    private string currentMenu = "Info";

    // Start is called before the first frame update
    void Start()
    {
        employees = gameObject.FindChild("Top", true).FindChild("Employees", true);
        addPerson = gameObject.FindChild("Top", true).FindChild("AddPerson", true);
        residents = gameObject.FindChild("Top", true).FindChild("Residents", true);
        students = gameObject.FindChild("Top", true).FindChild("Students", true);
        professions = gameObject.FindChild("Top", true).FindChild("Professions", true);
        infoProduction = gameObject.FindChild("Top", true).FindChild("InfoProduction", true);
        infoProcessing = gameObject.FindChild("Top", true).FindChild("InfoProcessing", true);
        infoService = gameObject.FindChild("Top", true).FindChild("InfoService", true);
        infoHouse = gameObject.FindChild("Top", true).FindChild("InfoHouse", true);
        buildShip = gameObject.FindChild("Top", true).FindChild("BuildShip", true);
        production = gameObject.FindChild("Bottom", true).FindChild("Production", true);
        house = gameObject.FindChild("Bottom", true).FindChild("House", true);
        shipyard = gameObject.FindChild("Bottom", true).FindChild("Shipyard", true);
        school = gameObject.FindChild("Bottom", true).FindChild("School", true);

        SetInfo();
        ActivateMenu(currentMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        PersonListItemBuilding.OnListChanged += UpdateInfo;
        PersonListItemBuilding.OnManagerAdded += ManagerAdded;
        GameState.OnIslandChanged += UpdateInfo;

        Start();
        ClearLists();
        ListPeople();
        UpdateInfo();
    }

    private void OnDisable()
    {
        PersonListItemBuilding.OnListChanged -= UpdateInfo;
        PersonListItemBuilding.OnManagerAdded -= ManagerAdded;
        GameState.OnIslandChanged -= UpdateInfo;
        ClearLists();
    }

    private void SetInfo()
    {
        if (building.GetType().IsSubclassOf(typeof(ProductionBuilding)))
            info = infoProduction;
        else if (building.GetType().IsSubclassOf(typeof(ProcessingBuilding)))
            info = infoProcessing;
        else if (building.GetType().IsSubclassOf(typeof(ServiceBuilding)) || building.GetType().Equals(typeof(Harbor)))
            info = infoService;
        else if (building.GetType().Equals(typeof(House)))
            info = infoHouse;
    }

    public void SetBuilding(Building b)
    {
        building = b;
        currentMenu = "Info";

        Start();

        if(building.GetType().Equals(typeof(Shipyard)))
            ActivateBottomMenu("Shipyard");
        else if(building.GetType().Equals(typeof(School)))
            ActivateBottomMenu("School");
        else if (building.GetType().IsSubclassOf(typeof(ProductionBuilding)) || building.GetType().IsSubclassOf(typeof(ProcessingBuilding)) || building.GetType().IsSubclassOf(typeof(ServiceBuilding)))
            ActivateBottomMenu("Production");
        else if (building.GetType().Equals(typeof(Harbor)))
            ActivateBottomMenu("Production");
        else if (building.GetType().Equals(typeof(House)))
            ActivateBottomMenu("House");
    }

    public void ActivateMenu(string menuName)
    {
        DeactivateMenu();

        currentMenu = menuName;

        if (menuName.Equals("Employees"))
            employees.SetActive(true);
        else if (menuName.Equals("AddPassenger"))
            addPerson.SetActive(true);
        else if (menuName.Equals("AddManager"))
            addPerson.SetActive(true);
        else if (menuName.Equals("AddEmployee"))
            addPerson.SetActive(true);
        else if (menuName.Equals("Residents"))
            residents.SetActive(true);
        else if (menuName.Equals("Students"))
            students.SetActive(true);
        else if (menuName.Equals("AddResident"))
            addPerson.SetActive(true);
        else if (menuName.Equals("BuildShip"))
            buildShip.SetActive(true);
        else if (menuName.Equals("AddStudent"))
            addPerson.SetActive(true);
        else if (menuName.Equals("Professions"))
            professions.SetActive(true);
        else if (menuName.Equals("Info"))
        {
            info.SetActive(true);
        }

        ClearLists();
        ListPeople();
    }

    public void ActivateBottomMenu(string menuName)
    {
        DeactivateBottomMenu();

        if (menuName.Equals("Production"))
            production.SetActive(true);
        else if (menuName.Equals("House"))
            house.SetActive(true);
        else if (menuName.Equals("Shipyard"))
            shipyard.SetActive(true);
        else if (menuName.Equals("School"))
            school.SetActive(true);
    }

    private void DeactivateMenu()
    {
        employees.SetActive(false);
        addPerson.SetActive(false);
        infoProduction.SetActive(false);
        infoProcessing.SetActive(false);
        infoService.SetActive(false);
        infoHouse.SetActive(false);
        residents.SetActive(false);
        students.SetActive(false);
        buildShip.SetActive(false);
        professions.SetActive(false);
    }

    private void DeactivateBottomMenu()
    {
        production.SetActive(false);
        house.SetActive(false);
        shipyard.SetActive(false);
        school.SetActive(false);
    }

    public void ListPeople()
    {
        if (building is IEmployees)
        {
            ListEmployees();
        }
        else if (building.GetType().Equals(typeof(House)))
        {
            ListResidents();
        }

        if (building.GetType().Equals(typeof(School)))
        {
            ListStudents();
        }
    }

    private void ListStudents()
    {
        GameObject listGameObject;
        List<Person> people;

        School schoolBuilding = (School)building;

        if (currentMenu.Equals("AddStudent"))
        {
            listGameObject = gameObject.FindChild("AddPerson", true).FindChild("Slots", true);
            people = building.island.GetPotentialStudents();
        }
        else if (currentMenu.Equals("Students"))
        {
            listGameObject = gameObject.FindChild("Top", true).FindChild("Students", true).FindChild("StudentsList", true).FindChild("Slots", true);
            people = schoolBuilding.students.Keys.ToList();
        }
        else return;

        foreach (var person in people)
        {
            var listItem = Instantiate(personListItem);
            listItem.transform.SetParent(listGameObject.transform);

            listItem.GetComponent<PersonListItemBuilding>().SetData(person, schoolBuilding, schoolBuilding.island, currentMenu);
            listItem.GetComponent<PersonListItemBuilding>().buildingInterface = this;
        }
    }

    public void ListProfessions(Person p)
    {
        GameObject listGameObject;
        List<string> professions;

        School schoolBuilding = (School)building;

        listGameObject = gameObject.FindChild("Top", true).FindChild("Professions", true).FindChild("ProfessionsList", true).FindChild("Slots", true);
        professions = School.professions.Keys.ToList();
        
        var c = listGameObject.transform.childCount;

        for (int i = 0; i < c; i++)
        {
            Destroy(listGameObject.transform.GetChild(i).gameObject);
        }

        foreach (var profession in professions)
        {
            if (p.learnedProfessions.Contains(profession))
                continue;

            var listItem = Instantiate(professionListItem);
            listItem.transform.SetParent(listGameObject.transform);

            listItem.GetComponent<ProfessionListItem>().SetData(p, schoolBuilding, profession, this);
        }        
    }

    private void ListResidents()
    {
        GameObject listGameObject;
        List<Person> people;

        House houseBuilding = (House)building;

        if (currentMenu.Equals("AddResident"))
        {
            listGameObject = gameObject.FindChild("AddPerson", true).FindChild("Slots", true);
            people = building.island.GetHomeless();
        }
        else if (currentMenu.Equals("Residents"))
        {
            listGameObject = gameObject.FindChild("Top", true).FindChild("Residents", true).FindChild("ResidentsList", true).FindChild("Slots", true);
            people = houseBuilding.residents;
        }
        else return;

        foreach (var person in people)
        {
            var listItem = Instantiate(personListItem);
            listItem.transform.SetParent(listGameObject.transform);

            listItem.GetComponent<PersonListItemBuilding>().SetData(person, houseBuilding, houseBuilding.island, currentMenu);
        }
    }

    private void ListEmployees()
    {
        GameObject listGameObject;
        List<Person> people;

        IEmployees employeesBuilding = (IEmployees)building;

        if (currentMenu.Equals("AddManager"))
        {
            listGameObject = gameObject.FindChild("AddPerson", true).FindChild("Slots", true);
            people = building.island.GetUnemployed();
        }
        else if (currentMenu.Equals("AddEmployee"))
        {
            listGameObject = gameObject.FindChild("AddPerson", true).FindChild("Slots", true);
            people = building.island.GetUnemployed();
        }
        else if (currentMenu.Equals("Employees"))
        {
            //if (employeesBuilding.manager != null)
            //{
            //    gameObject.FindChild("Top", true).FindChild("Employees", true).FindChild("Manager", true).FindChild("Scroll View", true).SetActive(true);
            //    gameObject.FindChild("Top", true).FindChild("Employees", true).FindChild("Manager", true).FindChild("AddManager", true).SetActive(false);

            //    var manager = gameObject.FindChild("Top", true).FindChild("Employees", true).FindChild("Manager", true).FindChild("Slots", true);
            //    var listItem = Instantiate(personListItem);
            //    listItem.transform.SetParent(manager.transform);
            //    listItem.GetComponent<PersonListItemBuilding>().SetData(employeesBuilding.manager, building, building.island, currentMenu);
            //}
            //else
            //{
            //    gameObject.FindChild("Top", true).FindChild("Employees", true).FindChild("Manager", true).FindChild("Scroll View", true).SetActive(false);
            //    gameObject.FindChild("Top", true).FindChild("Employees", true).FindChild("Manager", true).FindChild("AddManager", true).SetActive(true);
            //}

            listGameObject = gameObject.FindChild("Top", true).FindChild("Employees", true).FindChild("EmployeesList", true).FindChild("Slots", true);
            people = employeesBuilding.employees;
        }
        else return;

        foreach (var person in people)
        {
            var listItem = Instantiate(personListItem);
            listItem.transform.SetParent(listGameObject.transform);

            listItem.GetComponent<PersonListItemBuilding>().SetData(person, building, building.island, currentMenu);
        }
    }

    private void ClearLists()
    {
        var listGameObject = gameObject.FindChild("Top", true).FindChild("AddPerson", true).FindChild("Slots", true);

        var c = listGameObject.transform.childCount;

        for (int i = 0; i < c; i++)
        {
            Destroy(listGameObject.transform.GetChild(i).gameObject);
        }

        //listGameObject = gameObject.FindChild("Top", true).FindChild("Employees", true).FindChild("Manager", true).FindChild("Slots", true);

        //c = listGameObject.transform.childCount;

        //for (int i = 0; i < c; i++)
        //{
        //    Destroy(listGameObject.transform.GetChild(i).gameObject);
        //}

        listGameObject = gameObject.FindChild("Top", true).FindChild("Employees", true).FindChild("EmployeesList", true).FindChild("Slots", true);

        c = listGameObject.transform.childCount;
        
        for (int i = 0; i < c; i++)
        {
            if (listGameObject.transform.GetChild(i).name.Equals("AddEmployee")) continue;

            Destroy(listGameObject.transform.GetChild(i).gameObject);
        }

        listGameObject = gameObject.FindChild("Top", true).FindChild("Residents", true).FindChild("ResidentsList", true).FindChild("Slots", true);
        
        c = listGameObject.transform.childCount;

        for (int i = 0; i < c; i++)
        {
            if (listGameObject.transform.GetChild(i).name.Equals("AddResident")) continue;

            Destroy(listGameObject.transform.GetChild(i).gameObject);
        }

        listGameObject = gameObject.FindChild("Top", true).FindChild("Students", true).FindChild("StudentsList", true).FindChild("Slots", true);
        
        c = listGameObject.transform.childCount;

        for (int i = 0; i < c; i++)
        {
            if (listGameObject.transform.GetChild(i).name.Equals("AddStudent")) continue;

            Destroy(listGameObject.transform.GetChild(i).gameObject);
        }
    }
    private void ManagerAdded()
    {
        UpdateInfo();
        ActivateMenu("Employees");
    }

    private void UpdateInfo()
    {
        ClearLists();
        ListPeople();

        gameObject.FindChild("Title", true).GetComponent<Text>().text = ((IBuildingInfo)building).GetBuildingInfo().buildingName;

        if(building is IEmployees)
        {
            UpdateEmployees();
        }

        if(building is IWorkingBuilding)
        {
            ((IWorkingBuilding)building).CheckIsWorking();
            UpdateWorkingStatus();
        }

        if (building.GetType().IsSubclassOf(typeof(ProductionBuilding))) 
        {
            UpdateInfoInProductionBuilding();
        }
        else if (building.GetType().IsSubclassOf(typeof(ProcessingBuilding)))
        {
            UpdateInfoInProcessingBuilding();
        }
        else if (building.GetType().IsSubclassOf(typeof(ServiceBuilding)))
        {
            UpdateInfoInServiceBuilding();
            if (building.GetType().Equals(typeof(Shipyard)))
                UpdateBuildShip();
        }
        else if (building.GetType().Equals(typeof(House)))
        {
            UpdateInfoInHouse();
        }
    }

    private void UpdateEmployees()
    {
        IEmployees employeesBuilding = (IEmployees)building;

        //if (employeesBuilding.manager == null)
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

        info.FindChild("EmployeesCount", true).GetComponent<Text>().text = employeesBuilding.employees.Count + "/" + employeesBuilding.employeeCapacity;
        employees.FindChild("EmployeesList", true).FindChild("Amount", true).GetComponent<Text>().text = employeesBuilding.employees.Count + "/" + employeesBuilding.employeeCapacity;

        if (employeesBuilding.employees.Count * 1.0 / employeesBuilding.employeeCapacity >= 0.5)
        {
            info.FindChild("EmployeesCount", true).GetComponent<Text>().color = Color.green;
            employees.FindChild("EmployeesList", true).FindChild("Amount", true).GetComponent<Text>().color = Color.green;
        }
        else
        {
            info.FindChild("EmployeesCount", true).GetComponent<Text>().color = Color.red;
            employees.FindChild("EmployeesList", true).FindChild("Amount", true).GetComponent<Text>().color = Color.red;
        }
    }

    private void UpdateInfoInProcessingBuilding()
    {
        ProcessingBuilding processingBuilding = (ProcessingBuilding)building;

        processingBuilding.Calculate();
        info.FindChild("ProductivityAmount", true).GetComponent<Text>().text = processingBuilding.productivity + "%";
        info.FindChild("ProductionAmount", true).GetComponent<Text>().text = processingBuilding.production + "/" + processingBuilding.maxProduction;
    }

    private void UpdateInfoInProductionBuilding()
    {
        ProductionBuilding productionBuilding = (ProductionBuilding)building;

        productionBuilding.Calculate();
        info.FindChild("FieldsAmount", true).GetComponent<Text>().text = productionBuilding.activeFields + "/24";
        info.FindChild("ProductivityAmount", true).GetComponent<Text>().text = productionBuilding.productivity + "%";
        info.FindChild("ProductionAmount", true).GetComponent<Text>().text = productionBuilding.production + "/" + productionBuilding.maxProduction;
    }

    private void UpdateInfoInHouse()
    {
        House houseBuilding = (House)building;
        houseBuilding.CheckIsWorking();

        info.FindChild("ResidentsCount", true).GetComponent<Text>().text = houseBuilding.residents.Count + "/" + houseBuilding.peopleCapacity;
        residents.FindChild("ResidentsList", true).FindChild("Amount", true).GetComponent<Text>().text = houseBuilding.residents.Count + "/" + houseBuilding.peopleCapacity;

        if (houseBuilding.residents.Count == houseBuilding.peopleCapacity)
        {
            info.FindChild("ResidentsCount", true).GetComponent<Text>().color = Color.green;
            residents.FindChild("ResidentsList", true).FindChild("Amount", true).GetComponent<Text>().color = Color.green;
        }
        else
        {
            info.FindChild("ResidentsCount", true).GetComponent<Text>().color = Color.red;
            residents.FindChild("ResidentsList", true).FindChild("Amount", true).GetComponent<Text>().color = Color.red;
        }
    }

    private void UpdateWorkingStatus()
    {
        if (((IWorkingBuilding)building).isWorking)
        {
            info.FindChild("ActiveAnswer", true).GetComponent<Text>().text = "Yes";
            info.FindChild("ActiveAnswer", true).GetComponent<Text>().color = Color.green;
        }
        else
        {
            info.FindChild("ActiveAnswer", true).GetComponent<Text>().text = "No";
            info.FindChild("ActiveAnswer", true).GetComponent<Text>().color = Color.red;
        }
    }

    private void UpdateInfoInServiceBuilding()
    {
        ServiceBuilding serviceBuilding = (ServiceBuilding)building;

        DeactivateServiceInfo();

        if (serviceBuilding.GetType().Equals(typeof(Hospital)))
        {
            Hospital hospital = (Hospital)serviceBuilding;

            info.FindChild("Herbs", true).SetActive(true);
            info.FindChild("HerbsAnswer", true).SetActive(true);

            if(hospital.island.products.GetQuantity("herb") > 0)
            {
                info.FindChild("HerbsAnswer", true).GetComponent<Text>().text = "Yes";
                info.FindChild("ActiveAnswer", true).GetComponent<Text>().color = Color.green;
            }
            else
            {
                info.FindChild("HerbsAnswer", true).GetComponent<Text>().text = "No";
                info.FindChild("ActiveAnswer", true).GetComponent<Text>().color = Color.red;
            }
        }
        else if (serviceBuilding.GetType().Equals(typeof(Warehouse)))
        {
            Warehouse warehouse = (Warehouse)serviceBuilding;

            info.FindChild("Capacity", true).SetActive(true);
            info.FindChild("CapacityAnswer", true).SetActive(true);
            info.FindChild("Transport", true).SetActive(true);
            info.FindChild("TransportAnswer", true).SetActive(true);

            info.FindChild("CapacityAnswer", true).GetComponent<Text>().text = warehouse.productCapacity.ToString();
            info.FindChild("TransportAnswer", true).GetComponent<Text>().text = warehouse.transportCapacity.ToString();
        }
        else if (serviceBuilding.GetType().Equals(typeof(Harbor)))
        {
            Harbor harbor = (Harbor)serviceBuilding;

            info.FindChild("Capacity", true).SetActive(true);
            info.FindChild("CapacityAnswer", true).SetActive(true);
            info.FindChild("Discount", true).SetActive(true);
            info.FindChild("DiscountAnswer", true).SetActive(true);

            info.FindChild("CapacityAnswer", true).GetComponent<Text>().text = harbor.productCapacity.ToString();
            info.FindChild("DiscountAnswer", true).GetComponent<Text>().text = harbor.tradeDiscount.ToString();
        }
    }

    private void DeactivateServiceInfo()
    {
        info.FindChild("Herbs", true).SetActive(false);
        info.FindChild("HerbsAnswer", true).SetActive(false);
        info.FindChild("Capacity", true).SetActive(false);
        info.FindChild("CapacityAnswer", true).SetActive(false);
        info.FindChild("Transport", true).SetActive(false);
        info.FindChild("TransportAnswer", true).SetActive(false);
        info.FindChild("Discount", true).SetActive(false);
        info.FindChild("DiscountAnswer", true).SetActive(false);
    }

    private void UpdateBuildShip()
    {
        Shipyard shipyard = (Shipyard)building;

        if (shipyard.shipInProgress)
            buildShip.FindChild("Status", true).FindChild("Amount", true).GetComponent<Text>().text = "in progress";
        else
            buildShip.FindChild("Status", true).FindChild("Amount", true).GetComponent<Text>().text = "stopped";

        buildShip.FindChild("Progress", true).FindChild("Amount", true).GetComponent<Text>().text = (shipyard.shipProgress / 100.0) + "%";
        buildShip.FindChild("WoodLeft", true).FindChild("Amount", true).GetComponent<Text>().text = (Shipyard.shipCostWood - shipyard.shipWood).ToString();
        buildShip.FindChild("StoneLeft", true).FindChild("Amount", true).GetComponent<Text>().text = (Shipyard.shipCostStone - shipyard.shipStone).ToString();
        buildShip.FindChild("IronLeft", true).FindChild("Amount", true).GetComponent<Text>().text = (Shipyard.shipCostIron - shipyard.shipIron).ToString();
        buildShip.FindChild("CottonLeft", true).FindChild("Amount", true).GetComponent<Text>().text = (Shipyard.shipCostCotton - shipyard.shipCotton).ToString();
        buildShip.FindChild("GoldLeft", true).FindChild("Amount", true).GetComponent<Text>().text = (Shipyard.shipCostGold - shipyard.shipGold).ToString();
    }

    public void ToggleBuildShip(bool start)
    {
        Shipyard shipyard = (Shipyard)building;
        if (start)
            shipyard.shipInProgress = true;
        else
            shipyard.shipInProgress = false;

        UpdateInfo();
    }
}
