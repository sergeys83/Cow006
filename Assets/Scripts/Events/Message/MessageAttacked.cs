using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageAttacked :Messages
{
   
    public MessageAttacked(Transform character, AudioClip clip)
    {
        Character = character;
        Clip = clip;
    }
}
