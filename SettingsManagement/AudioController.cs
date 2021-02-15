using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    private Profiles m_profiles;

    [SerializeField]
    private List<Slider> m_volumeSliders = new List<Slider>();

    private void Awake()
    {
        if(m_profiles != null)
        {
            m_profiles.SetProfile(m_profiles);
        }
    }

    void Start()
    {
       if (Settings.profile && Settings.profile.audioMixer != null)
        {
           // Settings.profile.GetAudioLevels();
        }
    }


    public void ApplyChange()
    {
        if (Settings.profile && Settings.profile.audioMixer != null)
        {
            Settings.profile.SaveAudioLevels();
        }
    }

    public void CancelChangs()
    {
        if (Settings.profile)
           // Settings.profile.GetAudioLevels();
        for(int i = 0; i < m_volumeSliders.Count; i++)
        {
           // m_volumeSliders[i].ResetSliderValue();
        }

    }

}
