using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Advertisements;

[Serializable]
public class AdSettings
{
    public string androidGameId;
    public string iosGameId;
}

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] private bool testMode = true;

    void Awake()
    {
        if (!Advertisement.isInitialized)
        {
            // Resources 폴더의 unityads-settings.json 파일에서 Game ID를 로드합니다.
            TextAsset settingsText = Resources.Load<TextAsset>("unityads-settings");
            if (settingsText == null)
            {
                Debug.LogError("AdSettings 파일을 Resources에서 찾을 수 없습니다 (Assets/Resources/unityads-settings.json)!");
                return;
            }

            AdSettings settings = JsonUtility.FromJson<AdSettings>(settingsText.text);

#if UNITY_ANDROID
            string gameId = settings.androidGameId;
#elif UNITY_IOS
            string gameId = settings.iosGameId;
#else
            // Editor나 다른 플랫폼일 경우 기본적으로 Android ID를 사용
            string gameId = settings.androidGameId;
#endif

            if (string.IsNullOrEmpty(gameId))
            {
                Debug.LogError("Game ID가 설정되어 있지 않습니다!");
                return;
            }

            Advertisement.Initialize(gameId, testMode, this);
        }
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
