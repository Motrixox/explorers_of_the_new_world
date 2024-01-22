using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using DevionGames;
using Assets.Scripts.Interfaces;

public class PlayerActionController : MonoBehaviour
{
    private int state = 0; // 0 = select, 1 = build, 2 = destroy

    private new Camera camera;
    private LayerMask whatToHitSelect = 384; // 2^7 + 2^8,  7 = selectable, 8 = building
    private LayerMask whatToHitBuild = 64; // 2^6,  6 = ground
    private LayerMask whatToHitDestroy = 768; // 2^8 + 2^9,  8 = building, 9 = tree

    private GameObject buildGreen3x3;
    private GameObject buildGreen2x2;
    private GameObject buildGreen1x1;
    private GameObject buildRed3x3;
    private GameObject buildRed2x2;
    private GameObject buildRed1x1;

    private AlertBoxScript alert;

    private GameObject activeGameObject;
    private GameObject buildingInterface;

    private int buildingID;

    private GameState gameState;
    private Ship ship;

    private bool buildingsNeedToBeRefreshed = false;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        buildGreen3x3 = GameObject.Find("green3x3");
        buildGreen2x2 = GameObject.Find("green2x2");
        buildGreen1x1 = GameObject.Find("green1x1");
        buildRed3x3 = GameObject.Find("red3x3");
        buildRed2x2 = GameObject.Find("red2x2");
        buildRed1x1 = GameObject.Find("red1x1");
        alert = GameObject.Find("Canvas").FindChild("AlertBox", true).GetComponent<AlertBoxScript>();
        buildingInterface = GameObject.Find("Canvas").FindChild("Building Interface", true);

        var script = GameObject.Find("GenerateMap").GetComponent<WorldGeneratingScript>();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshBuildings();
        // this all need to be switched to events

        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Mouse1))
        {
            state = 0;
            ResetBuildingHelpers();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && state == 0)
        {
            Select();
        }

        if (state == 1)
        {
            if (buildingID == 99)
                BuildHarborFromShip(2,2);
            else if ((buildingID >= 6 && buildingID <= 10) || buildingID == 43)
                CheckBuild(1, 1, buildingID);
            else
                CheckBuild(2,2, buildingID);
        }
        else if(state == 2)
        {
            Destroy();
        }

        //time scale
        if (!gameState.isGamePaused)
        {
            if (Input.GetKeyUp(KeyCode.F1))
            {
                gameState.SetTimeScale(0);
            }
            if (Input.GetKeyUp(KeyCode.F2))
            {
                gameState.SetTimeScale(1);
            }
            if (Input.GetKeyUp(KeyCode.F3))
            {
                gameState.SetTimeScale(2);
            }
            if (Input.GetKeyUp(KeyCode.F4)) // DEBUG
            {
                gameState.SetTimeScale(20);
            }
        }

        //DEBUG
        if (Input.GetKeyUp(KeyCode.Q))
        {
            gameState.currentIsland.GetComponent<IslandScript>().products.AddQuantity("wood", 1000);
            gameState.CallOnIlsandChanged();
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            gameState.currentIsland.GetComponent<IslandScript>().products.AddQuantity("stone", 1000);
            gameState.CallOnIlsandChanged();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            gameState.currentIsland.GetComponent<IslandScript>().products.AddQuantity("iron", 1000);
            gameState.CallOnIlsandChanged();
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            gameState.goldAmount += 1000;
            gameState.CallOnIlsandChanged();
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            foreach (var key in Products.food)
            {
                gameState.currentIsland.GetComponent<IslandScript>().products.AddQuantity(key, 200);
            }
            gameState.CallOnIlsandChanged();
        }
        if (Input.GetKeyUp(KeyCode.Y))
        {
            var p = PersonFactory.CreatePerson(Guid.NewGuid().GetHashCode());
            p.island = gameState.currentIsland.GetComponent<IslandScript>();
            gameState.currentIsland.GetComponent<IslandScript>().people.Add(p);
            gameState.CallOnIlsandChanged();
        }
        //DEBUG
    }

    private void RefreshBuildings()
    {
        if (!buildingsNeedToBeRefreshed)
            return;

        if(Input.GetMouseButtonUp(0))
        {
			gameState.currentIsland.GetComponent<IslandScript>().RefreshBuildings();
            buildingsNeedToBeRefreshed = false;
		}
	}

    public void SetStateBuild(int buildingID)
    {
        state = 1;

        this.buildingID = buildingID;
    }

    public void SetStateBuildHarborFromShip(Ship s)
    {
        state = 1;
        ship = s;
        this.buildingID = 99;
    }

    public void SetStateDestroy()
    {
        state = 2;
    }

    private void Select()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        else if (Physics.Raycast(ray, out RaycastHit hit, 200.0f, whatToHitSelect) && hit.transform.tag == "Ship")
        {
            Deactivate();

            activeGameObject = hit.collider.gameObject;
            var controller = activeGameObject.GetComponent<ShipController>();

            controller.SetActive(true);
        }
        else if (Physics.Raycast(ray, out hit, 200.0f, whatToHitSelect) && hit.transform.tag == "Building2x2")
        {
            Deactivate();

            activeGameObject = hit.collider.gameObject;

            activeGameObject.FindChild("Active", true).GetComponent<MeshRenderer>().enabled = true;
            activeGameObject.FindChild("Active", true).FindChild("range5", true).SetActive(true);
            buildingInterface.GetComponent<BuildingInterface>().SetBuilding(activeGameObject.GetComponent<Building>());
            buildingInterface.SetActive(true);
        }
        else 
        {
            if(activeGameObject != null)
            {
                Deactivate();
            }
        }
    }

    private void Deactivate()
    {
        if (activeGameObject != null && activeGameObject.tag == "Ship")
        {
            var controller = activeGameObject.GetComponent<ShipController>();
            controller.SetActive(false);
            activeGameObject = null;
        }
        else if (activeGameObject != null && activeGameObject.tag == "Building2x2")
        {
            activeGameObject.FindChild("Active", true).GetComponent<MeshRenderer>().enabled = false;
            activeGameObject.FindChild("Active", true).FindChild("range5", true).SetActive(false);
            buildingInterface.SetActive(false);
            activeGameObject = null;
        }
    }

    private void CheckBuild(int xLength, int zLength, int buildingID)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (EventSystem.current.IsPointerOverGameObject())
        {
            ResetBuildingHelpers();
            return;
        }
        else if (Physics.Raycast(ray, out RaycastHit hit, 200.0f, whatToHitBuild))
        {
            int emptyFields = 0;

            int x = (int) Math.Ceiling(hit.point.x);
            int z = (int) Math.Ceiling(hit.point.z);

            List<Vector2> coords = new List<Vector2>();

            for (int i = 0; i < xLength; i++)
            {
                for (int j = 0; j < zLength; j++)
                {
                    if (gameState.worldBase[x + i, z + j] == 1 && gameState.worldSurface[x + i, z + j] == 0)
                    {
                        emptyFields++;
                        coords.Add(new Vector2(x + i, z + j));
                    }
                }
            }

            if(buildingID == 22 || buildingID == 42) //harbor & shipyard check
            {
                int beachFields = 0;

                for (int i = -1; i < 3; i++)
                {
                    for (int j = -1; j < 3; j++)
                    {
                        if (gameState.worldBase[x + i, z + j] == 2 && gameState.worldSurface[x + i, z + j] == 0)
                            beachFields++;
                    }
                }

                if (beachFields < 2)
                {
                    ResetBuildingHelpers();
                    return;
                }
            }

            if (emptyFields == xLength * zLength)
            {
                if (xLength * zLength == 1)
                    buildGreen1x1.transform.position = new Vector3(x - 0.5f + 0.5f * xLength, -0.5f, z - 0.5f + 0.5f * zLength);
                else if (xLength * zLength == 4)
                    buildGreen2x2.transform.position = new Vector3(x - 0.5f + 0.5f * xLength, -0.5f, z - 0.5f + 0.5f * zLength);
                else if (xLength * zLength == 9)
                    buildGreen3x3.transform.position = new Vector3(x - 0.5f + 0.5f * xLength, -0.5f, z - 0.5f + 0.5f * zLength);
            }
            else
            {
                ResetBuildingHelpers();
            }

            if (emptyFields == xLength * zLength && Input.GetMouseButton(0))
            {
                GameObject buildingGameObject = null;
                BuildingInfo buildingInstance = null;

                foreach (var info in BuildingInfo.buildingInfos)
                {
                    if(info.buildingID == buildingID)
                    {
                        if (info.prefab == null)
                            info.LoadBuildingInfo();

                        buildingInstance = info;
                        buildingGameObject = Instantiate(info.prefab);
                        buildingGameObject.GetComponent<Building>().coords = coords;
                        break;
                    }
                }

                if(!CheckResources(buildingInstance))
                {
                    Alert("Not enough resources!");
                    Destroy(buildingGameObject);
                    state = 0;
                    ResetBuildingHelpers();
                    return;
                }

                TakeResources(buildingInstance);

                for (int i = 0; i < xLength; i++)
                {
                    for (int j = 0; j < zLength; j++)
                    {
                        gameState.worldSurface[x + i, z + j] = buildingID;
                    }
                }

                buildingGameObject.transform.parent = gameState.currentIsland.transform;
                buildingGameObject.transform.position = new Vector3(x - 0.5f + 0.5f * xLength, -0.5f, z - 0.5f + 0.5f * zLength);


                gameState.currentIsland.GetComponent<IslandScript>().AddBuilding(buildingGameObject.GetComponent<Building>());
				buildingsNeedToBeRefreshed = true;

				ResetBuildingHelpers();
            }
        }
    }
    
    private void BuildHarborFromShip(int xLength, int zLength)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (EventSystem.current.IsPointerOverGameObject())
        {
            ResetBuildingHelpers();
            return;
        }
        else if (Physics.Raycast(ray, out RaycastHit hit, 200.0f, whatToHitBuild))
        {
            int emptyFields = 0;

            int x = (int) Math.Ceiling(hit.point.x);
            int z = (int) Math.Ceiling(hit.point.z);

            List<Vector2> coords = new List<Vector2>();

            for (int i = 0; i < xLength; i++)
            {
                for (int j = 0; j < zLength; j++)
                {
                    if (gameState.worldBase[x + i, z + j] == 1 && gameState.worldSurface[x + i, z + j] == 0)
                    {
                        emptyFields++;
                        coords.Add(new Vector2(x + i, z + j));
                    }
                }
            }

            var shipPos = ship.shipGameObject.transform.position;
            var xRadius = shipPos.x - x;
            var zRadius = shipPos.z - z;
            if ((xRadius * xRadius + zRadius * zRadius) >= (ship.radius * ship.radius))
            {
                ResetBuildingHelpers();
                return;
            }

            int beachFields = 0;

            for (int i = -1; i < 3; i++)
            {
                for (int j = -1; j < 3; j++)
                {
                    if (gameState.worldBase[x + i, z + j] == 2 && gameState.worldSurface[x + i, z + j] == 0)
                        beachFields++;
                }
            }

            if(beachFields < 2)
            {
                ResetBuildingHelpers();
                return;
            }

            if (emptyFields == xLength * zLength)
            {
                if (xLength * zLength == 1)
                    buildGreen1x1.transform.position = new Vector3(x - 0.5f + 0.5f * xLength, -0.5f, z - 0.5f + 0.5f * zLength);
                else if (xLength * zLength == 4)
                    buildGreen2x2.transform.position = new Vector3(x - 0.5f + 0.5f * xLength, -0.5f, z - 0.5f + 0.5f * zLength);
                else if (xLength * zLength == 9)
                    buildGreen3x3.transform.position = new Vector3(x - 0.5f + 0.5f * xLength, -0.5f, z - 0.5f + 0.5f * zLength);
            }
            else
            {
                ResetBuildingHelpers();
            }

            if (emptyFields == xLength * zLength && Input.GetMouseButton(0))
            {
                BuildingInfo buildingInstance = null;
                GameObject buildingGameObject;

                buildingInstance = Harbor.buildingInfo;

                if (buildingInstance.prefab == null)
                    buildingInstance.LoadBuildingInfo();

                buildingGameObject = Instantiate(buildingInstance.prefab);
                buildingGameObject.GetComponent<Building>().coords = coords;

                if ((buildingInstance.costWood > ship.products.GetQuantity("wood")) || (buildingInstance.costIron > ship.products.GetQuantity("iron")) || 
                        (buildingInstance.costStone > ship.products.GetQuantity("stone")) || (buildingInstance.costGold > gameState.goldAmount))
                {
                    Alert("Not enough resources!");
                    Destroy(buildingGameObject);
                    state = 0;
                    ResetBuildingHelpers();
                    return;
                }
                else
                {
                    ship.products.RemoveQuantity("wood", buildingInstance.costWood);
                    ship.products.RemoveQuantity("iron", buildingInstance.costIron);
                    ship.products.RemoveQuantity("stone", buildingInstance.costStone);
                    gameState.goldAmount -= buildingInstance.costGold;
                }

                for (int i = 0; i < xLength; i++)
                {
                    for (int j = 0; j < zLength; j++)
                    {
                        gameState.worldSurface[x + i, z + j] = buildingInstance.buildingID;
                    }
                }

                buildingGameObject.transform.parent = gameState.currentIsland.transform;
                buildingGameObject.transform.position = new Vector3(x - 0.5f + 0.5f * xLength, -0.5f, z - 0.5f + 0.5f * zLength);

                gameState.currentIsland.GetComponent<IslandScript>().AddBuilding(buildingGameObject.GetComponent<Harbor>());
				buildingsNeedToBeRefreshed = true;

				ResetBuildingHelpers();
            }
        }
    }

    private void Destroy()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (EventSystem.current.IsPointerOverGameObject())
        {
            ResetBuildingHelpers();
            return;
        }
        else if (Physics.Raycast(ray, out RaycastHit hit, 200.0f, whatToHitDestroy))
        {
            int x = (int)Math.Floor(hit.transform.position.x);
            int z = (int)Math.Floor(hit.transform.position.z);

            ResetBuildingHelpers();
            if (hit.transform.CompareTag("Tree") || hit.transform.CompareTag("Building1x1"))
            {
                buildRed1x1.transform.position = hit.transform.position;
            }
            else if (hit.transform.CompareTag("Building2x2"))
            {
                buildRed2x2.transform.position = hit.transform.position;
            }
            else if (hit.transform.CompareTag("Building3x3"))
            {
                buildRed3x3.transform.position = hit.transform.position;
            }
            else
            {
                return;
            }
            

            if (Input.GetMouseButton(0))
            {
                if (hit.transform.CompareTag("Tree") || hit.transform.CompareTag("Building1x1"))
                {
                    gameState.worldSurface[x, z] = 0;
                }
                else if (hit.transform.CompareTag("Building2x2"))
                {
                    gameState.worldSurface[x, z] = 0;
                    gameState.worldSurface[x + 1, z] = 0;
                    gameState.worldSurface[x, z + 1] = 0;
                    gameState.worldSurface[x + 1, z + 1] = 0;
                }
                else if (hit.transform.CompareTag("Building3x3"))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            gameState.worldSurface[x + i, z + j] = 0;
                        }
                    }
                }

                ResetBuildingHelpers();

                gameState.currentIsland.GetComponent<IslandScript>().RemoveBuilding(hit.transform.gameObject);
                buildingsNeedToBeRefreshed = true;

                Destroy(hit.transform.gameObject);
            }
        }
        else
        {
            ResetBuildingHelpers();
        }
    }

    private void ResetBuildingHelpers()
    {
        buildGreen3x3.transform.position = new Vector3(-100, 0, -100);
        buildGreen2x2.transform.position = new Vector3(-100, 0, -100);
        buildGreen1x1.transform.position = new Vector3(-100, 0, -100);
        buildRed3x3.transform.position = new Vector3(-100, 0, -100);
        buildRed2x2.transform.position = new Vector3(-100, 0, -100);
        buildRed1x1.transform.position = new Vector3(-100, 0, -100);
    }

    private bool CheckResources(BuildingInfo b)
    {
        if (b == null)
            return false;

        if (b.costWood > gameState.currentIsland.GetComponent<IslandScript>().products.GetQuantity("wood"))
            return false;

        if (b.costIron > gameState.currentIsland.GetComponent<IslandScript>().products.GetQuantity("iron"))
            return false;

        if (b.costStone > gameState.currentIsland.GetComponent<IslandScript>().products.GetQuantity("stone"))
            return false;

        if (b.costGold > gameState.goldAmount)
            return false;

        return true;
    }

    private void TakeResources(BuildingInfo b)
    {
        gameState.currentIsland.GetComponent<IslandScript>().products.RemoveQuantity("wood", b.costWood);
        gameState.currentIsland.GetComponent<IslandScript>().products.RemoveQuantity("iron", b.costIron);
        gameState.currentIsland.GetComponent<IslandScript>().products.RemoveQuantity("stone", b.costStone);
        gameState.goldAmount -= b.costGold;
    }

    private void Alert(string s)
    {
        alert.Alert(s);
    }

}
