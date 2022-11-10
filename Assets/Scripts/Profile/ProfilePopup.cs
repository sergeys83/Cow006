

using Scripts.Menus;
using UnityEngine;

namespace Scripts.Profile
{

    public class ProfilePopup : SingletonMenu<ProfilePopup>
    {
       
        [SerializeField] private ProfilePanel _profilePanel = null;

        protected override void Awake()
        {
            base.SetBackButtonHandler(Hide);
        }

      
        public override void Show()
        { 
            _profilePanel.ShowAsync();
            base.Show();
        }

        
    }

}