using DevionGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertBoxScript : MonoBehaviour
{
    public GameObject alert;
    public float timeToDisappear = 5f;

    private GameObject alertList;
    private int alertCount = 0;
    private Queue<float?> alertTimes;

    // Start is called before the first frame update
    void Start()
    {
        alertTimes = new Queue<float?>();
        alertList = gameObject.FindChild("Slots", true);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Alert(string message)
    {
        var a = Instantiate(alert);
        a.GetComponent<Text>().text = message;

        a.transform.SetParent(alertList.transform);
        alertTimes.Enqueue(Time.realtimeSinceStartup);
        //Invoke(nameof(DestroyAlert), timeToDisappear);

        alertCount++;

        if (alertCount <= 1)
        {
            gameObject.SetActive(true);
            StartCoroutine(DoCheck());
        }
    }

    IEnumerator DoCheck()
    {
        for (; ; )
        {
            if (alertCount <= 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                var time = alertTimes.Peek();

                while ((time + 5f) <= Time.realtimeSinceStartup)
                {
                    Destroy(alertList.transform.GetChild(0).gameObject);
                    alertCount--;
                    alertTimes.Dequeue();

                    if (alertCount > 0)
                        time = alertTimes.Peek();
                    else
                        break;
                }
            }

            yield return new WaitForSecondsRealtime(.1f);
        }
    }

    private void DestroyAlert()
    {
        Destroy(alertList.transform.GetChild(0).gameObject);
        alertCount--;
        if (alertCount == 0) gameObject.SetActive(false);
    }
}
