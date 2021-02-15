using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


public class SaveManagerPlayerPreferences : MonoBehaviour
{
    public static SaveManagerPlayerPreferences instance;

    public SavingSettingsData activeSettingsSave;

    public bool hasLoaded;

    private void Awake()
    {
        instance = this;

        LoadPlayerPreferences();
    }

    public void SavePlayerPreferences()
    {
        string dataPath = Application.persistentDataPath;
        //Save location
        var serializer = new XmlSerializer(typeof(SavingSettingsData));
        var stream = new FileStream(dataPath + "/" + activeSettingsSave.saveName + ".preferencesSave", FileMode.Create);
        serializer.Serialize(stream, activeSettingsSave);

        Debug.Log("Saved");
    }

    public void LoadPlayerPreferences()
    {
        string dataPath = Application.persistentDataPath;

        if (System.IO.File.Exists(dataPath + "/" + activeSettingsSave.saveName + ".preferencesSave"))
        {
            //Get the files out of the folder
            var serializer = new XmlSerializer(typeof(SavingSettingsData));
            var stream = new FileStream(dataPath + "/" + activeSettingsSave.saveName + ".preferencesSave", FileMode.Open);

            activeSettingsSave = serializer.Deserialize(stream) as SavingSettingsData;
            stream.Close(); 

            Debug.Log("Loaded");

            hasLoaded = true;
        }
    }

    public void DeleteSaveData()
    {
        string dataPath = Application.persistentDataPath;
        if (System.IO.File.Exists(dataPath + "/" + activeSettingsSave.saveName + ".preferencesSave"))
        {
            //Delete saved Data
            File.Delete(dataPath + "/" + activeSettingsSave.saveName + ".preferencesSave");
            

            Debug.Log("Saved Deleted");
        }
    }
}

[System.Serializable]
public class SavingSettingsData
{
    public string saveName;
    
    public float masterVolume;
    public float musicVolume;
    public float soundFXVolume;

    public bool speedrunClock_Save;
    public bool fullScreenMode_Save;
    public bool cameraShake_Save;

    public float masterSliderValue;
    public float musicSliderValue;
    public float soundFXSliderValue;

}