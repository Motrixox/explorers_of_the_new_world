using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";

    [Header("Content")]
    [SerializeField] private Text slotInfo;

    private Button saveSlotButton;

    private void Awake()
    {
        saveSlotButton = this.GetComponent<Button>();
    }
    public void SetData(GameData data)
    {
        if (data == null)
        {
            slotInfo.text = "no info";
        }
        else
        {
            slotInfo.text = "week " + (data.timeSinceStart / 60);
        }
    }

    public string GetProfileId()
    {
        return profileId;
    }

    public void SetInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
    }

    public void OnClick()
    {
        DataPersistenceManager.instance.NewGame();
        DataPersistenceManager.instance.ChangeSelectedProfileId(profileId);
        SceneManager.LoadSceneAsync("SampleScene");
    }
}
