
using System;
using System.Collections;
using Scripts.DataStorage;
using Scripts.Menus;
using Scripts.Profile;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Menu
{
   // private bool load = false;
   // private string gameScene = "Level_3";
    public Action done;
   [SerializeField] private GameMenu _gameMenu;
   [SerializeField] private ProfileMenu _profilePanel = null;
  // [SerializeField] private ProfileUpdatePanel _profileUpdate = null;
   [SerializeField] private Button btnLogin, btnPlay, btnOptions;
   [SerializeField] private Slider loadingSlider;
   public TextMeshProUGUI message; 

   private void Awake()
   {
       DataSaver.Instance.onDataLoad += CreateAcc ;
   }
   private void Start()
   {
       btnOptions.onClick.AddListener(()=>OnBtnOptionsClick());
       btnPlay.onClick.AddListener(()=>OnBtnPlay());
       if (Egame._endGame)
       {
           OnBtnPlay();
           Egame.SetMenu(false);
       }
       else
       {
           message.text = "(Loading data ...)";
           StartCoroutine(LoadingData());
       }
   }

   private IEnumerator LoadingData()
   {
       yield return new WaitForSeconds(1.5f);
       float t = 0f;
       while (t<10f)
       {
           t++;
           loadingSlider.value = t;
           if (!String.IsNullOrEmpty(Egame.InfoMessage) )
           {
               message.text = "( " + Egame.InfoMessage + " ...)";
           }
           yield return new WaitForSeconds(1f);
       }
       loadingSlider.transform.parent.gameObject.SetActive(false);
       message.text = "";
       Egame.InfoMessage = null;
       
       btnOptions.gameObject.SetActive(true);
       btnPlay.gameObject.SetActive(true);
   }
   private void CreateAcc(bool exist)
   {
       if (!exist)
       {
           GameObject GO = GameObject.Find("PanelUpdate");
           if (GO != null)
           {
               GO.GetComponent<ProfileUpdatePanel>().ShowUpdatePanel(OnAccountCreated, true);
           }
       }
   }
   
    public void OnBtnPlay()
    {
       base.Hide();
      _gameMenu.Show();
    }
  
    public void OnBtnOptionsClick()
    {
       base.Hide();
        _profilePanel.Show();
    }
 
    private void OnAccountCreated()
    {
        if (!PlayerPrefs.HasKey("name"))
        {
            string text = DataSaver.Instance.NameGenerator(); 
            Sprite sprite = AvatarManager.Instance.LoadAvatar(DataSaver.Instance.playerData.playerAvatar);
            PlayerPrefs.SetString("name", text);
            PlayerPrefs.SetString("avatar",sprite.name);
            PlayerPrefs.Save();
        }
    }
}
