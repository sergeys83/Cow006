using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HintBox : Singleton<HintBox>
{
    private string _message;
    [SerializeField]private Text _BoxTitle;
    [SerializeField]private Text _boxMessage;
    private bool _visible=false;
  //  private float time = -1f;
    
   protected override void Awake()
    {   
        base.Awake();
        _visible = true;
    }
    public void Show(String message, String title)
    {
        _BoxTitle.text = title;
        _boxMessage.text = message;
        _visible = true;
    }

    public void Hide()
    {
        _visible = false; 
    }
}