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

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PianoKeyCtrl>().PlayAudio();
        gameManager.destroyablePlaneParentCtrl.DestoryTianglesInPlanes();
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<PianoKeyCtrl>().StartStoppingEffect();
    }
}
