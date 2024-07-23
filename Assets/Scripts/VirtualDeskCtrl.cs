using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualDeskCtrl : MonoBehaviour
{
    private Vector3 tableOffset = new Vector3(0f, -0.2f, 0f);
    public void AttachDeskToTheTable()
    {

        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        MRUKAnchor tableAnchor = room.Anchors.Find((tableAnchor) =>
        {
            return tableAnchor.HasLabel("TABLE");

        });
        if (tableAnchor != null)
        {
            transform.position = tableAnchor.transform.position + tableOffset;
            Quaternion rotation = Quaternion.LookRotation(-1 * tableAnchor.transform.up, Vector3.up);
            transform.rotation = rotation;
        }
    }
}
