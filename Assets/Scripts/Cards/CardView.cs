using System;
using UnityEngine;
using UnityEngine.UI;
namespace Cards
{
    public class CardView : MonoBehaviour
    {
        public const float CARD_DURATION = 0.7f;
        public const float CARD_WIDTH = 86f;
        public const float CARD_HEIGHT = 112f;
        [Space]
        public int Attack;
        public int Points;
        [Space]
        public Image Avatar;
        public Text points;
        public Text name;
        private string Name;
        public AudioClip clip;
        public Image HideLogo;
        public bool faceUp
        {
            get { return (!HideLogo.gameObject.activeSelf); }
            set { HideLogo.gameObject.SetActive(!value); }
        }
        public void SetCardUi(CardData data)
        {
            Avatar.sprite = data.Logo;
            points.text = data.Points.ToString();
            name.text = data.Name;
            Attack = data.Attack;
            Points = data.Points;
            HideLogo.sprite = data.HideLogo;
        }
    }
}

