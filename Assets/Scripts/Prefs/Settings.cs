using System;
using Scripts.Menus;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[Serializable]
public class SoundSliderData
{
    public Slider VolumeSlider;
    public string MixerName;
    private AudioMixer Mixer;
    public void SliderChanged(float volume)
    {
        Mixer.SetFloat(MixerName, volume);
    }
    public void Init(AudioMixer mixer)
    {
        float currentBGVolume;

        Mixer = mixer;
        Mixer.GetFloat(MixerName, out currentBGVolume);
        VolumeSlider.value = currentBGVolume;
        VolumeSlider.onValueChanged.AddListener(SliderChanged);
    }
}


public class Settings : Menu
{
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Slider vSlider;
    public Button _buttonRules;
    [SerializeField] private Rules rulesPanel;
    [SerializeField] private Transform plHand;
    [SerializeField] private Image mOff;

    public AudioMixer Mixer;
    public SoundSliderData SoundSettings;
    private void Awake()
    {
        base.SetBackButtonHandler(Hide);
        SoundSettings.Init(Mixer);
        _buttonRules.onClick.AddListener(rulesPanel.Show);
    }

    public void SetVolume()
    {
        if (vSlider.value > 0 || musicToggle.isOn)
        {
            musicToggle.isOn = true;
        }
        else if (!musicToggle.isOn)
        {
            musicToggle.isOn = false;
        }
        mOff.gameObject.SetActive(!musicToggle.isOn);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("music"))
        {
            PlayerPrefs.SetInt("music", 1);
        }
        if (PlayerPrefs.GetInt("music") == 1)
        {
            musicToggle.isOn = true;
            vSlider.value = PlayerPrefs.GetFloat("volume", 0.5f);
//            audio.source.Play();
        }
        else
        {
            musicToggle.isOn = false;
        }
        mOff.gameObject.SetActive(!musicToggle.isOn);
    }

    private void OnDestroy()
    {
        if (musicToggle.isOn)
        {
            PlayerPrefs.SetInt("music", 1);
        }
        else
        {
            PlayerPrefs.SetInt("music", 0);
        }
        PlayerPrefs.SetFloat("volume", vSlider.value);
        PlayerPrefs.Save();
    }

    public override void Show()
    {
        base.Show();
        Time.timeScale = 0;
        if (!SceneManager.GetActiveScene().name.Equals("GameMenu")&&!SceneManager.GetActiveScene().name.Equals("Level_Net"))
        {
            GameManagerScr.S.isOpendSetting = true;
        }
        else if (!SceneManager.GetActiveScene().name.Equals("GameMenu")&&!SceneManager.GetActiveScene().name.Equals("Level_3"))
        {
            GameManagerNet.S.isOpendSetting = true;
        }
    }

    public override void Hide()
    {
        Time.timeScale = 1;
        base.Hide();
        if (!SceneManager.GetActiveScene().name.Equals("GameMenu")&&!SceneManager.GetActiveScene().name.Equals("Level_Net"))
        {
            GameManagerScr.S.isOpendSetting = false;
        }
        else if (!SceneManager.GetActiveScene().name.Equals("GameMenu")&&!SceneManager.GetActiveScene().name.Equals("Level_3"))
        {
            GameManagerNet.S.isOpendSetting = false;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
