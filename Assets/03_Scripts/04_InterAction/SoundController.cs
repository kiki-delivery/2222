using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{

    [SerializeField]
    AudioSource audio;

    public void SoundPlay(AudioClip clip)
    {
        audio.clip = clip;
        if (!audio.isPlaying)
        {
            audio.Play();
        }
    }
}
