using DevionGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarborController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var worldBase = GameObject.Find("GameState").GetComponent<GameState>().worldBase;
        var harbor = gameObject.FindChild("harbor", true).transform;

        for (int i = 0; i < harbor.childCount; i++)
        {
            var a = harbor.GetChild(i);

            if (a.name.Equals("door") || a.name.Equals("building"))
                continue;

            var pos = a.transform.position;
            int x = (int)Mathf.Round(pos.x);
            int z = (int)Mathf.Round(pos.z);

            if (worldBase[x, z] == 1)
                Destroy(a.gameObject);

            if(a.name.StartsWith("port_wooden_bridge_a"))
            {
                if (worldBase[x, z] != 0)
                {
                    Destroy(a.gameObject);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
