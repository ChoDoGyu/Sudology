using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string bannerPlacement = "Banner_Android";

    void Start()
    {
        // ��� ��ġ ���� (�ϴ� �߾�)
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        // ��� �ε� ��û
        Advertisement.Banner.Load(bannerPlacement, new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        });
    }

    void OnBannerLoaded()
    {
        Debug.Log("Banner Loaded");
        Advertisement.Banner.Show(bannerPlacement);
    }

    void OnBannerError(string message)
    {
        Debug.LogError("Banner Load Error: " + message);
    }

    // (IUnityAdsShowListener �������̽� �޼��� �� ����)
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) { }
    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) { }
    public void OnUnityAdsAdLoaded(string placementId) { }
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) { }
}
