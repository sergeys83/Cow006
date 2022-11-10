using System;
using System.Collections;
using Scripts.DataStorage;
using Scripts.Menus;
using Scripts.Profile;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenu : Singleton<LoadingMenu>
{
    public Action done;
    [SerializeField] private Button btnPlay;
    [SerializeField] private Slider loadingSlider;
    public TextMeshProUGUI message;
    private AsyncOperation loadingMenu;
    [SerializeField] private Canvas canvas;
    [SerializeField]  private Camera camera;
    private void Awake()
    {
        DataSaver.Instance.onDataLoad += CreateAcc;
        done += HideMenu;
        canvas = FindObjectOfType< Canvas>();
        camera = FindObjectOfType< Camera>();
    }
    private void Start()
    {
        btnPlay.onClick.AddListener(() => OnBtnPlay());
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

    private void HideMenu()
    {
           canvas.gameObject.SetActive(false);
           camera.gameObject.SetActive(false);
    }
    private IEnumerator LoadingData()
    {
        yield return new WaitForSeconds(1.5f);
        float t = 0f;
        while (t < 3f)
        {
            t++;
            loadingSlider.value = t;
            if (!String.IsNullOrEmpty(Egame.InfoMessage))
            {
                message.text = "( " + Egame.InfoMessage + " ...)";
            }
            yield return new WaitForSeconds(1f);
        }
        loadingSlider.transform.parent.gameObject.SetActive(false);
        message.text = "";
        Egame.InfoMessage = null;
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
        Scene curScene = SceneManager.GetActiveScene();
       // loadingMenu = SceneManager.LoadSceneAsync(curScene.buildIndex + 1, new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.None));
        StartCoroutine(LoadMenuCoroutine(curScene));
    }
    
    private IEnumerator LoadMenuCoroutine( Scene curScene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(curScene.buildIndex + 1, new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
        while (!asyncLoad.isDone)
        {
            float i = asyncLoad.progress;
            yield return null;
        }
        int sc = curScene.buildIndex;
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(sc+1));
        done?.Invoke();
    }

    private void OnAccountCreated()
    {
        if (!PlayerPrefs.HasKey("name"))
        {
            string text = DataSaver.Instance.NameGenerator();
            Sprite sprite = AvatarManager.Instance.LoadAvatar(DataSaver.Instance.playerData.playerAvatar);
            PlayerPrefs.SetString("name", text);
            PlayerPrefs.SetString("avatar", sprite.name);
        }
    }
}
