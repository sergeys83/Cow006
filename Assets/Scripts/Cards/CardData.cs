using UnityEngine;
using UnityEngine.UI;

namespace Cards
{
    [System.Serializable]
    public class CardData
    {
        public string Name;
        public Sprite Logo;
        public Sprite HideLogo;
        public int Attack;
        public int Points;
    
        public GameObject CardPrefub;
        public AudioClip clip;
    }
}
