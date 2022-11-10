using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEffect : MonoBehaviour
{
    public AudioClip clip;

    public void PlayMove()
    {
        
    }
    public void PlayClicked()
    {
        MessageClicked msg = new MessageClicked(clip);
        EventManager.Instance.SendEvent(EventId.CardClicked, msg);
    }
}

