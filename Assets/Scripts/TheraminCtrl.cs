using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TheraminCtrl : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    private string VOLUME = "Volume";
    private string PITCH = "Pitch";

    [SerializeField]
    private AudioSource audioSource;

    public void AdjustAudioVolume(float volume)
    {
        //audioMixer.SetFloat(VOLUME, Mathf.Log10(volume) * 20);
        audioSource.volume = volume;
    }

    public void AdjustAudioPitch(float pitch)
    {
        //audioMixer.SetFloat(PITCH, pitch*2);
        audioSource.pitch = pitch * 2.5f;
    }
}
