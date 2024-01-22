using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    public GameObject generator;
    private new GameObject camera;
    private GameState gameState;

    private int[,] world;

    private Texture2D baseTexture;
    private Texture2D mapTexture;

	private Sprite mapBase;
    private RectTransform cameraSquare;

    private float zoomScale = 1.0f;

    void Start()
    {
        camera = GameObject.FindWithTag("MainCamera");
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        baseTexture = new Texture2D(400, 400);
		mapTexture = new Texture2D(400, 400);
		SetMapBase();
        GetComponent<Image>().sprite = mapBase;

        cameraSquare = transform.parent.Find("CameraSquare").GetComponent<RectTransform>();
    }

    public void AddZoomScale(float a)
    {
        zoomScale += a;
    }

    private void SetMapBase()
    {
        world = gameState.worldBase;

        for (int i = 0; i < 400; i++)
        {
            for (int j = 0; j < 400; j++)
            {
                if (world[i, j] == 0)
                { 
                    baseTexture.SetPixel(i, j, new UnityEngine.Color(0.4f, 0.4f, 1f));
                }
                else if (world[i, j] == 1)
                {
                    baseTexture.SetPixel(i, j, new UnityEngine.Color(0.2f, 1f, 0.2f));
                }
                else if (world[i, j] == 2)
                {
                    baseTexture.SetPixel(i, j, new UnityEngine.Color(1f, 0f, 0f));
                }
                else if (world[i, j] == 5)
                {
                    baseTexture.SetPixel(i, j, new UnityEngine.Color(0.8f, 1f, 0.8f));
                }
            }
        }

        baseTexture.Apply();

        mapBase = Sprite.Create(baseTexture, new Rect(0.0f, 0.0f, baseTexture.width, baseTexture.height), new Vector2(0.5f, 0.5f));
    }

    public void UpdateMap()
    {
        Graphics.CopyTexture(mapBase.texture, mapTexture);

        world = gameState.worldSurface;

        for (int i = 0; i < 400; i++)
        {
            for (int j = 0; j < 400; j++)
            {
                if (world[i, j] >= 6)
                {
                    mapTexture.SetPixel(i, j, new UnityEngine.Color(1f, 0f, 0f));
                }
            }
        }

        foreach (var ship in gameState.shipList)
        {
            int x = (int)ship.shipGameObject.transform.position.x;
            int z = (int)ship.shipGameObject.transform.position.z;

            for (int i = -2; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    mapTexture.SetPixel(x + i, z + j, new UnityEngine.Color(1f, 0f, 0f));
                }
            }
        }
        foreach (var ship in gameState.merchantList)
        {
            int x = (int)ship.gameObject.transform.position.x;
            int z = (int)ship.gameObject.transform.position.z;

            for (int i = -2; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    if ((x + i) < 0 || (x + i) > 400 || (z + j) < 0 || (z + j) > 400)
                        continue;
                    mapTexture.SetPixel(x + i, z + j, new UnityEngine.Color(1f, 1f, 0.8f));
                }
            }
        }
        mapTexture.Apply();

        var mapUpdated = Sprite.Create(mapTexture, new Rect(0.0f, 0.0f, mapTexture.width, mapTexture.height), new Vector2(0.5f, 0.5f));

        GetComponent<Image>().sprite = mapUpdated;
    }

    // Update is called once per frame
    void Update()
    {
        var windowSize = transform.parent.transform.parent.GetComponent<RectTransform>().rect.height;

        var ratio = windowSize / 400;

        var camX = camera.transform.position.x * ratio;
        var camZ = camera.transform.position.z * ratio;

        cameraSquare.localScale = new Vector2(windowSize / 300 * zoomScale, windowSize / 300 * zoomScale);

        //cameraSquare.rect.y = camera.transform.position.x;
        //cameraSquare.position = new Vector3(camera.transform.position.z, camera.transform.position.x, 0);
        cameraSquare.localPosition = new Vector3((camX - camZ) / 2,
                                                 (camX + camZ) / 2 - windowSize / 2,
                                                 0);

    }
}
