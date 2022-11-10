using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blink : MonoBehaviour
{
    private Text txt;

    private float temp;
    private Color _txtColor;
    private Color _outColor;
    private Shadow _shadow;
    public float speed=0.7f;

    // Start is called before the first frame update
    void Start()
    {
         txt = GetComponent<Text>();
         _txtColor = txt.color;
         
         _shadow = GetComponent<Shadow>();
         _outColor = _shadow.effectColor;
         
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
            _shadow.effectColor = _outColor;
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
            _shadow.effectColor = _outColor;
        }
    }
}
