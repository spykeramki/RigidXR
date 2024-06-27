using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoCtrl : MonoBehaviour
{
    [Serializable]
    public struct KeySounds
    {
        public string keyName;
        public AudioClip audioClip;
    }

    public float speed = 5f;

    public List<KeySounds> keySounds;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PianoKey")
        {
            other.GetComponent<AudioSource>().Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PianoKey")
        {
            StartCoroutine(ReduceSoundSlowly(other.GetComponent<AudioSource>()));
        }
    }

    private IEnumerator ReduceSoundSlowly(AudioSource audioSource)
    {
        float volume = 1f;
        while(volume > 0f)
        {
            volume -= 0.004f;
            audioSource.volume = volume;
            yield return new WaitForEndOfFrame();
        }

        if(volume <= 0f)
        {
            audioSource.volume = 1f;
            audioSource.Stop();
            StopCoroutine(ReduceSoundSlowly(audioSource));
        }
    }


}
