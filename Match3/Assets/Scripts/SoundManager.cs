using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource swipenoise;
    public void PlayClip()
    {
        swipenoise.Play();
    }
}
