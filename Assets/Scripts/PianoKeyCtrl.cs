using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoKeyCtrl : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio()
    {
        if (audioSource != null)
        {
            StopAllCoroutines();
            audioSource.Stop();
            audioSource.volume = 1.0f;
            audioSource.Play();
        }
    }

    public void StartStoppingEffect()
    {
        StartCoroutine(ReduceSoundSlowly());
    }

    private IEnumerator ReduceSoundSlowly()
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
            StopCoroutine(ReduceSoundSlowly());
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
