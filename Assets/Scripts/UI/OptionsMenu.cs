using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musSlider;
    [SerializeField] private Slider sfxSlider;

    private void OnEnable()
    {
        SetSlidersValue();
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musSlider.onValueChanged.AddListener(SetMusVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void OnDisable()
    {
        masterSlider.onValueChanged.RemoveAllListeners();
        musSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
    }

    private void SetSlidersValue()
    {
        masterSlider.value = AudioManager.instance.GetVolume(VolumeGroup.MASTER);
        musSlider.value = AudioManager.instance.GetVolume(VolumeGroup.MUS);
        sfxSlider.value = AudioManager.instance.GetVolume(VolumeGroup.SFX);
    }

    private void SetMasterVolume(float volume)
    {
        AudioManager.instance.SetVolume(VolumeGroup.MASTER, volume);
    }

    private void SetMusVolume(float volume)
    {
        AudioManager.instance.SetVolume(VolumeGroup.MUS, volume);
    }

    private void SetSFXVolume(float volume)
    {
        AudioManager.instance.SetVolume(VolumeGroup.MUS, volume);
    }
}
