using DevionGames;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ShipController : MonoBehaviour
{
    private LayerMask whatToHit = 80; // 2^4 + 2^6, 4 = water, 6 = ground

    private new Camera camera;

    public bool active { get; private set; } = false;

    public Vector3 destinationPos;

    private bool isMoving = false;

    private GameObject activeGameObject;
    private GameObject destinationGameObject;

    private GameObject shipInterface;

    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Awake()
    {
        camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        activeGameObject = transform.Find("Active").GameObject();
        destinationGameObject = GameObject.Find("Destination").GameObject();
        shipInterface = GameObject.Find("Canvas").FindChild("Ship Interface", true).GameObject();
    }

    // Update is called once per frame
    void Update()
    {
        //PlaceGameObjects();

        if (active && Input.GetKeyUp(KeyCode.Mouse1))
        {
            CalculateDestination();
            navMeshAgent.destination = destinationPos;
        }

        if(CheckIfDestinationReached())
        {
            isMoving = false;
        }

        if (active && !isMoving)
            destinationGameObject.GetComponent<MeshRenderer>().enabled = false;
        else if (active && isMoving)
            destinationGameObject.GetComponent<MeshRenderer>().enabled = true;

    }

    public void SetActive(bool a)
    {
        active = a;

        shipInterface.SetActive(a);

        if (active)
        {
            activeGameObject.GetComponent<MeshRenderer>().enabled = true;
            destinationGameObject.transform.position = destinationPos;

            if(!CheckIfDestinationReached())
            {
                destinationGameObject.GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                destinationGameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        else
        {
            activeGameObject.GetComponent<MeshRenderer>().enabled = false;
            destinationGameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void CalculateDestination()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 200.0f, whatToHit) && hit.transform.tag == "Water")
        {
            destinationPos = hit.point;
            destinationGameObject.transform.position = destinationPos;
            destinationPos.y = -0.8f;

            isMoving = true;
        }
    }

    private bool CheckIfDestinationReached()
    {
        if (isMoving &&
            transform.position.x > destinationPos.x - 0.3f &&
            transform.position.x < destinationPos.x + 0.3f &&
            transform.position.z > destinationPos.z - 0.3f &&
            transform.position.z < destinationPos.z + 0.3f)
        {
            return true;
        }

        return false;
    }

    public void SetSpeed(float speed, float angularSpeed)
    {
        navMeshAgent.speed = speed;
        navMeshAgent.angularSpeed = angularSpeed;
    }
}
