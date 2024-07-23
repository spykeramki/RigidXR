using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuUiCtrl : MonoBehaviour
{
    public Transform desk;
    private Vector3 tableOffset = new Vector3(0f, -0.021f, 0f);

    public void OnClickPlayBtn(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            ChangeDeskDirection();
        }
    }

    public void AttachDeskToTheTable()
    {
        
            MRUKRoom room = MRUK.Instance.GetCurrentRoom();
            MRUKAnchor tableAnchor = room.Anchors.Find((tableAnchor) =>
            {
                return tableAnchor.HasLabel("TABLE");

            });
            if (tableAnchor != null)
            {
                desk.position = tableAnchor.transform.position + tableOffset;
                Quaternion rotation = Quaternion.LookRotation(-1 *tableAnchor.transform.up, Vector3.up);
                desk.rotation = rotation;
        }
    }

    private void ChangeDeskDirection()
    {
        desk.Rotate(new Vector3(0f, desk.rotation.y + 90f, 0f));
    }

}

