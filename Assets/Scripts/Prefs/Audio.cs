using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{

    public static Audio Instance;
    public float volume = 0.5f;
    public AudioSource source;

    private void Awake()
    {
        Instance = this;
        if (PlayerPrefs.HasKey("volume"))
        {
            volume = PlayerPrefs.GetFloat("volume", 0.5f);
        }

    }
    protected void OnDestroy()
    {
        if (Instance != this)
        {
            Instance = null;
        }
    }
   
    public void Play(AudioClip clip)
    {
        source.clip = clip;
        source.volume = volume;
        source.loop = true;
        source.Play();
    }
}
