using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    #region FMOD
    public static string AUDIO_MIXER_GROUP_MASTER = "MASTERVolume";
    public static string AUDIO_MIXER_GROUP_MUS = "MUSVolume";
    public static string AUDIO_MIXER_GROUP_SFX = "SFXVolume";

    public static string FMOD_BUS_MASTER = "bus:/";
    public static string FMOD_BUS_MUS = "bus:/MASTER/MUS";
    public static string FMOD_BUS_SFX = "bus:/MASTER/SFX";


    #region MUS

    public static string FMOD_EVENT_INSTANCE_MUS_MAIN_THEME = "event:/MUS/MAIN_THEME";

    #endregion

    #endregion

    #region Values
    public static float AUDIO_MIXER_DEFAULT_VOLUME = .5f;
    #endregion
}
