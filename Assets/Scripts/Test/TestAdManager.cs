using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

namespace BlackHole.Test
{
    public class TestAdManager : MonoBehaviour
    {
        [SerializeField] private Button loadBannerAdButton;
        [SerializeField] private Button showBannerAdButton;
        
        private string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        private BannerView bannerView;
        
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
        }

        private void LoadBannerAd()
        {
            if (bannerView == null)
            {
                CreateBannerAd();
            }

            if (bannerView != null)
            {
                bannerView.LoadAd(new AdRequest());
                bannerView.Hide();
            }
        }
        
        private void ShowBannerAd()
        {
            if (bannerView == null)
            {
                CreateBannerAd();
            }
            
            if (bannerView != null)
            {
                bannerView.Show();
            }
        }
        
        private void CreateBannerAd()
        {
            if (bannerView != null)
            {
                bannerView.Destroy();
                bannerView = null;
            }
            
            bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        }
    }
}