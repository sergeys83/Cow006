
using Scripts.Menus;
using UnityEngine;

namespace Scripts.Profile
{
    public class ProfileMenu : Menu
    {
       
        [SerializeField] private ProfilePanel _profilePanel = null;

        private void Awake()
        {
            base.SetBackButtonHandler(Mmenu);
        }

        public override void Show()
        {
            _profilePanel.ShowAsync();
            base.Show();
        }

        private void Mmenu()
        {
           GameObject mM = GameObject.Find("Game");
           mM.GetComponent<GameMenu>().Show();
           Hide();
        }
        
    }

}