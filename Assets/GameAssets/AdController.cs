using System;
using System.Collections;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Advertisements;

public class AdController : MonoBehaviour {

    public Button rewardButton;

    // TODO: This is laziness
    public static AdController Current;         //A public static reference to itself (make's it visible to other objects without a reference)

    const int kFinalFailCount = 3;

    void Awake()
    {
        //Ensure that there is only one
        if (Current == null)
        {
            Current = this;
        }
        else
        {
            Debug.LogError("Attempting to spawn a second AdController!");
            Destroy(gameObject);
        }
    }

#if (UNITY_ANDROID || UNITY_IOS)
    void Start()
    {
        StartCoroutine(EnableAdButton());
    }

    public IEnumerator EnableAdButton()
    {
        while (!Advertisement.isInitialized || !Advertisement.IsReady())
        {
            yield return new WaitForSeconds(0.5f);
        }

        rewardButton.gameObject.SetActive(true);
    }

    IEnumerator ShowAdCoroutine(string adZone, Action<ShowResult> actionHandleResult)
    {
        int count = 0;
        while ( (!Advertisement.isInitialized || !Advertisement.IsReady(adZone)) && count < kFinalFailCount )
        {
            yield return new WaitForSeconds(0.5f);
            count++;
        }

        if (Advertisement.IsReady(adZone))
        {
            var options = new ShowOptions { resultCallback = actionHandleResult };
            Time.timeScale = 0.0f;
            Advertisement.Show(adZone, options);
        }
        else if(count ==  kFinalFailCount)
        {
            // we've failed, this lets people play with no wifi, they just get no rewards
            actionHandleResult.Invoke(ShowResult.Failed);
        }
    }

    public void ShowAd(string adZone, Action<ShowResult> action)
    {
        StartCoroutine(ShowAdCoroutine(adZone, action));
    }
#endif
}


