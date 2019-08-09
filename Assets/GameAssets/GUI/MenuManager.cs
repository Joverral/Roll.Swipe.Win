using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour {

    [SerializeField]
    ScatterShotTransitionCanvas GameOverPanel;
    [SerializeField]
    ScatterShotTransitionCanvas SkipPanel;
    [SerializeField]
    ScatterShotTransitionCanvas PauseMenu;
    [SerializeField]
    ScatterShotTransitionCanvas OptionsMenu;
    [SerializeField]
    ScatterShotTransitionCanvas MainMenu;
    [SerializeField]
    ScatterShotTransitionCanvas Credits;
    [SerializeField]
    ScatterShotTransitionCanvas HUDPanel; // main game panel // OMG Unity and Interfaces is awful...this is terrilbe, but oh well

    [SerializeField]
    TutorialManager tutorialManager;

    [SerializeField]
    ScatterShotTransitionCanvas statusPanel;


    [SerializeField]
    Text VersionText;

    const int kMinPanelCount = 1;

    Stack<ITransitionCanvas> panelStack = new Stack<ITransitionCanvas>();

    private void PushPanel(ITransitionCanvas newPanel)
    {
        //if (panelStack.Count != kMinPanelCount && panelStack.Peek() == newPanel)
        //{
        //    PopCurrentMenu();
        //}
        //else
        {
            PauseGame();

            var baseNewPanel = (MonoBehaviour)newPanel;
            // slight kludge, since we never make them inactive
            baseNewPanel.gameObject.SetActive(true);

            if (panelStack.Count > 0)
            {
                panelStack.Peek().TransitionOut();
            }

            newPanel.TransitionIn();
            panelStack.Push(newPanel);
        }
    }

    void Start()
    {
        VersionText.text = "Version: " + Application.version;
        //HUDPanel.TransitionOut();
        panelStack.Push(HUDPanel); // note how we dont' use any fancy schmancy methods
        PushPanel(MainMenu);
    }

    public void PopCurrentMenu()
    {
        //Debug.Assert(panelStack.Count == kMinPanelCount);
        //Debug.Assert(panelStack.Peek() != HUDPanel, "We're trying to pop the HUD");

        panelStack.Pop().TransitionOut();
    
        if (panelStack.Count > 0)
        {
            panelStack.Peek().TransitionIn();

            if (panelStack.Count == kMinPanelCount)
            {
                UnpauseGame();
            }
        }
    }

    public void PopAll()
    {
        while (panelStack.Count != kMinPanelCount)
        {
            panelStack.Pop().TransitionOut();
        }

        panelStack.Peek().TransitionIn();

        UnpauseGame();
    }

    private void PauseGame()
    {
        tutorialManager.DisableTutorialManager();
        Time.timeScale = 0.0f;
    }

    private void UnpauseGame()
    {
        tutorialManager.EnableTutorialManager();
        Time.timeScale = 1.0f;
    }

    public void ShowMainMenu()
    {


#if (UNITY_ANDROID || UNITY_IOS)
        AdController.Current.ShowAd(null, (result) =>
        {
#endif
        PopAll();
        PushPanel(MainMenu);
#if (UNITY_ANDROID || UNITY_IOS)
        });
#endif
    }

    public void ShowGameOverMenu()
    {
        PushPanel(GameOverPanel);
    }

    public void ShowSkipMenu()
    {
        PushPanel(SkipPanel);
    }

    public void ShowPauseMenu()
    {
        PushPanel(PauseMenu);
    }

    public void ShowOptionsMenu()
    {
        PushPanel(OptionsMenu);
    }

    public void ShowCreditsMenu()
    {
        PushPanel(Credits);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadMainMenu()
    {

#if (UNITY_ANDROID || UNITY_IOS)
        AdController.Current.ShowAd(null, (result) =>
        {
#endif
            // We actually don't care if the ad succeeded, was watched or canceled
            SceneManager.LoadScene("StartScene");

#if (UNITY_ANDROID || UNITY_IOS)
        });
#endif
        
    }

    public void SavePlayerPrefsAndPop()
    {
        PlayerPrefs.Save();
        PopCurrentMenu();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && panelStack.Count == kMinPanelCount)
        {
            ShowPauseMenu();
        }
    }

    static public IEnumerator WaitForUnscaledSeconds(float timeToWait)
    {
        //hackery to wait on unscaled time...why this isnt' a yield type, I don't know
        for (float waitTime = timeToWait;
            waitTime > 0.0;
            waitTime -= Time.unscaledDeltaTime)
        {
            yield return null;
        }
    }

    IEnumerator ShowStatusMessageCoRoutine(float statusDisplayTime, bool useUnscaledTime)
    {
        this.statusPanel.gameObject.SetActive(true);
        this.statusPanel.TransitionIn();

        if (useUnscaledTime)
        {
            yield return StartCoroutine(WaitForUnscaledSeconds(statusPanel.TransitionTime + statusDisplayTime));
        }
        else
        {
            yield return new WaitForSeconds(statusPanel.TransitionTime + statusDisplayTime);
        }

        this.statusPanel.TransitionOut();

        currentDisplayMessage = null;
    }

    Coroutine currentDisplayMessage;
    public void DisplayInGameStatusMessage(string message, float displayTime, bool useUnscaledTime)
    {
        //TODO:  Store this, probably put this all on another script or whatever
        if (currentDisplayMessage == null)
        {
            this.statusPanel.GetComponentInChildren<Text>().text = message;
            currentDisplayMessage = StartCoroutine(ShowStatusMessageCoRoutine(displayTime, useUnscaledTime));
        }
        // TODO I should probably do something better...transition out, then start the new one
    }
}
