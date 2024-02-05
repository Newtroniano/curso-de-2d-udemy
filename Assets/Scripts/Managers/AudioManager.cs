using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private FMOD.Studio.EventInstance musAudioEvent;
    private FMOD.Studio.Bus masterBus;
    private FMOD.Studio.Bus musBus;
    private FMOD.Studio.Bus sfxBus;
    private float masterBusVolume;
    private float musBusVolume;
    private float sfxBusVolume;
    private string lastSong;


    private void Awake()
    {
        if (instance == null) { instance = this; }
        masterBus = RuntimeManager.GetBus(Constants.FMOD_BUS_MASTER);
        musBus = RuntimeManager.GetBus(Constants.FMOD_BUS_MUS);
        sfxBus = RuntimeManager.GetBus(Constants.FMOD_BUS_SFX);
    }

    private void Start()
    {
        SetStoredVolume();
        SwitchMusic(Constants.FMOD_EVENT_INSTANCE_MUS_MAIN_THEME);
    }

    public void PlayMusic(string audioClip)
    {
        lastSong = audioClip;
        musAudioEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musAudioEvent = RuntimeManager.CreateInstance(audioClip);
        musAudioEvent.start();
    }

    public void SwitchMusic(string musicEvent)
    {
        if (musicEvent != lastSong)
            PlayMusic(musicEvent);
    }

    private void SetStoredVolume()
    {
        masterBusVolume = PlayerPrefs.GetFloat(Constants.AUDIO_MIXER_GROUP_MASTER, Constants.AUDIO_MIXER_DEFAULT_VOLUME);
        musBusVolume = PlayerPrefs.GetFloat(Constants.AUDIO_MIXER_GROUP_MUS, Constants.AUDIO_MIXER_DEFAULT_VOLUME);
        sfxBusVolume = PlayerPrefs.GetFloat(Constants.AUDIO_MIXER_GROUP_SFX, Constants.AUDIO_MIXER_DEFAULT_VOLUME);

        masterBus.setVolume(masterBusVolume);
        musBus.setVolume(musBusVolume);
        sfxBus.setVolume(sfxBusVolume);
    }

    public void SetVolume(VolumeGroup volumeGroup, float value)
    {
        switch (volumeGroup)
        {
            case (VolumeGroup.MASTER):
                masterBus.setVolume(value);
                masterBusVolume = value;
                PlayerPrefs.SetFloat(Constants.AUDIO_MIXER_GROUP_MASTER, value);
                break;
            case (VolumeGroup.MUS):
                musBus.setVolume(value);
                musBusVolume = value;
                PlayerPrefs.SetFloat(Constants.AUDIO_MIXER_GROUP_MUS, value);
                break;
            case (VolumeGroup.SFX):
                sfxBus.setVolume(value);
                sfxBusVolume = value;
                PlayerPrefs.SetFloat(Constants.AUDIO_MIXER_GROUP_SFX, value);
                break;
        }
    }

    public float GetVolume(VolumeGroup volumeGroup)
    {
        switch (volumeGroup)
        {
            case VolumeGroup.MASTER:
                return masterBusVolume;
            case VolumeGroup.MUS:
                return musBusVolume;
            case VolumeGroup.SFX:
                return sfxBusVolume;
            default:
                return 0;
        }
    }
}

public enum VolumeGroup
{
    MASTER,
    MUS,
    SFX
}

