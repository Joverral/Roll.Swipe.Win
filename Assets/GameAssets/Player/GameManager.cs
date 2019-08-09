using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.SocialPlatforms;

public class GameManager : MonoBehaviour {

    [SerializeField]
    bool DestroyTracksOnDeath = true;

    [SerializeField]
    TransitionCanvas coinTutorialElement;  //  Yeah this system is spaghetti terrible

    [SerializeField]
    Text debugText;

    const string socialLeaderboardID = "CgkIm7-ZseEMEAIQAQ";

    LevelGenerator levelGenerator;
    ShapeController shapeController;
    SwipeController swipeController;
    LocalHighScore localHighScores;

    [SerializeField]
    float MaxPlayerSpeed = 50;
    float maxPlayerSpeedSquared;
    Rigidbody2D playerBody;

    [SerializeField]
    int skipTimePenalty = 10;

    int numCoinsPickedUpThisLevel = 0;
    int coinScore = 1;
    int levelScore = 10;

    System.TimeSpan LevelTime = System.TimeSpan.FromMinutes(1.5);
    System.TimeSpan timeRemaining;

    [SerializeField]
    Text timeText;
    [SerializeField]
    Text levelText;
    [SerializeField]
    Text ScoreText;

    [SerializeField]
    MenuManager menuManager;

    [SerializeField]
    Canvas ScreenCanvas;

    [SerializeField]
    GameObject CoinDisplayPrefab;

    [SerializeField]
    GameObject particlePrefab;

    [SerializeField]
    GameObject[] vortexMeteors;
    [SerializeField]
    int meteorPerLevel = 6;

    int currentLevel = 0;
    int currentScore = 0;

    // Kludgey optimization to remove garbage collection
    WaitForSeconds waitForTenthSecond = new WaitForSeconds(0.1f);
    WaitForSeconds waitForSecond = new WaitForSeconds(1);

    Coroutine timerCoroutine = null;
    Coroutine blinkRoutine;

    [SerializeField]
    TutorialManager tutorialManager;

    // tempted to move all the audio stuff out. 
    // and move to an event manager system, instead of silly hard coded-ness
    AudioSource[] audioSource;

    [SerializeField]
    AudioClip collisionStartClip;
    [SerializeField]
    AudioClip collisionLoopClip;
    [SerializeField]
    AudioClip collisionEndClip;

    uint numCollisionStays = 0;

    [SerializeField]
    AudioClip coinPickupClip;
    AudioSource coinSource;

    [SerializeField]
    AudioClip gameOverClip;
    AudioSource gameOverSource;

    [SerializeField]
    AudioClip deathZoneClip;
    AudioSource deathZoneSource;

    [SerializeField]
    AudioClip[] bgMusicClip;
    int currentBgClip = 0;

    AudioSource bgMusicSource;
    public AnimationCurve bgCurve;

    public float bgMinPitch = 1.0f;
    public float bgMaxPitch = 3.0f;

    [SerializeField]
    AudioClip goalReached;

    AudioSource goalSource;

    void Awake()
    {
        audioSource = new AudioSource[3];
        for (int i = 0; i < audioSource.Length; ++i)
        {
            audioSource[i] = this.CreateAudioSource();
            audioSource[i].volume = 0.05f; // TODO multiply by master volume
        }

        bgMusicSource = this.CreateAudioSource();
        coinSource = this.CreateAudioSource();

        gameOverSource = this.CreateAudioSource();
        gameOverSource.clip = this.gameOverClip;

        deathZoneSource = this.CreateAudioSource();
        deathZoneSource.clip = this.deathZoneClip;

        goalSource = this.CreateAudioSource();
        goalSource.pitch = 2.0f;
    }

    // Use this for initialization
    void Start()
    {

        Screen.orientation = (ScreenOrientation)PlayerPrefs.GetInt("ScreenOrientation", (int)ScreenOrientation.LandscapeLeft);

        shapeController = GetComponent<ShapeController>();
        swipeController = GetComponent<SwipeController>();
        playerBody = GetComponent<Rigidbody2D>();
        localHighScores = GetComponent<LocalHighScore>();
        levelGenerator = GetComponent<LevelGenerator>();
        levelGenerator.AddAllPrefabsToPooler();
        maxPlayerSpeedSquared = MaxPlayerSpeed * MaxPlayerSpeed;
        //Restart();

        StartCoroutine(MoveTowardsGoalCoroutine());
    }

    void FixedUpdate()
    {
        // TODO:  Look into damping instead
        if (playerBody.velocity.sqrMagnitude > maxPlayerSpeedSquared)
        {
            playerBody.velocity = playerBody.velocity.normalized * MaxPlayerSpeed;
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            GameOver();
        }
    }

    private void UpdateTimerText()
    {
        timeText.text = string.Format("{0:00}:{1:00}",
            timeRemaining.Minutes, timeRemaining.Seconds); //sadly Unity doesn't have the later implementation of timespan
    }

    private void UpdateScoreText()
    {
        ScoreText.text = "Score: " + currentScore;
    }

    IEnumerator MoveTowardsGoalCoroutine()
    {
        var constantForce = GetComponent<ConstantForce2D>();
        var forwardTorque = constantForce.torque;
        var reverseTorque = -constantForce.torque;

        while (constantForce != null)
        {
            yield return waitForSecond;

            if (gameObject.transform.position.x > levelGenerator.Goal.transform.position.x)
            {
                Debug.Log("Going in reverse!");
                constantForce.torque = reverseTorque;
            }
            else
            {
                constantForce.torque = forwardTorque;
            }
        }
    }

    IEnumerator TimerBlinkCoroutine()
    {
        while (timeRemaining.TotalSeconds >= 0.0f)
        {
            timeText.color = Color.Lerp(Color.red, Color.yellow, Mathf.PingPong(Time.time * 5, 1.0f));
            yield return null;
        }
    }

    IEnumerator TimerCoroutine()
    {
        while (timeRemaining.TotalMilliseconds > 0.0f)
        {
            timeRemaining -= System.TimeSpan.FromSeconds(1);
            var secondsLeft = timeRemaining.TotalSeconds;
            var SecondsAsTValue = (float)(secondsLeft / LevelTime.TotalSeconds);

            var bgT = bgCurve.Evaluate(1.0f - SecondsAsTValue);
            bgMusicSource.pitch = Mathf.Lerp(bgMinPitch, bgMaxPitch, bgT);
            //Debug.Log("T: " + bgT + " Pitch: " + bgMusicSource.pitch);

            if (secondsLeft < 10.0)
            {
                if (blinkRoutine == null)
                {
                    blinkRoutine = StartCoroutine(TimerBlinkCoroutine());
                }
            }
            else
            {
                if (blinkRoutine != null)
                {
                    StopCoroutine(blinkRoutine);
                    blinkRoutine = null;
                }

                if (SecondsAsTValue < .5)
                {
                    timeText.color = Color.Lerp(Color.red, Color.yellow, (SecondsAsTValue * 2));
                }
                else {
                    timeText.color = Color.Lerp(Color.yellow, Color.green, (SecondsAsTValue - .5f) * 2);
                }
            }

            UpdateTimerText();
            yield return waitForSecond;
        }

        GameOver();
        timerCoroutine = null;
    }

    IEnumerator AddTime(int amount)
    {
        Debug.Assert(amount > 0, "Positive amounts only!");
        while (amount > 0)
        {
            timeRemaining += System.TimeSpan.FromSeconds(1);
            amount--;
            UpdateTimerText();
            yield return waitForTenthSecond;
        }
    }

    IEnumerator RemoveTime(int positiveAmount)
    {
        while (positiveAmount > 0)
        {
            timeRemaining -= System.TimeSpan.FromSeconds(1);
            positiveAmount--;
            UpdateTimerText();
            yield return waitForTenthSecond;
        }
    }

    IEnumerator AddScore(int amount)
    {
        while (amount > 0)
        {
            currentScore++;
            amount--;
            UpdateScoreText();
            yield return waitForTenthSecond;
        }
    }

    public void GenerateLevel()
    {
        if (coinTutorialElement 
            && !tutorialManager.IsEmpty()
            && numCoinsPickedUpThisLevel < 2)
        {
            tutorialManager.PushPanel(coinTutorialElement);
        }

        numCoinsPickedUpThisLevel = 0;
        coinScore = 1;
        currentLevel++;
        levelText.text = "Level: " + currentLevel.ToString();


        shapeController.ClearAllShapes();
        swipeController.Clear();
        
        levelGenerator.GenerateLevel(currentLevel);

        for(int i = 0; i < vortexMeteors.Length; i++)
        {
            vortexMeteors[i].SetActive( i < (currentLevel / meteorPerLevel) );
        }
    }


    IEnumerator PlayGameOverEffectCoroutine()
    {
        var imageEffect = Camera.main.GetComponent<RadialGrayScaleEffect>();
        Time.timeScale = 0.0f; // pause game

        imageEffect.enabled = true;
        imageEffect.center = Camera.main.WorldToViewportPoint(this.transform.position);


        const float kTotalEffectTime = 1.5f;

        float timeLeft = kTotalEffectTime;
        // play 
        while (timeLeft > 0.0)
        {
            timeLeft -= Time.unscaledDeltaTime;
            float t = 1.0f - timeLeft / kTotalEffectTime;
            imageEffect.radius = new Vector2(t, t);
            yield return null;
        }

        yield return StartCoroutine(MenuManager.WaitForUnscaledSeconds(0.5f));

        imageEffect.enabled = false;
        Time.timeScale = 1.0f; // unpause game

        string name = GameSettings.PlayerName;
        if (Social.localUser.authenticated)
        {
            name = Social.localUser.userName;
        }

        localHighScores.DisplayScores(localHighScores.AddScore(name, currentLevel, currentScore));

        LogIn();

        bgMusicSource.Stop();
     
        menuManager.ShowGameOverMenu();
    }

    public void GameOver()
    {
        gameOverSource.Play();
        menuManager.DisplayInGameStatusMessage("Out of time!", 2.0f, true);

        swipeController.Clear();

        GameObjectPooler.Current.PoolAllObjects();

        StartCoroutine(PlayGameOverEffectCoroutine());

        //string name = GameSettings.PlayerName;
        //if (Social.localUser.authenticated)
        //{
        //    name = Social.localUser.userName;
        //}

        //localHighScores.DisplayScores(localHighScores.AddScore(name, currentLevel, currentScore));

        //LogIn();
        
        //bgMusicSource.Stop();

        //menuManager.ShowGameOverMenu();
    }

    public void LogIn()
    {
#if (UNITY_ANDROID || UNITY_IOS)
        GooglePlayGames.PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        GooglePlayGames.PlayGamesPlatform.Activate();
#endif

        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate(ProcessAuthentication);
        }
        else
        {
            ReportScore(currentScore, socialLeaderboardID);
           
        }

    }

    void ReportScore(long score, string leaderboardID)
    {
        Debug.Log("Reporting score " + score + " on leaderboard " + leaderboardID);
        Social.ReportScore(score, leaderboardID, success => 
        {
            Debug.Log(success ? "Reported score successfully" : "Failed to report score");
        });

        Social.Active.ReportScore(currentScore, leaderboardID, success =>
        {
            Debug.Log(success ? "Reported Active score successfully" : "Failed to report score");
            if(success)
            {
                ShowLeaderBoardUI();
            }
        });
    }

    // This function gets called when Authenticate completes
    // Note that if the operation is successful, Social.localUser will contain data from the server. 
    void ProcessAuthentication(bool success)
    {
        if (success)
        {
            ReportScore(currentScore, socialLeaderboardID);
        }
        else
        {
            debugText.text += " Failed to authenticate";

            Debug.Log("Failed to authenticate");
            
        }
    }

    public void ShowLeaderBoardUI()
    {
        debugText.text += " Attempting to show active leaderboard UI !";
        //Social.Active.ShowLeaderboardUI();

#if (UNITY_ANDROID || UNITY_IOS)
        GooglePlayGames.PlayGamesPlatform.Instance.ShowLeaderboardUI(socialLeaderboardID);
#endif
    }

    public void ShowCheevosUI()
    {
        Social.LoadAchievements(ProcessLoadedAchievements);
        Social.Active.ShowAchievementsUI();
    }

    // This function gets called when the LoadAchievement call completes
    void ProcessLoadedAchievements(IAchievement[] achievements)
    {
        if (achievements.Length == 0)
        {
            debugText.text += " Error: no achievements found";
            Debug.Log("Error: no achievements found");
        }
        else
        {
            debugText.text += " Got " + achievements.Length + " achievements";
            Debug.Log("Got " + achievements.Length + " achievements");
        }

        //// You can also call into the functions like this
        //Social.ReportProgress("Achievement01", 100.0, result => 
        //{
        //    if (result)
        //    {

        //        Debug.Log("Successfully reported achievement progress");
        //    }
        //    else
        //    {
        //        Debug.Log("Failed to report achievement");
        //    }
        //});
    }

    IEnumerator playBgMusic()
    {
        //
        while (timeRemaining.TotalMilliseconds > 0.0f)
        {
            bgMusicSource.clip = bgMusicClip[currentBgClip];
            bgMusicSource.Play();
            bgMusicSource.volume = GameSettings.MusicVolume;
            yield return new WaitForSeconds(bgMusicSource.clip.length);

            if(++currentBgClip == bgMusicClip.Length)
            {
                currentBgClip = 0;
            }
        }
    }

    public void Restart()
    {
        currentLevel = 0;
        currentScore = 0;

        menuManager.PopAll();
        UpdateScoreText();

        Time.timeScale = 1.0f;
        GenerateLevel();

        timeRemaining = LevelTime;

        if (timerCoroutine == null)
        {
            Debug.Log("Starting Timer Coroutine!");
            timerCoroutine = StartCoroutine(TimerCoroutine());
        }

        bgMusicSource.clip = bgMusicClip[0];
        //bgMusicSource.loop = true;
        bgMusicSource.pitch = 1.0f;

        //bgMusicSource.Play();
        StartCoroutine(playBgMusic());
    }

    IEnumerator PlayTwirlShader()
    {
        var twirlEffect = Camera.main.GetComponent<VortexEffect>();
        Time.timeScale = 0.0f; // pause game

        var tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        // We should only read the screen buffer after rendering is complete
        yield return new WaitForEndOfFrame();

        // Note To Self:  I tried doing this in OnEnabled and inside the OnRender, neither worked
        // Read screen contents into the texture

        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        tex.Apply();
        twirlEffect.PassTheTexture(tex);
        twirlEffect.enabled = true;

        twirlEffect.center = Camera.main.WorldToViewportPoint(this.transform.position);

        GenerateLevel();

        const float kTotalTwirlTime = 0.5f;

        float timeToTwirl = kTotalTwirlTime;
        // play 
        while(timeToTwirl > 0.0)
        {
            timeToTwirl -= Time.unscaledDeltaTime;

            twirlEffect.angle = Mathf.Lerp(0.0f, 360.0f, 1.0f - timeToTwirl / kTotalTwirlTime);
            yield return null;
        }

        twirlEffect.enabled = false;
        Time.timeScale = 1.0f; // unpause game

        StartCoroutine(AddScore(levelScore + currentLevel));

        menuManager.DisplayInGameStatusMessage("Level " + this.currentLevel, 2.0f, false);

        // Hackery to prevent repeat collisions (waiting a single frame wasn't enough..just fudging it and going half a second
        yield return new WaitForSeconds(0.5f);
        GoalTransitionCoroutine = null;
    }

    Coroutine GoalTransitionCoroutine;
    void HitGoal()
    {
        if (GoalTransitionCoroutine == null)
        {
            goalSource.PlayOneShot(goalReached);

            // TODO:  Play Vortex Effect on Camera
            GoalTransitionCoroutine = StartCoroutine(PlayTwirlShader());

         
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject == levelGenerator.Goal)
        {
            HitGoal();
        }
        else
        {
            SpawnParticle(coll);
            numCollisionStays++;
            if (!audioSource[0].isPlaying && !audioSource[1].isPlaying)
            {
                audioSource[0].PlayOneShot(collisionStartClip);
            }
        }
    }

    [SerializeField]
    float ParticleDelay = 0.2f;

    float currentDelay = 0.0f;

    void SpawnParticle(Collision2D coll)
    {
        currentDelay += Time.deltaTime;

        if (currentDelay < ParticleDelay)
            return;

        currentDelay = 0.0f;

        var randomRot = Quaternion.Euler(0.0f, 0.0f, Random.Range(-10.0f, 30.0f));

        var particle = GameObjectPooler.Current.GetObject(particlePrefab);
        particle.transform.position = coll.contacts[0].point;
        //particle.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        particle.GetComponent<Rigidbody2D>().AddForce(randomRot * -GetComponent<Rigidbody2D>().velocity, ForceMode2D.Impulse);
        particle.GetComponent<Rigidbody2D>().AddTorque(-GetComponent<Rigidbody2D>().angularVelocity, ForceMode2D.Impulse);

    }

    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject == levelGenerator.Goal)
        {
            HitGoal();
        }
        else
        {
            SpawnParticle(coll);
            numCollisionStays++;
        }
    }

    void Update()
    {
        if(numCollisionStays > 0)
        {
            if(!audioSource[1].isPlaying)
            {
                audioSource[1].loop = true;
                audioSource[1].clip = collisionLoopClip;
                audioSource[1].loop = true;
                audioSource[1].Play();
                audioSource[1].loop = true;
            }
        }
        else if(audioSource[1].isPlaying)
        {
            audioSource[1].Stop();
            audioSource[2].PlayOneShot(collisionEndClip);
        }

        numCollisionStays = 0;
    }

    void OnCoinPickup(GameObject coin)
    {
        if (coinTutorialElement && tutorialManager.IsTop(coinTutorialElement) && numCoinsPickedUpThisLevel > 2)
        {
            tutorialManager.PopCurrentMenu();
        }

        var newCoinDisplay = GameObjectPooler.Current.GetObject(CoinDisplayPrefab);


        var coinRectTransform = newCoinDisplay.GetComponent<RectTransform>();
        coinRectTransform.SetParent(ScreenCanvas.GetComponent<RectTransform>());
        coinRectTransform.position = new Vector2(coin.transform.position.x, coin.transform.position.y); // Note the sneaky Vec2 to get rid of the Z
        coinRectTransform.localScale = Vector3.one;

        newCoinDisplay.GetComponent<CoinScoreDisplay>().endTransform = timeText.GetComponent<RectTransform>();
        newCoinDisplay.GetComponent<Text>().text = "+" + coinScore;

        StartCoroutine(AddScore(coinScore));
        StartCoroutine(AddTime(coinScore));

        coinScore++;
        numCoinsPickedUpThisLevel++;

        coinSource.PlayOneShot(coinPickupClip);
    }

    void OnEnterDeathZone()
    {
        deathZoneSource.Play();

        levelGenerator.Player.transform.position = levelGenerator.PlayerStartPosition;
        levelGenerator.Player.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
        levelGenerator.Player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;


        // Sigh, i really should move to an event system to prevent all this spaghetti
        if (DestroyTracksOnDeath)
        {
            swipeController.DecayAll();
        }

        coinScore = 1;
    }

    public void OnRetry()
    {
        
#if (UNITY_ANDROID || UNITY_IOS)
        AdController.Current.ShowAd(null, (result) =>
        {
#endif
            // We actually don't care if the ad succeeded, was watched or canceled
            menuManager.PopAll();
            Restart();
#if (UNITY_ANDROID || UNITY_IOS)
        });
#endif
        }

    public void OnSkipViaAd()
    {
        
#if (UNITY_ANDROID || UNITY_IOS)
        
        AdController.Current.ShowAd("rewardedVideo", (result) =>
        {
            // Only perform Skip on ad completion
            switch (result)
            {
                case ShowResult.Finished:
                    GenerateLevel();
                    break;
                case ShowResult.Skipped:
                    Debug.Log("The ad was skipped before reaching the end.");
                    break;
                case ShowResult.Failed:
                    Debug.LogError("The ad failed to be shown.");
                    //Not their fault, restart anyway
                    SkipLevel();
                    break;
            }

            menuManager.PopAll();
        });
#endif
    }

    public void OnSkipViaTimePenalty()
    {
        StartCoroutine(RemoveTime(skipTimePenalty));
        SkipLevel();
    }

    private void SkipLevel()
    {
        GenerateLevel();
        menuManager.PopAll();
    }

    public bool CanSkip()
    {
        return timeRemaining.TotalSeconds > skipTimePenalty;
    }

   
}

public static class MonoBehaviourExtensions
{
    public static AudioSource CreateAudioSource(this MonoBehaviour b)
    {
        return b.gameObject.AddComponent<AudioSource>();
    }
}