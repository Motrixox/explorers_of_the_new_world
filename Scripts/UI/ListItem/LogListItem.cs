using DevionGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogListItem : MonoBehaviour
{
    private Text message;
    private Vector3 position;
    private GameObject notification;

    private static new GameObject camera;

    // Start is called before the first frame update
    void Awake()
    {
        if(camera == null)
            camera = GameObject.FindWithTag("MainCamera");
        message = gameObject.FindChild("Message", true).GetComponent<Text>();
        notification = gameObject.FindChild("Notification", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(string message, Vector3 position)
    {
        this.message.text = message;
        this.position = position;
    }

    public void MoveCameraToEventPlace()
    {
        notification.SetActive(false);
        var pos = position;
        pos.y = 0f;
        camera.transform.position = pos;
    }

    public void Delete()
    {
        Destroy(gameObject);
    }
}
