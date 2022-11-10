using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;


public class Admobs : MonoBehaviour, IUnityAdsInitializationListener
{
    string gameId = "3264684";
    bool testMode = true;
    // Start is called before the first frame update
    void Start()
    {
       Advertisement.Initialize(gameId, !testMode,this);
       MetaData userMetaData = new MetaData("user");
       userMetaData.Set("nonbehavioral", "true");
       Advertisement.SetMetaData(userMetaData);
    }

    public void OnInitializationComplete() 
    {
        string s = "Init Ad: success";
        HintBox.Instance.Show(s,"");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        string s = "Init Ad: fale error, "+message;
        HintBox.Instance.Show(s,error.ToString());
    }

}
