using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Scripts.DataStorage;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _name;
    [SerializeField]
    private TMP_Text _points;
    [SerializeField]
    private TMP_Text _energy;
    [SerializeField]
    
    private Image _avatar;

    // Update is called once per frame
    private void Awake()
    {
        if (PlayerPrefs.HasKey("name"))
        {
            _name.text = PlayerPrefs.GetString("name");
        }
        if (PlayerPrefs.HasKey("avatar"))
        {
            _avatar.sprite = AvatarManager.Instance.LoadAvatar( PlayerPrefs.GetString("avatar"));
        }
    }
    void Update()
    {
        if (_name.text!= PlayerPrefs.GetString("name"))
        {
            _name.text = PlayerPrefs.GetString("name");
        }
        if (_avatar.name!= PlayerPrefs.GetString("avatar"))
        {
            _avatar.sprite = AvatarManager.Instance.LoadAvatar(PlayerPrefs.GetString("avatar"));
        }
    }
}
