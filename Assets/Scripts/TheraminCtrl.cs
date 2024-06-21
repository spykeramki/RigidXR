using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheraminCtrl : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    public void AdjustAudioVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void AdjustAudioPitch()
    {

    }
}
