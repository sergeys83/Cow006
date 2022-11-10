using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Menus;
using UnityEngine;
using UnityEngine.UI;



public class Rules : Menu
{
    [SerializeField] private Text rulesTex;
    [SerializeField] private CanvasGroup _optionMenu;

    public Text RulesTex
    {
        get => rulesTex;
        set => rulesTex = value;
    }

    private void Awake()
    {
        base.SetBackButtonHandler(Hide);
            
    }
// Update is called once per frame
    
    public override void Show()
    {
        _optionMenu.blocksRaycasts = false;
        base.Show();
    }

    public override void Hide()
    {
        _optionMenu.blocksRaycasts = true;
        base.Hide();
    }
}
