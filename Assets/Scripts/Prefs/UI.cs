using System;
using Scripts.DataStorage;
using Scripts.Profile;
using UnityEngine.UI;
using UnityEngine;

namespace Prefs
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private Text reward;
        [SerializeField] private Text points;
        [SerializeField] private Text Name;
        [SerializeField] private Image logo;
        private int GPoints()
        {
            int m = DataSaver.Instance.playerData.curPoints;
            return m;
        }
        private int GMoney()
        {
            int m = DataSaver.Instance.playerData.money;
            return m;
        }
        private void Start()
        {
            reward.text = GMoney().ToString();
            points.text = GPoints().ToString();
            Name.text = DataSaver.Instance.playerData.playerName;
            logo.sprite = AvatarManager.Instance.LoadAvatar(DataSaver.Instance.playerData.playerAvatar);
        }

    }
}
