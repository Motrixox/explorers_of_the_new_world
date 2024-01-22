using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderExtension : MonoBehaviour
{
    public GameObject ValueGameObject;

	private void Awake()
	{
        var slider = gameObject.GetComponent<Slider>();

		if (slider == null) return;


        if (gameObject.name.Equals("Volume"))
            gameObject.GetComponent<Slider>().value = DataPersistenceManager.instance.gameOptions.audioVolume;

		if (gameObject.name.Equals("Sensitivity"))
			gameObject.GetComponent<Slider>().value = DataPersistenceManager.instance.gameOptions.cameraSensitivity;

		if (gameObject.name.Equals("Autosave"))
			gameObject.GetComponent<Slider>().value = DataPersistenceManager.instance.gameOptions.autosaveFrequencyMinutes;

	}

	public void ChangeValue()
    {
        if (ValueGameObject == null) return;

        if (ValueGameObject.GetComponent<Text>() == null) return;

        if (gameObject.GetComponent<Slider>() == null) return;

        if (gameObject.GetComponent<Slider>().value == 0f)
        {
            ValueGameObject.GetComponent<Text>().text = "OFF";
            return;
		}

        ValueGameObject.GetComponent<Text>().text = ((int)(gameObject.GetComponent<Slider>().value * 100) / 100.0).ToString();

	}

    public void Apply()
    {
        if (gameObject.name.Equals("Volume"))
            DataPersistenceManager.instance.gameOptions.audioVolume = gameObject.GetComponent<Slider>().value;

        if (gameObject.name.Equals("Sensitivity"))
            DataPersistenceManager.instance.gameOptions.cameraSensitivity = gameObject.GetComponent<Slider>().value;

        if (gameObject.name.Equals("Autosave"))
            DataPersistenceManager.instance.gameOptions.autosaveFrequencyMinutes = (int)gameObject.GetComponent<Slider>().value;

        DataPersistenceManager.instance.gameOptions.Apply();
	}
}
