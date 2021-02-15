using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Runtime.InteropServices.WindowsRuntime;

[System.Serializable]
public class Volume
{
    public string name;
    public float volume;
    public float tempvolume;
}

public class Settings
{
    public static Profiles profile;
}

[CreateAssetMenu(menuName = "Create Profile")]
public class Profiles : ScriptableObject
{
    public bool saveInPlayerPrefs = true;
    public string prefPrefix = "Settings_";
    
    public AudioMixer audioMixer;
    public Volume[] volumeControl;

    public void SetProfile(Profiles profile)
    {
        Settings.profile = profile;
    }

    public float GetAudioLevels(string name)
    {
        float volume = 1f;

        if (!audioMixer)
        {
            Debug.LogWarning("There is no AudioMixer defined in the profiles file");
            return volume;
        }
        for (int i = 0; i < volumeControl.Length; i++)
        {
            if (volumeControl[i].name != name)
            {
                continue;
            }
            else
            {
                if (saveInPlayerPrefs)
                {
                    if (PlayerPrefs.HasKey(prefPrefix + volumeControl[i].name))
                    {
                        volumeControl[i].volume = PlayerPrefs.GetFloat(prefPrefix + volumeControl);
                    }
                }

                volumeControl[i].tempvolume = volumeControl[i].volume;

                //Apperantly this sets the volume and maybe the other MainMenu SetSlider is not needed
                if (audioMixer)
                {
                    audioMixer.SetFloat(volumeControl[i].name, Mathf.Log(volumeControl[i].volume) * 20f);
                }

                volume = volumeControl[i].volume;
                break;
            }
        }
        return volume;
    }


    public void SetAudioLevels(string name, float volume)
    {
        if (!audioMixer)
        {
            Debug.LogWarning("There is no AudioMixer defined in the profiles file");
            return;
        }

        for (int i = 0; i < volumeControl.Length; i++)
        {
            if (volumeControl[i].name != name)
            {
                continue;
            }
            else
            {
                audioMixer.SetFloat(volumeControl[i].name, Mathf.Log(volume) * 20);
                volumeControl[i].tempvolume = volume;
                break;
            }
        }
    }


    public void SaveAudioLevels()
    {
        if (!audioMixer)
        {
            Debug.LogWarning("There is no AudioMixer defined in the profiles file");
            return;
        }

        float volume = 0f;
        for (int i = 0; i < volumeControl.Length; i++)
        {
            volume = volumeControl[i].tempvolume;
            if (saveInPlayerPrefs)
            {
                PlayerPrefs.SetFloat(prefPrefix + volumeControl[i].name, volume);
            }
            audioMixer.SetFloat(volumeControl[i].name, Mathf.Log(volume) * 20);
            volumeControl[i].volume = volume;
        }
    }
}



