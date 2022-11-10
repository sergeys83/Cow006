using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageDeath : Messages
{
    public MessageDeath(Transform character, AudioClip clip)
    {
        Character = character;
        Clip = clip;
    }

}
