
using UnityEngine;
using UnityEngine.UI;

public class GameDesc : MonoBehaviour
{
    public Text classicDesc;

    public Text logicText;
    public Text originalText;
    public Toggle classic;

    public Toggle logic;
    // Start is called before the first frame update
    void Start()
    {
        if (logic.isOn)
        {
            logicText.gameObject.SetActive(true);
            classicDesc.gameObject.SetActive(false);
            originalText.gameObject.SetActive(false);
            PlayerPrefs.SetInt("GameType",2);
           
        }
        else if (classic.isOn)
        {
            logicText.gameObject.SetActive(false);
            classicDesc.gameObject.SetActive(true);
            originalText.gameObject.SetActive(false);
            PlayerPrefs.SetInt("GameType",1);
        }
        else
        {
            originalText.gameObject.SetActive(true);
            logicText.gameObject.SetActive(false);
            classicDesc.gameObject.SetActive(false);
            PlayerPrefs.SetInt("GameType",3);
        }
       
    }

    public void SetClassic()
    {
        logicText.gameObject.SetActive(false);
        classicDesc.gameObject.SetActive(true);
        originalText.gameObject.SetActive(false);
    }

    public void SetLogic()
    {
        logicText.gameObject.SetActive(true);
        classicDesc.gameObject.SetActive(false);
        originalText.gameObject.SetActive(false);
    }
    public void SetOriginal()
    {
        originalText.gameObject.SetActive(true);
        logicText.gameObject.SetActive(false);
        classicDesc.gameObject.SetActive(false);
    }
}
