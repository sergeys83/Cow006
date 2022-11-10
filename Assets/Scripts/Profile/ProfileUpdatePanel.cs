
using System;
using Scripts.DataStorage;
using Scripts.Menus;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Scripts.Profile
{
    public class ProfileUpdatePanel : SingletonMenu<ProfileUpdatePanel>
    {
        [SerializeField] private Button _doneButton = null;
        [SerializeField] private InputField _usernameText = null;
        [SerializeField] private Image _avatarImage = null;
        [SerializeField] private Button _avatarButton = null;
        [SerializeField] private Text _userText = null;
        [SerializeField] private CanvasGroup _optionMenu;
        
        private string _avatarPath;

        private void Start()
        {
            _avatarButton.onClick.AddListener(ChangeAvatar);
            base.SetBackButtonHandler(Hide);
        }

        public void ShowUpdatePanel(Action onDone, bool canTerminate)
        {
            _doneButton.onClick.RemoveAllListeners();
            _doneButton.onClick.AddListener(() => Done(onDone));

            _usernameText.text = DataSaver.Instance.playerData.playerName;
            _userText.text = DataSaver.Instance.playerData.playerName;
            _avatarPath = DataSaver.Instance.playerData.playerAvatar;
            _avatarImage.sprite = AvatarManager.Instance.LoadAvatar(_avatarPath);
            Show();
        }

        private void ChangeAvatar()
        {
            _avatarPath = AvatarManager.Instance.NextAvatar(_avatarPath);
            _avatarImage.sprite = AvatarManager.Instance.LoadAvatar(_avatarPath);
        }

        private void Done(Action onDone)
        {
            DataSaver.Instance.SetData(_usernameText.text, _avatarPath);
            PlayerPrefs.SetString("name",_usernameText.text);
            DataSaver.Instance.SaveToCloud();
            Hide();
            onDone?.Invoke();
        }

        public override void Show()
        {
            if (_optionMenu.alpha==1)
            {
                _optionMenu.blocksRaycasts = false;
            }
            base.Show();
        }

        public override void Hide()
        {
            if (_optionMenu.alpha==1)
            {
                _optionMenu.blocksRaycasts = true;
            }
            base.Hide();
        }
    }
}