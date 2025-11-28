using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEventSetting : MonoBehaviour
{
    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;
    [SerializeField] Button loadtableButton;

    [SerializeField] Button firstDeckButton;
    [SerializeField] Button useDeckButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        saveButton.onClick.AddListener(() =>
        {
            SaveLoadManager.Instance.SetSaveData(DataCenter.Instance.userDeck);

            SaveLoadManager.Instance.SaveGame();
        });
        loadButton.onClick.AddListener(() =>
        {
            SaveLoadManager.Instance.LoadGame();
            DataCenter.Instance.userDeck = SaveLoadManager.Instance.GetCurrentData().userDeckData;
        });

        //firstDeckButton.onClick.AddListener(() => { GameData.Instance.FirstDeckSet(); });
        useDeckButton.onClick.AddListener(() => { GameData.Instance.NextDeckSet(1); });

        /*loadtableButton.onClick.AddListener(() =>
        {
            //DataCenter.GetItemData("11000001", (data) => {  });
            DataCenter.Instance.GetCardData("11000002", (data) =>
            {
                if(data == null) return;
                Debug.Log(data.itemName);
                Debug.Log(data.image.name);
                Debug.Log(data.effectDescription);
            });

            DataCenter.Instance.GetCardData("11000003", (data) =>
            {
                if (data == null) return;
                Debug.Log(data.itemName);
                Debug.Log(data.image.name);
                Debug.Log(data.effectDescription);
            });
        });*/
    }
}
