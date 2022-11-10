
using Scripts.DataStorage;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Scripts.Profile
{

    public class ProfilePanel : MonoBehaviour
    {

        [SerializeField] private Text _usernameText = null;

        [SerializeField] private Text _statsText = null;

        [SerializeField] private Image _avatarImage = null;

        [SerializeField] private Button _profileUpdateButton = null;

       private void Awake()
        {
            if (_profileUpdateButton)
            {
                _profileUpdateButton.onClick.AddListener(() => ProfileUpdatePanel.Instance.ShowUpdatePanel(OnAccoutUpdated, true)); 
            }
           
        }

       public  void ShowAsync()
        {
          SetUIAccessAsync();
        }
   
        private void SetUIAccessAsync()
        {
            _usernameText.text = DataSaver.Instance.playerData.playerName;
            _avatarImage.sprite = AvatarManager.Instance.LoadAvatar(DataSaver.Instance.playerData.playerAvatar);
            if (_profileUpdateButton)
            {
                _profileUpdateButton.gameObject.SetActive(true);
            }
        }

        private void OnAccoutUpdated()
        {
            _usernameText.text = DataSaver.Instance.playerData.playerName; 
            _avatarImage.sprite = AvatarManager.Instance.LoadAvatar(DataSaver.Instance.playerData.playerAvatar);
            PlayerPrefs.SetString("name", _usernameText.text);
            PlayerPrefs.SetString("avatar",_avatarImage.sprite.name);
            
            ShowAsync();
        }

    }

}