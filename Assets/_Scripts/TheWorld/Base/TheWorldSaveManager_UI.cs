using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TheWorldSaveManager_UI : MonoBehaviour
{
    public TheWorldSaveManager theWorldSaveManager;
    public TMP_Dropdown saveSlotDropdown;
    public Button refreshJsonFileListButton;
    public Button saveButton;
    public Button loadButton;
    public TMP_InputField customFileNameInput;
    public Toggle useGZipToggle;

    private void Start()
    {
        InitializeUI();
        RefreshJsonFileList();
    }

    private void InitializeUI()
    {
        saveSlotDropdown.onValueChanged.AddListener(OnSaveSlotSelected);
        refreshJsonFileListButton.onClick.AddListener(RefreshJsonFileList);
        saveButton.onClick.AddListener(SaveWorldToJson);
        loadButton.onClick.AddListener(LoadWorldFromJson);
        /*customFileNameInput.onEndEdit.AddListener(OnCustomFileNameInput);*/
        useGZipToggle.onValueChanged.AddListener(OnUseGZipToggle);
    }

    private void OnSaveSlotSelected(int index)
    {
        theWorldSaveManager.ReadSaveFolderPath = saveSlotDropdown.options[index].text;
    }

    private void RefreshJsonFileList()
    {
      /*  theWorldSaveManager.RefreshSaveFileList();*/
        saveSlotDropdown.ClearOptions();
    }

    private void OnUseGZipToggle(bool isOn)
    {
        theWorldSaveManager.IsGZip = isOn;
    }

    private void SaveWorldToJson()
    {
        theWorldSaveManager.SaveWorldToDisk();
    }

    private void LoadWorldFromJson()
    {
        theWorldSaveManager.LoadWorldFromDiskSave();
    }
}