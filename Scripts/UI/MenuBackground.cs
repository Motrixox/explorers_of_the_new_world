using Assets.Scripts.Extensions;
using DevionGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBackground : MonoBehaviour
{
    private RawImage image1;
    private RawImage image2;
    private List<Texture2D> textureList;
    private float timeToChange = 0f;
    private bool switchBool = true;

    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = 60;

        image1 = gameObject.FindChild("1", true).GetComponent<RawImage>();
        image2 = gameObject.FindChild("2", true).GetComponent<RawImage>();
        textureList = new List<Texture2D>(Resources.LoadAll<Texture2D>("background"));

        image1.texture = textureList.PickRandom();
        image2.texture = textureList.PickRandom();
    }

    // Update is called once per frame
    void Update()
    {
        timeToChange += Time.deltaTime;

        if(timeToChange > 10f)
        {
            Fade();
        }
    }

	private void Fade()
	{
		Color c = image2.color;
		
		if (switchBool)
        {
            c.a -= 0.01f;
		}
        else 
        {
			c.a += 0.01f;
		}
		image2.color = c;

        if(c.a >= 1f || c.a <= 0f)
        {
			timeToChange = 0f; 
            
            if (switchBool)
			{
				image2.texture = textureList.PickRandom();
			}
			else
			{
				image1.texture = textureList.PickRandom();
			}

			switchBool = !switchBool;
		}
	}


}
