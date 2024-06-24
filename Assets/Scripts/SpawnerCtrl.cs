using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerCtrl : MonoBehaviour
{
    public enum SpawnPosition
    {
        CENTER,
        SHORT_EDGE,
        LONG_EDGE
    }
    public GameObject spawnPrefab;
    public MRUKAnchor.SceneLabels spawnLabel;
    public OVRCameraRig ovrCameraRig;
    public SpawnPosition spawnPosition;

    private void Start()
    {
        SpawnPrefabOnDefinedAnchor();
    }

    public void SpawnPrefabOnDefinedAnchor()
    {
        Debug.Log("1");
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        List<MRUKAnchor> roomAnchors = room.GetRoomAnchors();
        List<MRUKAnchor> specificLabelAnchors = new List<MRUKAnchor>();
        foreach (MRUKAnchor anchor in roomAnchors)
        {
            if ((LabelFilter.FromEnum(spawnLabel)).PassesFilter(anchor.AnchorLabels))
            {
                Debug.Log(anchor.AnchorLabels[0] + " first label");
                specificLabelAnchors.Add(anchor);
            }
        }
        MRUKAnchor nearestAnchor = null;

        Debug.Log(specificLabelAnchors.Count + " specificLabelAnchors count");
        if (specificLabelAnchors.Count > 0) { 

            nearestAnchor = specificLabelAnchors[0];
            foreach (MRUKAnchor anchor in specificLabelAnchors)
            {
                if(Vector3.Distance(anchor.transform.position, ovrCameraRig.centerEyeAnchor.position)< 
                    Vector3.Distance(nearestAnchor.transform.position, ovrCameraRig.centerEyeAnchor.position))
                {
                    nearestAnchor = anchor;
                }
            }
        }
        if(nearestAnchor != null)
        {
            Vector3 anchorSize = nearestAnchor.VolumeBounds.Value.size;

            Vector3 position = nearestAnchor.transform.position;
            if (spawnPosition == SpawnPosition.CENTER)
            {
                position = nearestAnchor.transform.position;
            }
            else if(spawnPosition == SpawnPosition.LONG_EDGE)
            {
                if (anchorSize.y > anchorSize.x)
                {
                    position = nearestAnchor.transform.localPosition + new Vector3(anchorSize.x/2, 0f, 0f);
                }
                else if(anchorSize.y < anchorSize.x)
                {
                    position = nearestAnchor.transform.localPosition + new Vector3(0f, 0f, anchorSize.y / 2);

                }
            }
            else
            {
                if (anchorSize.y > anchorSize.x)
                {
                    position = nearestAnchor.transform.localPosition + new Vector3(0f, 0f, anchorSize.y / 2);
                }
                else if (anchorSize.y < anchorSize.x)
                {
                    position = nearestAnchor.transform.localPosition + new Vector3(anchorSize.x / 2, 0f, 0f);
                }
            }

            Quaternion direction = Quaternion.LookRotation((nearestAnchor.transform.position - position).normalized);
            Instantiate(spawnPrefab, position, direction);
        }
    }
}
