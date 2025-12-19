using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public box Box;

    [Header("Game IDs")]
    [SerializeField] private string _androidGameId;
    [SerializeField] private string _iOSGameId;
    [SerializeField] private bool _testMode = true;

    private string _gameId;

    [Header("Banner Ads")]
    [SerializeField] private string _androidBannerId = "Banner_Android";
    [SerializeField] private string _iOSBannerId = "Banner_iOS";
    [SerializeField] private BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;
    private string _bannerAdUnitId;

    [Header("Rewarded Ads")]
    [SerializeField] private string _androidRewardedId = "Rewarded_Android";
    [SerializeField] private string _iOSRewardedId = "Rewarded_iOS";
    private string _rewardedAdUnitId;
    private bool isRewardedAdReady = false;

    [Header("Interstitial Ads")]
    [SerializeField] private string _androidInterId = "Interstitial_Android";
    [SerializeField] private string _iOSInterId = "Interstitial_iOS";
    private string _interAdUnitId;
    private bool isInterAdReady = false;
    void Awake() => InitializeAds();

    #region Initialization
    private void InitializeAds()
    {
#if UNITY_IOS
        _gameId = _iOSGameId;
        _bannerAdUnitId = _iOSBannerId;
        _rewardedAdUnitId = _iOSRewardedId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
        _bannerAdUnitId = _androidBannerId;
        _rewardedAdUnitId = _androidRewardedId;
        _interAdUnitId = _androidInterId;

#else
        _gameId = _androidGameId; // Editor testing
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialized.");
        LoadBannerAd();
        LoadInterAd();
        LoadRewardedAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads Initialization Failed: {error} - {message}");
    }
    #endregion

    #region Banner Ads
    public void LoadBannerAd()
    {
        Advertisement.Banner.SetPosition(_bannerPosition);
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded, 
            errorCallback = OnBannerError
        };

        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(_bannerAdUnitId, options);
    }

    private IEnumerator RefreshBannerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Advertisement.Banner.Hide(false);
        LoadBannerAd();
    }
    private void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");
        Advertisement.Banner.Show(_bannerAdUnitId);
        StartCoroutine(RefreshBannerAfterDelay(35f));
    }

    private void OnBannerError(string message)
    {
        Debug.LogError($"Banner Load Error: {message}");
        // Retry could be added here if needed
        StartCoroutine(RefreshBannerAfterDelay(35f));
    }
    #endregion

    #region Rewarded Ads
    public void LoadRewardedAd()
    {
        Debug.Log("Loading Rewarded Ad...");
        Advertisement.Load(_rewardedAdUnitId, this);
    }

    public void ShowAd()
    {
        if (isRewardedAdReady)
        {
            isRewardedAdReady = false;
            Advertisement.Show(_rewardedAdUnitId, this);
            // Reload for next time
            LoadRewardedAd();
        }
        else
        {
            Debug.Log("Rewarded Ad not ready. Reloading...");
            LoadRewardedAd(); // attempt to reload
        }
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        if (adUnitId.Equals(_rewardedAdUnitId))
        {
            isRewardedAdReady = true;
            Debug.Log("Rewarded ad is ready.");
        }
        if (adUnitId.Equals(_interAdUnitId))
        {
            isInterAdReady = true;
            Debug.Log("Inter ad is ready.");
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Failed to load Ad Unit {adUnitId}: {error} - {message}");
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState state)
    {
        if (adUnitId == _rewardedAdUnitId && state == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("Rewarded Ad Completed");
            Box.reward();
        }
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Ad Show Failed: {adUnitId}: {error} - {message}");
        if (adUnitId.Equals(_rewardedAdUnitId))
        {
            // Reload rewarded ad
            LoadRewardedAd();
        }
        if (adUnitId.Equals(_interAdUnitId))
        {
            // Reload interstitial ad
            LoadInterAd();
        }
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
    #endregion

    #region Inter Ads
    public void LoadInterAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _interAdUnitId);
        Advertisement.Load(_interAdUnitId, this);
    }
    public void ShowInterAd()
    {
        // Note that if the ad content wasn't previously loaded, this method will fail
        Debug.Log("Showing Ad: " + _interAdUnitId);
        Advertisement.Show(_interAdUnitId, this);
        LoadInterAd();
    }

    #endregion
}