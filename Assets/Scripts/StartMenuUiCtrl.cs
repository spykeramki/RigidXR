using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuUiCtrl : MonoBehaviour
{
    public Transform desk;
    private Vector3 tableOffset = new Vector3(0f, 1.037f, 0f);

    private void Awake()
    {
        SceneManager.sceneLoaded += AttachDeskToTheTable;
    }

    public void OnClickPlayBtn()
    {
        SceneManager.LoadScene("01Main");
    }
    private void AttachDeskToTheTable(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "00Start")
        {
            MRUKRoom room = MRUK.Instance.GetCurrentRoom();
            MRUKAnchor tableAnchor = room.Anchors.Find((tableAnchor) =>
            {
                return tableAnchor.HasLabel("TABLE");

            });
            if (tableAnchor != null)
            {
                desk.position = tableAnchor.transform.position + tableOffset;
                desk.up = tableAnchor.transform.forward;
                desk.forward = tableAnchor.transform.right;
                //desk.Lo
                //desk.rotation = tableAnchor.transform.rotation;
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= AttachDeskToTheTable;
    }
}

