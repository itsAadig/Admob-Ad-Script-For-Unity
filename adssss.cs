using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    private BannerView bannerAd;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    // Reference to BallController for adding coins
    public BallController ballController;

    // Reference to sound effect for coin reward
    public AudioSource rewardSound;

    // Public variable for the reward amount
    public int rewardAmount;  // This can be set in the Unity Inspector

    private void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });

        // Load Ads after initialization
        RequestBannerAd();
        RequestInterstitialAd();
        RequestRewardedAd();
    }

    // Request and load a Banner Ad
    private void RequestBannerAd()
    {
        string adUnitId = "ca-app-pub-5252078634511980/9433420977"; // Test Banner Ad ID

        bannerAd = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest();
        bannerAd.LoadAd(request);
    }

    // Request and load an Interstitial Ad
    private void RequestInterstitialAd()
    {
        string adUnitId = "ca-app-pub-5252078634511980/1923953852"; // Test Interstitial Ad ID

        InterstitialAd.Load(adUnitId, new AdRequest(),
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Interstitial Ad failed to load: " + error);
                    return;
                }

                Debug.Log("Interstitial Ad loaded.");
                interstitialAd = ad;

                interstitialAd.OnAdFullScreenContentClosed += HandleOnInterstitialAdClosed;
            });
    }

    // Show Interstitial Ad
    public void ShowInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial Ad is not loaded yet.");
        }
    }

    private void HandleOnInterstitialAdClosed()
    {
        interstitialAd.Destroy();
        RequestInterstitialAd();
    }

    // Request and load a Rewarded Ad
    private void RequestRewardedAd()
    {
        string adUnitId = "ca-app-pub-5252078634511980/6807257636"; // Test Rewarded Ad ID

        RewardedAd.Load(adUnitId, new AdRequest(),
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded Ad failed to load: " + error);
                    return;
                }

                Debug.Log("Rewarded Ad loaded.");
                rewardedAd = ad;

                rewardedAd.OnAdFullScreenContentClosed += HandleRewardedAdClosed;
            });
    }

    // Show Rewarded Ad
    public void ShowRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Show((Reward reward) =>
            {
                HandleUserEarnedReward(reward);
            });
        }
        else
        {
            Debug.Log("Rewarded Ad is not loaded yet.");
        }
    }

    private void HandleRewardedAdClosed()
    {
        rewardedAd.Destroy();
        RequestRewardedAd();
    }

    private void HandleUserEarnedReward(Reward reward)
    {
        Debug.Log("User earned reward: " + reward.Amount);

        // Use the public rewardAmount variable instead of reward.Amount
        int finalRewardAmount = rewardAmount;  // Use the public variable

        // Debug log to check if the BallController is assigned
        if (ballController != null)
        {
            ballController.AddCoins(finalRewardAmount);
            Debug.Log("Coins added: " + finalRewardAmount);
        }
        else
        {
            Debug.LogError("BallController reference is not set!");
        }

        // Play the reward sound effect
        if (rewardSound != null)
        {
            rewardSound.Play();
        }
    }
}
