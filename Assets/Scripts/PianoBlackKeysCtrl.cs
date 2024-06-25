using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoBlackKeysCtrl : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BlackPianoKey")
        {
            other.GetComponent<AudioSource>().Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "BlackPianoKey")
        {
            StartCoroutine(ReduceSoundSlowly(other.GetComponent<AudioSource>()));
        }
    }

    private IEnumerator ReduceSoundSlowly(AudioSource audioSource)
    {
        float volume = 1f;
        while (volume > 0f)
        {
            volume -= 0.004f;
            audioSource.volume = volume;
            yield return new WaitForEndOfFrame();
        }

        if (volume <= 0f)
        {
            audioSource.volume = 1f;
            audioSource.Stop();
            StopCoroutine(ReduceSoundSlowly(audioSource));
        }
    }
}
