using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEventSetting : MonoBehaviour
{
    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;
    [SerializeField] Button loadtableButton;
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
        loadtableButton.onClick.AddListener(() =>
        {
            DataCenter.Instance.LoadCardDataTable();
        });
    }
}
