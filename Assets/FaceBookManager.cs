using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if Facebook
using Facebook.Unity;
#endif

public class FaceBookManager : MonoBehaviour
{
    public static FaceBookManager instance;
    // Awake function from Unity's MonoBehavior
#if Facebook
    void Awake()
    {
        GameAnalyticsSDK.GameAnalytics.Initialize();
        instance = this;
        DontDestroyOnLoad(gameObject);
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
    public void Log(float lvl,ulong score){
        var tutParams = new Dictionary<string, object>();
        tutParams["LVL Player"] = lvl.ToString();
        tutParams["LVL Score"] = score.ToString();

        FB.LogAppEvent(
            "Level passed",
            parameters: tutParams
        );
    }
#else
    void Awake()
    {
        GameAnalyticsSDK.GameAnalytics.Initialize();
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Log(float lvl, ulong score)
    {
    }
#endif
}
