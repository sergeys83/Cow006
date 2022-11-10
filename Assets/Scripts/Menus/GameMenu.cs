using System;
using System.Collections;
using Scripts.Menus;
using Scripts.Profile;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : Menu
{
    private Toggle _temp;

    [SerializeField] private ProfilePanel _profilePanel = null;
    [SerializeField] private ProfileMenu _pmenu;
    [SerializeField] private CanvasGroup soloGame;
    [SerializeField] private CanvasGroup netGame;
    [SerializeField] private Button solo;
    [SerializeField] private Button net;
    [SerializeField] private Button playSolo;

    private void Awake()
    {
        base.SetBackButtonHandler(OptionMenu);
        playSolo.onClick.AddListener(OnStartGameClick);

        solo.onClick.AddListener(() =>
        {
            GamePanel(soloGame, netGame);
            solo.image.sprite = solo.spriteState.selectedSprite;
            playSolo.gameObject.SetActive(true);
        });
        net.onClick.AddListener(() =>
        {
            GamePanel(netGame, soloGame);
            solo.image.sprite = solo.spriteState.disabledSprite;
            playSolo.gameObject.SetActive(false);
        });

    }
    private void Start()
    {
        GamePanel(soloGame, netGame);
        solo.onClick.Invoke();
    }

    private void GamePanel(CanvasGroup solo, CanvasGroup net)
    {
        GamePanelShow(solo);
        GamePanelHide(net);
    }

    public void GamePanelShow(CanvasGroup cgroup)
    {
        cgroup.alpha = 1;
        cgroup.blocksRaycasts = true;
    }
    public void GamePanelHide(CanvasGroup cgroup)
    {
        cgroup.alpha = 0;
        cgroup.blocksRaycasts = false;
    }

    private void OptionMenu()
    {
        _pmenu.Show();
        Hide();
    }
    public override void Show()
    {
        base.Show();
#if UNITY_ANDROID
         if (!Gpg.Instance.Authenticated)
            {
                GameObject.Find("Ranking").GetComponent<Button>().interactable = false;
            }
#endif
        _profilePanel.ShowAsync();
    }

    public void OnExitBtnClick()
    {
        Application.Quit();
    }

    public void OnStartGameClick()
    {
        StartCoroutine(LoadMenuCoroutine());
    }

    private IEnumerator LoadMenuCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Level_3", LoadSceneMode.Additive);
        
        while (!asyncLoad.isDone)
        {
            float i = asyncLoad.progress;
            yield return null;
        }

        SceneManager.UnloadSceneAsync("GameMenu");
        SceneChanged msg = new SceneChanged("Level_3");
        EventManager.Instance.SendEvent(EventId.Scenechanged, msg);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level_3"));
    }

    public void OnFourPlClick()
    {
        PlayerPrefs.SetInt("players", 4);
    }

    public void OnThreePlClick()
    {
        PlayerPrefs.SetInt("players", 3);
    }

    public void OnTwoPlClick()
    {
        PlayerPrefs.SetInt("players", 2);
    }

    public void OnClassicClick()
    {
        PlayerPrefs.SetInt("GameType", 1);
    }

    public void OnLogicClick()
    {
        PlayerPrefs.SetInt("GameType", 2);
    }
    public void OnOriginalClick()
    {
        PlayerPrefs.SetInt("GameType", 3);
    }

}
