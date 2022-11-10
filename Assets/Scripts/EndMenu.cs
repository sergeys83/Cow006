
using System;
using System.Collections;
using Scripts.Gameplay.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class EndMenu : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
  [SerializeField]  private SummaryMenu _summaryMenu;
  private const string MainMenu = "GameMenu";
  private const string Game = "Level_3";
  public string level=null;
  string _gameId = "SVideo";
  //public LoadingMenu LoadingMenu;

  private void Awake()
  {
      Debug.Log("Ads start Loading");
      LoadAd();
  }
  private void Restart()
  {
      Debug.Log("Going to main menu" + _gameId);
      Egame.SetMenu(false);
      SceneManager.LoadScene(MainMenu);
  }

  public void ShowUnityAds()
  {
      // Note that if the ad content wasn't previously loaded, this method will fail
      Debug.Log("Showing Ad: " + _gameId);
      Advertisement.Show(_gameId, this);
  }
  public void OnMenuClick()
    {
        level = MainMenu;
        Egame.SetMenu(true);
        ShowUnityAds();
    }

    public void OnRestartGame()
    {
        level = Game;
        SceneManager.LoadScene(Game);
    }

    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Start Loading Ad: " + _gameId);
        Advertisement.Load(_gameId, this);
    }
    
    public void OnUnityAdsAdLoaded(string placementId)
    {
        _summaryMenu.menuBtn.gameObject.SetActive(true);
        
        string s = "Loading Ad: success, ";
        HintBox.Instance.Show(s,"");
    }
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log("Ads Loaded fail");
        
        string s = "Loading Ad: fale error, "+message;
        HintBox.Instance.Show(s,error.ToString());
        StartCoroutine(Delay(3f));
        _summaryMenu.menuBtn.gameObject.SetActive(true);
    }

    IEnumerator Delay(float dTime)
    {
        yield return new WaitForSeconds(dTime);
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
       // string s = "Show Ad: fale error, "+message;
      //  HintBox.Instance.Show(s,error.ToString());
      //  Egame.SetMenu(false);
        Restart();
    }

    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            StartCoroutine(Delay());
            Restart();
        }
        Restart();
      //  string s = "Show Ad: fale error, " +showCompletionState.ToString();
     //   HintBox.Instance.Show(s,showCompletionState.ToString());
        
    }
    IEnumerator Delay()
    {
       yield return new WaitForSeconds(1f);
    }
  
}
