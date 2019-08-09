using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class OptionsScript : MonoBehaviour {

    [SerializeField]
    MenuManager menuManager;

    [SerializeField]
    InputField playerNameInputField;

    [SerializeField]
    Slider fxSlider;

    [SerializeField]
    Slider musicSlider;

    string playerName;
    Single fxVolume;
    Single musicVolume;

    public void OnEditPlayerName(string newName)
    {
        playerName = newName;
    }

    public void OnFxVolumeChanged(Single newVolume)
    {
        fxVolume = newVolume;
    }

    public void OnMusicVolumeChanged(Single newVolume)
    {
        musicVolume = newVolume;
    }

    public void OnTransitionIn()
    {
        fxSlider.value = fxVolume = GameSettings.FxVolume;
        musicSlider.value = musicVolume = GameSettings.MusicVolume = musicVolume;
        playerNameInputField.text = playerName = GameSettings.PlayerName;
    }

    public void OnCancel()
    {
        menuManager.PopCurrentMenu();
    }

    public void OnAccept()
    {
        GameSettings.PlayerName = playerName;
        GameSettings.FxVolume = fxVolume;
        GameSettings.MusicVolume = musicVolume;

        menuManager.SavePlayerPrefsAndPop();
    }
}

static class GameSettings
{
    const string kPlayerNameKey = "PlayerName";
    const string kMusicVolumeKey = "MusicVolume";
    const string kFxVolumeKey = "FxVolume";

    public static string PlayerName
    {
        get { return PlayerPrefs.GetString(kPlayerNameKey, "PlayerX"); }
        set { PlayerPrefs.SetString(kPlayerNameKey, value); }
    }

    public static float MusicVolume
    {
        get { return PlayerPrefs.GetFloat(kMusicVolumeKey, 1.0f); }
        set { PlayerPrefs.SetFloat(kMusicVolumeKey, value); }
    }

    public static float FxVolume
    {
        get { return PlayerPrefs.GetFloat(kFxVolumeKey, 1.0f); }
        set { PlayerPrefs.SetFloat(kFxVolumeKey, value); }
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }
}
