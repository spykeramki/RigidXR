using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnerCtrl : MonoBehaviour
{
    public enum SpawnPosition
    {
        CENTER,
        SHORT_EDGE,
        LONG_EDGE
    }
    public enum SpawnDirection
    {
        TOWARDS_ANCHOR,
        AWAY_FROM_ANCHOR
    }
    public GameObject spawnPrefab;
    public MRUKAnchor.SceneLabels spawnLabel;
    public OVRCameraRig ovrCameraRig;
    public SpawnPosition spawnPosition;
    public SpawnDirection spawnDirection;

    private void Start()
    {
        Invoke("SpawnPrefabOnDefinedAnchor", 2f);
    }

    public void SpawnPrefabOnDefinedAnchor()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        List<MRUKAnchor> roomAnchors = room.GetRoomAnchors();
        List<MRUKAnchor> specificLabelAnchors = new List<MRUKAnchor>();
        foreach (MRUKAnchor anchor in roomAnchors)
        {
            if ((LabelFilter.FromEnum(spawnLabel)).PassesFilter(anchor.AnchorLabels))
            {
                specificLabelAnchors.Add(anchor);
            }
        }
        MRUKAnchor nearestAnchor = null;

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
                    //position = nearestAnchor.transform.localPosition + new Vector3(anchorSize.x/2, 0f, 0f);
                    position = ShortestPointFromUser(nearestAnchor.transform.localPosition, new Vector3(anchorSize.x / 2, 0f, 0f), ovrCameraRig.centerEyeAnchor.position);
                }
                else if(anchorSize.y < anchorSize.x)
                {
                    //position = nearestAnchor.transform.localPosition + new Vector3(0f, 0f, anchorSize.y / 2);
                    position = ShortestPointFromUser(nearestAnchor.transform.localPosition, new Vector3(0f, 0f, anchorSize.y / 2), ovrCameraRig.centerEyeAnchor.position);

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
            Quaternion direction;
            if (spawnDirection == SpawnDirection.TOWARDS_ANCHOR)
            {
                direction = Quaternion.LookRotation((nearestAnchor.transform.position - position));
            }
            else
            {
                direction = Quaternion.LookRotation((position - nearestAnchor.transform.position));
            }
            Instantiate(spawnPrefab, position, direction);
        }
    }

    private Vector3 ShortestPointFromUser(Vector3 pos1, Vector3 pos2, Vector3 posRef)
    {
        Vector3 posA = pos1 + pos2;
        Vector3 posB = pos1 - pos2;

        float distanceA = Vector3.Distance(posA, posRef);
        float distanceB = Vector3.Distance(posB, posRef);

        if(distanceA < distanceB)
        {
            return posA;
        }
        else { return posB; }
    }
}
