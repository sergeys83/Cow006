
using System;

namespace Scripts.Menus
{
   
    public interface IMenu
    {
     
        bool IsShown { get; }

        void Show();

        void Hide();

      void SetBackButtonHandler(Action onBack);
    }

}