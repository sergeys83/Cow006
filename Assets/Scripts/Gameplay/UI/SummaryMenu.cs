using System;
using Scripts.Menus;
using Scripts.Profile;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Gameplay.UI
{
    public class SummaryMenu : Menu
    {
        public Button menuBtn;
        [SerializeField] private Image[] winShine = null;

        [SerializeField] private Text _rewardText = null;
        [SerializeField] private Text _pointsText = null;

        [SerializeField] private Text _header = null;

        [SerializeField] private Image _resultImage = null;

        [SerializeField] private string _winHeader = string.Empty;

        [SerializeField] private Sprite _winSprite = null;

        [SerializeField] private string _loseHeader = string.Empty;

        [SerializeField] private Sprite _loseSprite = null;

        [SerializeField] private AudioClip loseClip;
        [SerializeField] private AudioClip winClip;
        private void Awake()
        {
            base.SetBackButtonHandler(Hide);
        }
        public void SetResult(bool win, int reward, int points)
        {

            if (win)
            {
                MessageGameEnded msg = new MessageGameEnded(winClip);
                EventManager.Instance.SendEvent(EventId.EndGame, msg);
                _header.text = _winHeader;
                _resultImage.sprite = _winSprite;
                _rewardText.text = $"+{reward}";
                _pointsText.text = $"+{points}";
                foreach (var item in winShine)
                {
                    item.transform.gameObject.SetActive(true);
                }
            }
            else
            {
                MessageGameEnded msg = new MessageGameEnded(loseClip);
                EventManager.Instance.SendEvent(EventId.EndGame, msg);

                foreach (var item in winShine)
                {
                    item.transform.gameObject.SetActive(false);
                }
                _header.text = _loseHeader;
                _resultImage.sprite = _loseSprite;
                _resultImage.color = Color.red;
                _rewardText.text = $"+{reward}";
                _pointsText.text = $"({points})";
            }

            points += DataSaver.Instance.playerData.curPoints;
            reward += DataSaver.Instance.playerData.money;

            PlayerPrefs.SetInt("points", points);
            PlayerPrefs.SetInt("money", reward);
            PlayerPrefs.Save();
            DataSaver.Instance.SetData(points, reward);
#if UNITY_ANDROID
           if (Gpg.Instance.Authenticated)
            {
                DataSaver.Instance.SaveToCloud();
                Gpg score = Gpg.Instance;
      
                score.SetScore(points);
                score.PostToLeaderboard();
            }
#endif
        }

        /*  public void Hide()
          {
            base.Hide();
          }*/

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
