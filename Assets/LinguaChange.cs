using UnityEngine;
using UnityEngine.UI;

public sealed class LinguaChange : MonoBehaviour
{
    public GameObject rusBtn;
    public GameObject engBtn;
    private int isEnglish;
    [SerializeField] Image rusFlag;
    [SerializeField] Image engFlag;

    [SerializeField] Lean.Localization.LeanLocalization lean;
    // Start is called before the first frame update
    private void Awake()
    {

        if (isEnglish== PlayerPrefs.GetInt("English", 1))
        {
            PlayerPrefs.SetInt("English", 1);
            SetEnglish();
            PlayerPrefs.Save();
        }
     
    }
    void Start()
    {        
        rusBtn.GetComponent<Button>().onClick.AddListener(SetRussian);
        engBtn.GetComponent<Button>().onClick.AddListener(SetEnglish);
        if (isEnglish==0)
        {
            SetEnglish();
        }
        else SetRussian();
    }

  private void SetEnglish()
    {
        isEnglish = 1;
        lean.SetCurrentLanguage("English");
        rusFlag.gameObject.SetActive(false);
        engFlag.gameObject.SetActive(true);
        PlayerPrefs.SetInt("English", 1);
        PlayerPrefs.Save();
    }

    private void SetRussian()
    {
        isEnglish = 0;
        lean.SetCurrentLanguage("Russian");
        rusFlag.gameObject.SetActive(true);
        engFlag.gameObject.SetActive(false);
        PlayerPrefs.SetInt("English", 0);
        PlayerPrefs.Save();
    }
}
