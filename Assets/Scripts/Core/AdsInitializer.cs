using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] private string androidGameId = "YOUR_ANDROID_GAME_ID"; // ∫ª¿Œ ID
    [SerializeField] private bool testMode = true;

    void Awake()
    {
        if (!Advertisement.isInitialized)
            Advertisement.Initialize(androidGameId, testMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Ads Initialization Complete");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Ads Init Failed: {error} - {message}");
    }
}
