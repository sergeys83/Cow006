
using UnityEngine;
using UnityEngine.UI;

public class ToggleUi : MonoBehaviour
{
    private Toggle musicToggle;
    public GameObject mOff, mOn;
    private SceneChanged msg;
    void Awake()
    {
        musicToggle = GetComponent<Toggle>();
    }
    
    public void SetPik()
    {
        if (musicToggle.isOn)
        {
            SoundManager.Instance.PlayBg();
        }
        else
        {
            SoundManager.Instance.StopBgSound();
        }
        mOff.SetActive(!musicToggle.isOn);
        mOn.SetActive(musicToggle.isOn);
    }

    public void SetVfx()
    {
        SoundManager.Instance.PlayVfx = musicToggle.isOn;
        mOff.SetActive(!musicToggle.isOn);
        mOn.SetActive(musicToggle.isOn);
    }
   
}
