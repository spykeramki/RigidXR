using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnerCtrl : MonoBehaviour
{
    public enum ViewDirection
    {
        AWAY,
        TOWARDS
    }
    public GameObject spawnPrefab;
    public MRUKAnchor.SceneLabels spawnLabel;
    public OVRCameraRig ovrCameraRig;
    public ViewDirection viewDirection = ViewDirection.TOWARDS;

    private List<Vector3> anchorPlaneMidCenters = new List<Vector3>();
    private int currentPoint = 0;
    private MRUKAnchor nearestAnchor = null;

    private void Start()
    {
        Invoke("SpawnsPrefabOnDefinedAnchor", 2f);
    }

    private Transform spawnedObjTransform;

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            ChangeObjPosition();
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


    public void SpawnsPrefabOnDefinedAnchor()
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

        if (specificLabelAnchors.Count > 0)
        {

            nearestAnchor = specificLabelAnchors[0];
            foreach (MRUKAnchor anchor in specificLabelAnchors)
            {
                if (Vector3.Distance(anchor.transform.position, ovrCameraRig.centerEyeAnchor.position) <
                    Vector3.Distance(nearestAnchor.transform.position, ovrCameraRig.centerEyeAnchor.position))
                {
                    nearestAnchor = anchor;
                }
            }
        }
        if (nearestAnchor != null)
        {
            //Plane Boundary 2D represents the corners of the surface of the shape. So getting table surface
            List<Vector2> planeBoundaries = nearestAnchor.PlaneBoundary2D;
            for (int i = 0; i < planeBoundaries.Count; i++)
            {
                for (int j = 0; j < planeBoundaries.Count; j++)
                {
                    if (i != j && i < j)
                    {
                        Vector2 firstPoint = planeBoundaries[i];
                        Vector2 secondPoint = planeBoundaries[j];
                        Vector3 midPoint = new Vector3((firstPoint.x + secondPoint.x) / 2, (firstPoint.y + secondPoint.y) / 2, 0f);
                        if(midPoint!= new Vector3(0f, 0f, 0f))
                        {
                            anchorPlaneMidCenters.Add(midPoint);
                        }
                    }

                }
                
            }
            if (anchorPlaneMidCenters.Count > 0)
            {
                Vector3 position = anchorPlaneMidCenters[currentPoint];

                Quaternion direction;
                if(viewDirection == ViewDirection.TOWARDS)
                {
                    direction =  Quaternion.LookRotation((nearestAnchor.transform.position - nearestAnchor.transform.TransformPoint(position)));
                }
                else
                {
                    direction = Quaternion.LookRotation((nearestAnchor.transform.TransformPoint(position) - nearestAnchor.transform.position));
                }

                GameObject spawnedObject = Instantiate(spawnPrefab, nearestAnchor.transform.TransformPoint(position), direction, nearestAnchor.transform);
                spawnedObjTransform = spawnedObject.transform;
            }
        }
    }

    private void ChangeObjPosition()
    {
        currentPoint++;
        if(currentPoint >= anchorPlaneMidCenters.Count)
        {
            currentPoint = 0;
        }
        Vector3 position = anchorPlaneMidCenters[currentPoint];
        Quaternion direction;
        if (viewDirection == ViewDirection.TOWARDS)
        {
            direction = Quaternion.LookRotation((nearestAnchor.transform.position - nearestAnchor.transform.TransformPoint(position)));
        }
        else
        {
            direction = Quaternion.LookRotation((nearestAnchor.transform.TransformPoint(position) - nearestAnchor.transform.position));
        }
        spawnedObjTransform.localPosition = anchorPlaneMidCenters[currentPoint];
        spawnedObjTransform.rotation = direction;
        //spawnedObjTransform.rotation = Quaternion.LookRotation((new Vector3(0f, 0f, 0f) - position));
    }
}
