using Assets.Scripts.Interfaces;
using DevionGames;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameState : MonoBehaviour, IDataPersistence
{
    public new GameObject camera;

    public int[,] worldBase { get; set; }
    public int[,] worldSurface { get; set; }

    public int goldAmount { get; set; }

    public int nextWeekSeed { get; set; }
    public GameObject[] islands { get; set; }

    public GameObject currentIsland { get; set; }
    public List<Ship> shipList { get; set; }
    public List<GameObject> merchantList { get; set; }

    public delegate void IslandChanged();
    public static event IslandChanged OnIslandChanged;

    public bool isGamePaused { get; private set; } = false;

    private int week = 1;
    private float timeUntilNextWeek = 0f;
    private float timeUntilMapUpdate = 0f;
    private float timeUntilAutoSave = 0f;
    public float timeSinceStart = 0f;

    public System.Random random;
    public GameObject merchantShip;
    private MinimapController minimapController;
    public GameLog gameLog;

    // Start is called before the first frame update
    void Awake()
    {
        camera = GameObject.FindWithTag("MainCamera");
        shipList = new List<Ship>();
        merchantList = new List<GameObject>();
        minimapController = GameObject.Find("Map").GetComponent<MinimapController>();
        gameLog = GameObject.Find("Game Log").GetComponent<GameLog>();
        goldAmount = 10000;
        SetTimeScale(1);
    }

    void Start()
    {
        CheckCurrentIsland();
	}

    // Update is called once per frame
    void Update()
    {
        CheckCurrentIsland();

        ManageTime();
    }

    public void CallOnIlsandChanged()
    {
        OnIslandChanged?.Invoke();
    }

    public void SetTimeScale(int scale)
    {
        if (scale == -1)
        {
            scale = 0;
            isGamePaused = true;
        }
        else
            isGamePaused = false;

        Time.timeScale = scale;
    }

    private void ManageTime()
    {
        timeUntilNextWeek += Time.deltaTime;
        timeUntilMapUpdate += Time.deltaTime;
        timeUntilAutoSave += Time.deltaTime;

        if (timeUntilNextWeek > 60)
        {
            random = new System.Random(nextWeekSeed);
            week++;
            timeUntilNextWeek = 0f;

            gameLog.ClearLogs();

            CalculateProduction();
            Consumption();
            RandomEvents();
            SpawnMerchants();
            AddAge();
            CalculateShipSpeed();

            OnIslandChanged?.Invoke();

            nextWeekSeed = random.Next();
        }  
        
        if(timeUntilMapUpdate > 0.5f)
        {
            minimapController.UpdateMap();
            timeUntilMapUpdate = 0f;
            CheckGameLoss();
		}

        if(timeUntilAutoSave > DataPersistenceManager.instance.gameOptions.autosaveFrequencyMinutes * 60 && DataPersistenceManager.instance.gameOptions.autosaveFrequencyMinutes != 0)
        {
            DataPersistenceManager.instance.SaveGame();
            GameObject.Find("Canvas").FindChild("AlertBox", true).GetComponent<AlertBoxScript>().Alert("Game has been saved!");
            timeUntilAutoSave = 0f;
		}
    }

    private void CheckGameLoss()
    {
        int peopleCount = 0;

        foreach (var ship in shipList)
        {
            peopleCount += ship.passengers.Count;
            peopleCount += ship.employees.Count;
        }

        foreach (var island in islands)
        {
            var i = island.GetComponent<IslandScript>();
			peopleCount += i.people.Count;
        }

        if(peopleCount < 5)
        {
            GameLost();
        }
    }

    private void GameLost()
    {
        GameObject.Find("Canvas").FindChild("GameLost", true).SetActive(true);
        SetTimeScale(-1);
    }

    public void LoadGameMenu()
    {
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadScene("MainMenu");
    }

    private void CalculateShipSpeed()
    {
        foreach (var ship in shipList)
        {
            ship.Calculate();
        }
    }

    private void Consumption()
    {
        foreach (var island in islands)
        {
            var script = island.GetComponent<IslandScript>();
            var people = new List<Person>(script.people);

            foreach (var person in people)
            {
                person.Consume(script.products, script.productsToBeAdded);
            }

            AddProducts();
            script.CheckFood();
        }

        foreach (var ship in shipList)
        {
            var people = new List<Person>(ship.passengers);
            people.AddRange(ship.employees);

            foreach (var person in people)
            {
                person.Consume(ship.products, new Products());
            }

            if(people.Count > 0)
                ship.CheckFood();
        }
    }
    
    private void AddAge()
    {
        foreach (var island in islands)
        {
            var script = island.GetComponent<IslandScript>();
            var people = new List<Person>(script.people);
            ManageAge(people);
        }

        foreach (var ship in shipList)
        {
            var people = new List<Person>(ship.passengers);
            people.AddRange(ship.employees);
            ManageAge(people);
        }
    }

    private void ManageAge(List<Person> people)
    {
        foreach (var p in people)
        {
            p.ageWeeks++;
            if (p.ageWeeks >= 53)
            {
                p.age++;
                p.ageWeeks = 0;
            }

            if (p.age >= 50)
            {
                var r = random.Next(0, 120 - p.age);
                if (r == 0)
                {
                    gameLog.Log(p.name + " died of an old age", p.GetPosition(), "People");
                    p.KillPerson();
                }
            }
        }
    }

    private void RandomEvents()
    {
        foreach (var island in islands)
        {
            var script = island.GetComponent<IslandScript>();

            Fire(script);
            Drought(script);
            AnimalPlague(script);
            FishPlague(script);
            FatalAccident(script);
            Sickness(script);
            Revolt(script);
        }

        foreach (var ship in shipList)
        {
            FatalAccident(ship);
            Sickness(ship);
        }
    }

    public void PersonEscape(IslandScript island)
    {
        var chances = 5;

        var people = new List<Person>(island.people);

        foreach (var person in people)
        {
            if (person.morale == 70)
                continue;

            if(person.home == null)
            {
                var r = random.Next(0, chances);
                if(r == 0)
                {
                    gameLog.Log(person.name + " has escaped the island due to being homeless", person.GetPosition(), "People");
                    person.KillPerson();
                }
            }
            if(!person.hasFood)
            {
                var r = random.Next(0, chances);
                if(r == 0)
                {
                    gameLog.Log(person.name + " has escaped the island due to them starving", person.GetPosition(), "People");
                    person.KillPerson();
                }
            }
        }
    }

    private void Revolt(IslandScript script)
    {
        var chances = 5; 
        
        var list = script.GetBuildingsOfType(typeof(PoliceStation));

        if (list.Count > 0)
        {
            foreach (var b in list)
            {
                if (((PoliceStation)b).isWorking)
                    chances = 10;
            }
        }

        var buildings = new List<Building>(script.buildings);

        foreach (var building in buildings)
        {
            if(building is IMorale b)
            {
                if(b.GetAvgMorale() < 60)
                {
                    var r = random.Next(0,chances);

                    if(r == 0)
                    {
                        gameLog.Log(building.GetBuildingInfo().buildingName + ": there was a revolt", building.transform.position, "Buildings");
                        script.StartRevolt(building);
                    }
                }
            }
        }
    }

    private void Sickness(Ship ship)
    {
        var chances = 200;

        var peopleList = new List<Person>(ship.passengers);
        peopleList.AddRange(ship.employees);

        if (peopleList.Count == 0)
            return;

        foreach (var person in peopleList)
        {
            if (person.isSick)
            {
                var chancesOfDeath = 5;

                person.sicknessWeeksLeft--;

                var r2 = random.Next(0, chancesOfDeath);
                if (r2 == 0)
                {
                    gameLog.Log(person.name + " died of sickness", person.GetPosition(), "People");
                    person.KillPerson();
                }

                if (person.sicknessWeeksLeft <= 0 && r2 != 0)
                {
                    person.isSick = false;
                    gameLog.Log(person.name + " has recovered from sickness", person.GetPosition(), "People");
                }
            }
            else
            {
                var r = random.Next(0, chances);

                if (r == 0)
                {
                    person.isSick = true;
                    person.sicknessWeeksLeft = random.Next(1, 6);
                }
            }
        }
    }

    private void Sickness(IslandScript script)
    {
        var chances = 200;

        var list = script.GetBuildingsOfType(typeof(Hospital));

        if (list.Count > 0)
        {
            foreach (var b in list)
            {
                if (((Hospital)b).isWorking)
                    chances = 500;
            }
        }

        var peopleList = new List<Person>(script.people);

        foreach (var person in peopleList)
        {
            if (person.isSick)
            {
                var chancesOfDeath = 5;
                if (chances == 500)
                    chancesOfDeath = 10;

                person.sicknessWeeksLeft--;

                var r2 = random.Next(0, chancesOfDeath);
                if (r2 == 0)
                {
                    gameLog.Log(person.name + " died of sickness", person.GetPosition(), "People");
                    person.KillPerson();
                }

                if (person.sicknessWeeksLeft <= 0 && r2 != 0)
                {
                    person.isSick = false;
                    gameLog.Log(person.name + " has recovered from sickness", person.GetPosition(), "People");
                }
            }
            else
            {
                var r = random.Next(0, chances);

                if (r == 0)
                {
                    gameLog.Log(person.name + " got sick", person.GetPosition(), "People");
                    person.isSick = true;
                    person.sicknessWeeksLeft = random.Next(1, 6);
                }
            }
        }
    }

    private void FatalAccident(Ship ship)
    {
        var chances = 1000;
        var people = new List<Person>(ship.passengers);
        people.AddRange(ship.employees);

        if (people.Count == 0)
            return;

        foreach (var person in people)
        {
            var r = random.Next(0, chances);

            if (r == 0)
            {
                gameLog.Log(person.name + " died in accident", person.GetPosition(), "People");
                person.KillPerson();
            }
        }
    }

    private void FatalAccident(IslandScript script)
    {
        var chances = 1000;

        var peopleList = new List<Person>(script.people);

        foreach (var person in peopleList)
        {
            var r = random.Next(0, chances); 
            
            if (r == 0)
            {
                gameLog.Log(person.name + " died in accident", person.GetPosition(), "People");
                person.KillPerson();
            }
        }
    }

    private void FishPlague(IslandScript script)
    {
        if (script.isFishPlague)
        {
            script.fishPlagueWeeksLeft--;
            if (script.fishPlagueWeeksLeft <= 0)
                script.isFishPlague = false;
            return;
        }

        var chances = 200;

        var r = random.Next(0, chances);

        if (r == 0)
        {
            gameLog.Log(script.islandName + ": fish plague started", script.GetPosition(), "Disasters");
            script.isFishPlague = true;
            script.fishPlagueWeeksLeft = random.Next(10, 20);
        }
    }

    private void AnimalPlague(IslandScript script)
    {
        if (script.isAnimalPlague)
        {
            script.animalPlagueWeeksLeft--;
            if (script.animalPlagueWeeksLeft <= 0)
                script.isAnimalPlague = false;
            return;
        }

        var chances = 200;

        var r = random.Next(0, chances);

        if (r == 0)
        {
            gameLog.Log(script.islandName + ": animal plague started", script.GetPosition(), "Disasters");
            script.isAnimalPlague = true;
            script.animalPlagueWeeksLeft = random.Next(10, 20);
        }
    }

    private void Drought(IslandScript script)
    {
        if (script.isDrought)
        {
            script.droughtWeeksLeft--;
            if (script.droughtWeeksLeft <= 0)
                script.isDrought = false;
            return;
        }

        var chances = 200;

        var r = random.Next(0, chances);

        if (r == 0)
        {
            gameLog.Log(script.islandName + ": drought started", script.GetPosition(), "Disasters");
            script.isDrought = true;
            script.droughtWeeksLeft = random.Next(10, 20);
        }
    }

    private void Fire(IslandScript script)
    {
        if (script.buildings.Count <= 0)
            return;

        var chances = 50;
        var list = script.GetBuildingsOfType(typeof(FireStation));

        if (list.Count > 0)
        {
            foreach (var b in list)
            {
                if (((FireStation)b).isWorking)
                    chances = 100;
            }
        }

        var r = random.Next(0, chances);

        if (r == 0)
        {
            script.SetFire(chances == 100);
        }
    }

    private void SpawnMerchants()
    {
        int amount = random.Next(-2, 3);
        for (int i = 0; i < amount; i++)
        {
            var ship = Instantiate(merchantShip);
            ship.transform.position = MerchantShip.endpoints[random.Next(0, MerchantShip.endpoints.Count)];
            merchantList.Add(ship);
        }
    }

    private void AddProducts()
    {
        foreach (var island in islands)
        {
            island.GetComponent<IslandScript>().AddProducts();
        }
    }

    private void CalculateProduction()
    {
        foreach (var island in islands)
        {
            var islandScript = island.GetComponent<IslandScript>();

            bool homeless = false;
            foreach (var person in islandScript.people)
            {
                person.CalculateProductivity();
                if(!homeless && person.home == null)
                    homeless = true;
            }
            if(homeless)
                gameLog.Log(islandScript.islandName + ": some people are homeless", islandScript.GetPosition(), "People");

            foreach (var building in islandScript.buildings)
            {
                if (building is IWorkingBuilding wb)
                    wb.CheckIsWorking();

                if (building.GetType().IsSubclassOf(typeof(ServiceBuilding)))
                {
                    ServiceBuilding serviceBuilding = (ServiceBuilding)building;
                    serviceBuilding.Calculate();
                }

                if (building.GetType().IsSubclassOf(typeof(ProcessingBuilding)))
                {
                    ProcessingBuilding productionBuilding = (ProcessingBuilding)building;
                    if(productionBuilding.isWorking)
                        productionBuilding.Produce();
                }
                else if (building.GetType().IsSubclassOf(typeof(ProductionBuilding)))
                {
                    ProductionBuilding productionBuilding = (ProductionBuilding)building;
                    if (productionBuilding.isWorking)
                        productionBuilding.Produce();
                }
                else if(building.GetType().Equals(typeof(Shipyard)))
                {
                    Shipyard shipyard = (Shipyard)building;
                    if (shipyard.isWorking)
                        shipyard.Produce();
                }
                else if(building.GetType().Equals(typeof(School)))
                {
                    School school = (School)building;
                    if (school.isWorking)
                        school.Produce();
                }
                else if(building.GetType().Equals(typeof(House)))
                {
                    House house = (House)building;
                    house.CheckIsWorking();
                }

                building.Log();
            }
        }
    }

    private void CheckCurrentIsland()
    {
        foreach (var island in islands)
        {
            if (camera.transform.position.x >= island.transform.position.x &&
                camera.transform.position.x < island.transform.position.x + 100 &&
                camera.transform.position.z >= island.transform.position.z &&
                camera.transform.position.z < island.transform.position.z + 100)
            {
                if (currentIsland == island)
                {
                    break;
                }

                currentIsland = island;

                OnIslandChanged?.Invoke();
                break;
            }
        }
    }

    public void LoadData(GameData data)
    {
        goldAmount = data.goldAmount;
        timeSinceStart = data.timeSinceStart;
        timeUntilNextWeek = data.timeUntilNextWeek;
        camera.transform.position = data.cameraPos;

        if(data.nextWeekSeed != 0)
        {
            nextWeekSeed = data.nextWeekSeed;
            random = new System.Random(nextWeekSeed);
        }

        bool empty = true;
        if (data.worldSurface[0][0] != 0)
            empty = false;

        if (!empty)
        {
            worldSurface = GameData.LoadArray(data.worldSurface);

            var treeList = GameObject.FindGameObjectsWithTag("Tree");

            foreach (var tree in treeList)
            {
                var pos = tree.transform.position;
                if (worldSurface[(int)pos.x, (int)pos.z] != 1)
                    Destroy(tree);
            }
        }


        if(data.shipList.Count > 0)
        {
            foreach (var ship in shipList)
            {
                Destroy(ship.shipGameObject);
            }

            shipList = new List<Ship>();

            foreach (var ship in data.shipList)
            {
                var s = new Ship(ship);
                GameObject shipPrefab = Resources.Load<GameObject>(Ship.prefabPath);
                s.shipGameObject = Instantiate(shipPrefab);
                s.shipGameObject.transform.position = ship.position;
                s.shipGameObject.transform.rotation = ship.rotation;
                s.shipGameObject.GetComponent<NavMeshAgent>().enabled = true;
                if(!ship.destination.Equals(new Vector3()))
                {
                    s.shipGameObject.GetComponent<NavMeshAgent>().destination = ship.destination;
                    s.shipGameObject.GetComponent<ShipController>().destinationPos = ship.destination;
                }
                s.SetJobHome();
                shipList.Add(s);
            }
        }

        if (data.islands[0] != null)
        {
            for (int i = 0; i < 16; i++)
            {
                var island = islands[i].GetComponent<IslandScript>();
                island.islandName = data.islands[i].islandName;
                island.productCapacity = data.islands[i].productCapacity;
                island.transportCapacity = data.islands[i].transportCapacity;

                island.products = data.islands[i].products;
                island.buildings = new List<Building>();
                island.people = new List<Person>();

                island.isDrought = data.islands[i].isDrought;
                island.droughtWeeksLeft = data.islands[i].droughtWeeksLeft;
                island.isAnimalPlague = data.islands[i].isAnimalPlague;
                island.animalPlagueWeeksLeft = data.islands[i].animalPlagueWeeksLeft;
                island.isFishPlague = data.islands[i].isFishPlague;
                island.fishPlagueWeeksLeft = data.islands[i].fishPlagueWeeksLeft;

                foreach (var person in data.islands[i].people)
                {
                    var p = new Person(person);
                    p.island = island;
                    island.people.Add(p);
                }

                var buildingList = new List<BuildingSerialized>(data.islands[i].buildings);
                buildingList.AddRange(data.islands[i].shipyards);

                foreach (var building in buildingList)
                {
                    BuildingInfo buildingInfo = null;

                    foreach (var info in BuildingInfo.buildingInfos)
                    {
                        if (info.buildingID == building.buildingID)
                        {
                            buildingInfo = info;
                            break;
                        }
                    }

                    if (buildingInfo.prefab == null)
                        buildingInfo.LoadBuildingInfo();

                    var b = Instantiate(buildingInfo.prefab);
                    b.transform.parent = island.gameObject.transform;
                    b.transform.position = building.position;
                    var buildingClass = b.GetComponent<Building>();
                    buildingClass.coords = building.coords;

                    island.AddBuilding(buildingClass);

                    if (buildingClass.GetType().Equals(typeof(Shipyard)))
                    {
                        foreach (var id in building.peopleIDs)
                        {
                            var person = island.people.Where(p => p.id == id).FirstOrDefault();
                            person.job = buildingClass.gameObject;
                            ((IEmployees)buildingClass).employees.Add(person);
                        }

                        ((Shipyard)buildingClass).shipStone = ((ShipyardSerialized)building).shipStone;
                        ((Shipyard)buildingClass).shipWood = ((ShipyardSerialized)building).shipWood;
                        ((Shipyard)buildingClass).shipIron = ((ShipyardSerialized)building).shipIron;
                        ((Shipyard)buildingClass).shipGold = ((ShipyardSerialized)building).shipGold;
                        ((Shipyard)buildingClass).shipCotton = ((ShipyardSerialized)building).shipCotton;
                        ((Shipyard)buildingClass).shipProgress = ((ShipyardSerialized)building).shipProgress;
                        ((Shipyard)buildingClass).shipInProgress = ((ShipyardSerialized)building).shipInProgress;
                    }
                    else if (buildingClass.GetType().Equals(typeof(School)))
                    {
                        foreach (var id in building.peopleIDs)
                        {
                            var person = island.people.Where(p => p.id == id).FirstOrDefault();
                            person.job = buildingClass.gameObject;

                            if(!person.professionInProgress.Equals(string.Empty))
                                ((School)buildingClass).students.Add(person, person.professionInProgress);
                            else
                                ((IEmployees)buildingClass).employees.Add(person);
                        }
                    }
                    else if (buildingClass.GetType().Equals(typeof(House)))
                    {
                        foreach (var id in building.peopleIDs)
                        {
                            var person = island.people.Where(p => p.id == id).FirstOrDefault();
                            person.home = buildingClass.gameObject;
                            ((House)buildingClass).residents.Add(person);
                        }
                    }
                    else if (buildingClass is IEmployees)
                    {
                        foreach (var id in building.peopleIDs)
                        {
                            var person = island.people.Where(p => p.id == id).FirstOrDefault();
                            person.job = buildingClass.gameObject;
                            ((IEmployees)buildingClass).employees.Add(person);
                        }
                    }
                }

                //island.RefreshBuildings();
            }
        }

        foreach (var merchant in data.merchantList)
        {
            var ship = Instantiate(merchantShip);
            ship.transform.position = merchant.position;
            ship.GetComponent<NavMeshAgent>().destination = merchant.destinationPos;
            ship.GetComponent<MerchantShip>().LoadData(merchant);
            merchantList.Add(ship);
        }
    }

    public void SaveData(GameData data)
    {
        data.goldAmount = goldAmount;
        data.worldSurface = GameData.SaveArray(worldSurface);
        data.timeSinceStart = (float)(timeSinceStart + Time.timeSinceLevelLoadAsDouble);
        data.timeUntilNextWeek = timeUntilNextWeek;
        data.nextWeekSeed = nextWeekSeed;
        data.cameraPos = camera.transform.position;

        data.shipList = new List<ShipSerialized>();
        foreach (var ship in shipList)
        {
            data.shipList.Add(ship.GetSerialized());
        }

        for (int i = 0; i < 16; i++)
        {
            var island = islands[i].GetComponent<IslandScript>();
            data.islands[i] = new IslandSerialized();
            data.islands[i].islandName = island.islandName;
            data.islands[i].productCapacity = island.productCapacity;
            data.islands[i].transportCapacity = island.transportCapacity;

            data.islands[i].products = island.products;
            data.islands[i].buildings = new List<BuildingSerialized>();
            data.islands[i].people = new List<PersonSerialized>();
            data.islands[i].shipyards = new List<ShipyardSerialized>();

            data.islands[i].isDrought = island.isDrought;
            data.islands[i].droughtWeeksLeft = island.droughtWeeksLeft;
            data.islands[i].isAnimalPlague = island.isAnimalPlague;
            data.islands[i].animalPlagueWeeksLeft = island.animalPlagueWeeksLeft;
            data.islands[i].isFishPlague = island.isFishPlague;
            data.islands[i].fishPlagueWeeksLeft = island.fishPlagueWeeksLeft;

            foreach (var person in island.people)
            {
                data.islands[i].people.Add(person.GetSerialized());
            }

            foreach (var building in island.buildings)
            {
                var list = new List<string>();

                if(building is IEmployees)
                {
                    foreach (var e in ((IEmployees)building).employees)
                    {
                        list.Add(e.id);
                    }
                }

                if(building.GetType().Equals(typeof(House)))
                {
                    foreach (var r in ((House)building).residents)
                    {
                        list.Add(r.id);
                    }
                }

                if(building.GetType().Equals(typeof(School)))
                {
                    foreach (var r in ((School)building).students)
                    {
                        list.Add(r.Key.id);
                    }
                }

                if (building.GetType().Equals(typeof(Shipyard)))
                {
                    var s = new ShipyardSerialized
                    {
                        buildingID = building.GetBuildingInfo().buildingID,
                        position = building.gameObject.transform.position,
                        coords = building.coords,
                        peopleIDs = list,
                        shipWood = ((Shipyard)building).shipWood,
                        shipCotton = ((Shipyard)building).shipCotton,
                        shipIron = ((Shipyard)building).shipIron,
                        shipStone = ((Shipyard)building).shipStone,
                        shipGold = ((Shipyard)building).shipGold,
                        shipProgress = ((Shipyard)building).shipProgress,
                        shipInProgress = ((Shipyard)building).shipInProgress
                    };
                    data.islands[i].shipyards.Add(s);
                    continue;
                }

                var b = new BuildingSerialized { buildingID = building.GetBuildingInfo().buildingID, position = building.gameObject.transform.position, coords = building.coords, peopleIDs = list };
                data.islands[i].buildings.Add(b);
            }

            data.merchantList = new List<MerchantSerialized>();
            foreach (var merchant in merchantList)
            {
                var m = merchant.GetComponent<MerchantShip>();

                data.merchantList.Add(m.GetSerialized());
            }
        }
    }
}
