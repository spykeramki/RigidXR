using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (gameManager != null)
        {
            gameManager.destroyablePlaneParentCtrl.DestoryTianglesInPlanes();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<PianoKeyCtrl>().StartStoppingEffect();
    }

    public void OnClickExitBtn()
    {
        SceneManager.LoadScene("00Start");
    }
}
