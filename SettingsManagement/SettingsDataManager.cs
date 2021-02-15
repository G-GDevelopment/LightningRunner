using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsDataManager : MonoBehaviour
{
    [Header("Options")]
    [SerializeField]
    private AudioMixer audioMixer;
    [Space]
    public bool speedRunClock; //Public becuase this is data that needs to be saved when the game is starting & if true the game will start with a timer
    public bool fullScreenMode = true; //If true then the game will be played in fullsreen
    public bool cameraShakeEffect = true; //If true then there will be camera shake effect in the game
    [Space]
    public float currentVolumeForMaster;
    public float currentVolumeForSoundFX;
    public float currentVolumeForMusic;
    [Space]
    [Header("New Saved Value")]
    public float newVolumeForMaster;
    public float newVolumeForSoundFX;
    public float newVolumeForMusic;
    [Space]
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundFXSlider;
    [Space]
    [Header("Toggle")]
    public Toggle cameraToggle;
    public Toggle fullScreenToggle;
    public Toggle timerToggle;

    // Start is called before the first frame update
    void Start()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/" + SaveManagerPlayerPreferences.instance.activeSettingsSave.saveName + ".preferencesSave"))
        {
            speedRunClock = SaveManagerPlayerPreferences.instance.activeSettingsSave.speedrunClock_Save;
            fullScreenMode = SaveManagerPlayerPreferences.instance.activeSettingsSave.fullScreenMode_Save;
            cameraShakeEffect = SaveManagerPlayerPreferences.instance.activeSettingsSave.cameraShake_Save;

            SpeedRunClockActivated(speedRunClock);
            SetFullScreen(fullScreenMode);
            CameraShakeEffect(cameraShakeEffect);

            //Slider Volume & Values
            newVolumeForMaster = SaveManagerPlayerPreferences.instance.activeSettingsSave.masterVolume;
            newVolumeForSoundFX = SaveManagerPlayerPreferences.instance.activeSettingsSave.soundFXVolume;
            newVolumeForMusic = SaveManagerPlayerPreferences.instance.activeSettingsSave.musicVolume;
            //Slider value change
            ChangeMasterVolume(newVolumeForMaster);
            ChangeMusicVolume(newVolumeForMusic);
            ChangeSoundFXVolume(newVolumeForSoundFX);
            //Graphic Toggle Value

            cameraToggle.isOn = cameraShakeEffect;
            fullScreenToggle.isOn = fullScreenMode;
            timerToggle.isOn = speedRunClock;
        }
    }

    #region OptionsMenu
    public void SetMasterVolume(float volume)
    {
  
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        currentVolumeForMaster = volume;

    }
    public void SetSoundFXVolume(float SoundsVolume)
    {
        audioMixer.SetFloat("soundsVolume", Mathf.Log10(SoundsVolume) * 20);

        currentVolumeForSoundFX = SoundsVolume;

    }

    public void SetMusicVolume(float musicVolume)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(musicVolume) * 20);

        currentVolumeForMusic = musicVolume;
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;

        fullScreenMode = isFullScreen;
    }

    public void SpeedRunClockActivated(bool _SpeedRunClock)
    {
        speedRunClock = _SpeedRunClock;

    }

    public void CameraShakeEffect(bool isCameraShakeEffect)
    {
        cameraShakeEffect = isCameraShakeEffect;
    }

    #endregion

    public void SaveSettingsData()
    {
        //Bool values
        SaveManagerPlayerPreferences.instance.activeSettingsSave.speedrunClock_Save = speedRunClock;
        SaveManagerPlayerPreferences.instance.activeSettingsSave.fullScreenMode_Save = fullScreenMode;
        SaveManagerPlayerPreferences.instance.activeSettingsSave.cameraShake_Save = cameraShakeEffect;

        //Slider aka the Float value
        SaveManagerPlayerPreferences.instance.activeSettingsSave.masterVolume = currentVolumeForMaster;
        SaveManagerPlayerPreferences.instance.activeSettingsSave.musicVolume = currentVolumeForMusic;
        SaveManagerPlayerPreferences.instance.activeSettingsSave.soundFXVolume = currentVolumeForSoundFX;
        //Slider Values
        SaveManagerPlayerPreferences.instance.activeSettingsSave.masterSliderValue = masterSlider.value;
        SaveManagerPlayerPreferences.instance.activeSettingsSave.soundFXSliderValue = soundFXSlider.value;
        SaveManagerPlayerPreferences.instance.activeSettingsSave.musicSliderValue = musicSlider.value;
        SaveManagerPlayerPreferences.instance.SavePlayerPreferences();
       
    }
    public void ChangeMasterVolume(float newVolume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(newVolume) * 20);
        masterSlider.value = SaveManagerPlayerPreferences.instance.activeSettingsSave.masterSliderValue;
    }
    public void ChangeMusicVolume(float newVolumeMusic)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(newVolumeMusic) * 20);
        musicSlider.value = SaveManagerPlayerPreferences.instance.activeSettingsSave.musicSliderValue;
    }

    public void ChangeSoundFXVolume(float newVolumeSounds)
    {
        audioMixer.SetFloat("soundsVolume", Mathf.Log10(newVolumeSounds) * 20);
        soundFXSlider.value = SaveManagerPlayerPreferences.instance.activeSettingsSave.soundFXSliderValue;
    }
}
