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
            // Resources ������ unityads-settings.json ���Ͽ��� Game ID�� �ε��մϴ�.
            TextAsset settingsText = Resources.Load<TextAsset>("unityads-settings");
            if (settingsText == null)
            {
                Debug.LogError("AdSettings ������ Resources���� ã�� �� �����ϴ� (Assets/Resources/unityads-settings.json)!");
                return;
            }

            AdSettings settings = JsonUtility.FromJson<AdSettings>(settingsText.text);

#if UNITY_ANDROID
            string gameId = settings.androidGameId;
#elif UNITY_IOS
            string gameId = settings.iosGameId;
#else
            // Editor�� �ٸ� �÷����� ��� �⺻������ Android ID�� ���
            string gameId = settings.androidGameId;
#endif

            if (string.IsNullOrEmpty(gameId))
            {
                Debug.LogError("Game ID�� �����Ǿ� ���� �ʽ��ϴ�!");
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
