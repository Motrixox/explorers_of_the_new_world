using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    private float panSpeed = 40f;

    private new GameObject camera;
    private  GameObject map;

    Vector3 mousePos;

    private GameState gameState;

    void Start()
    {
        map = GameObject.Find("Map");
        camera = GameObject.FindWithTag("MainCamera");
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        mousePos = Input.mousePosition;
    }

    void Update()
    {
        mousePos = Input.mousePosition;

        if(!gameState.isGamePaused)
        {
            PanScreen(mousePos.x, mousePos.y);

            CameraZoom();
        }
    }

    private void CameraZoom()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        else if (camera.GetComponent<Camera>().orthographicSize < 20 && Input.mouseScrollDelta.y < 0f)
        {
            camera.GetComponent<Camera>().orthographicSize += 1;
            map.GetComponent<MinimapController>().AddZoomScale(0.1f);
        }
        else if (camera.GetComponent<Camera>().orthographicSize > 8 && Input.mouseScrollDelta.y > 0f)
        {
            camera.GetComponent<Camera>().orthographicSize -= 1;
            map.GetComponent<MinimapController>().AddZoomScale(-0.1f);
        }
    }

    private Vector3 PanDirection(float x, float y)
    {
        Vector3 direction = Vector3.zero;

        if (x >= Screen.width * 0.98f)
        {
            if (camera.transform.position.x < 400f && camera.transform.position.z > 0f)
            {
                direction.x += 1f;
                direction.z -= 1f;
            }
            else if (camera.transform.position.x > 400f && camera.transform.position.z > 0f)
            {
                direction.z -= 1f;
            }
            else if (camera.transform.position.x < 400f && camera.transform.position.z < 0f)
            {
                direction.x += 1f;
            }
        }
        
        if (x <= Screen.width * 0.02f)
        {
            if (camera.transform.position.x > 0f && camera.transform.position.z < 400f)
            {
                direction.x -= 1f;
                direction.z += 1f;
            }
            else if (camera.transform.position.x < 0f && camera.transform.position.z < 400f)
            {
                direction.z += 1f;
            }
            else if (camera.transform.position.x > 0f && camera.transform.position.z > 400f)
            {
                direction.x -= 1f;
            }
        }

        if (y >= Screen.height * 0.98f)
        {
            if (camera.transform.position.x < 400f && camera.transform.position.z < 400f)
            {
                direction.x += 1f;
                direction.z += 1f;
            }
            else if (camera.transform.position.x > 400f && camera.transform.position.z < 400f)
            {
                direction.z += 1f;
            }
            else if (camera.transform.position.x < 400f && camera.transform.position.z > 400f)
            {
                direction.x += 1f;
            }
        }

        if (y <= Screen.height * 0.02f)
        {
            if (camera.transform.position.x > 0f && camera.transform.position.z > 0f)
            {
                direction.x -= 1f;
                direction.z -= 1f;
            }
            else if (camera.transform.position.x < 0f && camera.transform.position.z > 0f)
            {
                direction.z -= 1f;
            }
            else if (camera.transform.position.x > 0f && camera.transform.position.z < 0f)
            {
                direction.x -= 1f;
            }
        }

        return direction;
    }

    public void PanScreen(float x, float y) //mouse pos
    {
        Vector3 direction = Vector3.zero;

        direction = PanDirection(x, y);
        
        transform.position = Vector3.Lerp(transform.position,
                                                transform.position + (Vector3)direction * panSpeed,
                                                Time.unscaledDeltaTime);
    }
}
