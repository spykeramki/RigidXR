using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuUiCtrl : MonoBehaviour
{
    public void OnClickPlayBtn()
    {
        SceneManager.LoadScene("01Main");
    }
}
