using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(Shadow))]
public class BlinkImage : MonoBehaviour
{
    private Image txt;

    private float temp;
    private Color _txtColor;
    private Color _outColor;
    private Shadow _outline;
    public float speed=0.7f;

    // Start is called before the first frame update
    
    void Start()
    {
         txt = GetComponent<Image>();
         _txtColor = txt.color;
         
         _outline = GetComponent<Shadow>();
         _outColor = _outline.effectColor;
         
         temp = 0;
    }

    // Update is called once per frame
    void Update()
    {
        temp += Time.deltaTime*speed;
        if (temp>0.001f&&temp<1f)
        {
            _txtColor.a = temp;
            txt.color = _txtColor;
            _outColor.a = _txtColor.a * 0.8f;
            _outline.effectColor = _outColor;
        }
        else if (temp>0.999f)
        {
            temp = -1f;
        }

        if (temp < 0f)
        {
            _txtColor.a =Math.Abs( temp);
            txt.color = _txtColor;
            
            _outColor.a = _txtColor.a * 0.8f;
            _outline.effectColor = _outColor;
        }
    }
}
