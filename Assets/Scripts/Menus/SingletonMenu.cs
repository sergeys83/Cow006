
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Menus
{
  
    public class SingletonMenu<T> : Singleton<T>, IMenu where T : MonoBehaviour
    {
       
        [SerializeField] private CanvasGroup _canvasGroup = null;

        [SerializeField] protected Button _backButton = null;
        public bool IsShown { get; protected set; }
      
        [ContextMenu("Show")]
        public virtual void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            IsShown = true;
        }

        [ContextMenu("Hide")]
        public virtual void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            IsShown = false;

        }

        public virtual void SetBackButtonHandler(Action onBack)
        {
            _backButton?.onClick.AddListener(() => onBack());
        }

    }

}