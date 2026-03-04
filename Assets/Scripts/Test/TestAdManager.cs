using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BlackHole.Test
{
    public class TestAdManager : MonoBehaviour
    {
        [SerializeField] private Button loadBannerAdButton;
        [SerializeField] private Button showBannerAdButton;
        [SerializeField] private Button loadInterstitialAdButton;
        [SerializeField] private Button showInterstitialAdButton;
        
        private readonly string _adUnitId = "ca-app-pub-3940256099942544/6300978111";
        private readonly string _interestialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
        
        private BannerView _bannerView;
        private InterstitialAd _interstitialAd;
        
        private void Awake()
        {
            MobileAds.Initialize((initializationStatus) =>
            {
                if (initializationStatus == null)
                {
                    Debug.LogError("Google Mobile Ads initialization failed with code " + initializationStatus);
                    return;
                }

                Debug.Log("Google Mobile Ads initialization complete.");

                // Google Mobile Ads events are raised off the Unity Main thread. If you need to
                // access UnityEngine objects after initialization,
                // use MobileAdsEventExecutor.ExecuteInUpdate(). For more information, see:
                // https://developers.google.com/admob/unity/global-settings#raise_ad_events_on_the_unity_main_thread
            });
            
            loadBannerAdButton.onClick.AddListener(LoadBannerAd);
            showBannerAdButton.onClick.AddListener(ShowBannerAd);
            loadInterstitialAdButton.onClick.AddListener(LoadInterstitialAd);
            showInterstitialAdButton.onClick.AddListener(ShowInterstitialAd);
        }

        private void LoadBannerAd()
        {
            if (_bannerView == null)
            {
                CreateBannerAd();
            }

            if (_bannerView != null)
            {
                _bannerView.LoadAd(new AdRequest());
                _bannerView.Hide();
            }
        }
        
        private void ShowBannerAd()
        {
            if (_bannerView == null)
            {
                CreateBannerAd();
            }
            
            if (_bannerView != null)
            {
                _bannerView.Show();
            }
        }
        
        private void LoadInterstitialAd()
        {
            CreateInterstitialAd();
        }
        
        private void ShowInterstitialAd()
        {
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {
                _interstitialAd.Show();
            }
            else
            {
                Debug.LogWarning("Interstitial ad is not ready to be shown.");
            }
        }
        
        private void CreateBannerAd()
        {
            if (_bannerView != null)
            {
                _bannerView.Destroy();
                _bannerView = null;
            }
            
            _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);
        }
        
        private void CreateInterstitialAd()
        {
            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            var adRequest = new AdRequest();
            InterstitialAd.Load(_interestialAdUnitId, adRequest, (interstitialAd, error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Interstitial ad failed to load with error: " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded successfully.");
                _interstitialAd = interstitialAd;
            });
        }
    }
}