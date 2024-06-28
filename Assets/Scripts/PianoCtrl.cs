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
            other.GetComponent<PianoKeyCtrl>().PlayAudio();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PianoKey")
        {
            other.GetComponent<PianoKeyCtrl>().StartStoppingEffect();
        }
    }
}
