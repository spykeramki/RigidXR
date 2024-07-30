using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuUiCtrl : MonoBehaviour
{
    public Transform desk;
    public MRUKAnchor.SceneLabels spawnLabel;
    public OVRCameraRig ovrCameraRig;
    private Vector3 tableOffset = new Vector3(0f, -0.021f, 0f);
    private MRUKAnchor nearestAnchor = null;
    private List<Vector3> anchorPlaneMidCenters = new List<Vector3>();
    private int currentPoint = 0;

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
                return tableAnchor.Label == MRUKAnchor.SceneLabels.TABLE;

            });
            if (tableAnchor != null)
            {
                desk.position = tableAnchor.transform.position + tableOffset;
                desk.rotation = GetTableDirection();
        }
    }

    private void ChangeDeskDirection()
    {
        desk.Rotate(new Vector3(0f, desk.rotation.y + 90f, 0f));
    }

    public Quaternion GetTableDirection()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        List<MRUKAnchor> roomAnchors = room.Anchors;
        List<MRUKAnchor> specificLabelAnchors = new List<MRUKAnchor>();

        foreach (MRUKAnchor anchor in roomAnchors)
        {
            if (LabelFilter.Included(spawnLabel).PassesFilter(anchor.Label))
            {
                specificLabelAnchors.Add(anchor);
            }
        }
        if (specificLabelAnchors.Count > 0)
        {

            nearestAnchor = specificLabelAnchors[0];
            foreach (MRUKAnchor anchor in specificLabelAnchors)
            {
                if (GetDistanceFromPlayer(anchor.transform.position) <
                    GetDistanceFromPlayer(nearestAnchor.transform.position))
                {
                    nearestAnchor = anchor;
                }
            }
        }
        if (nearestAnchor != null)
        {
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
                        if (midPoint != new Vector3(0f, 0f, 0f))
                        {
                            anchorPlaneMidCenters.Add(midPoint);
                        }
                    }

                }

            }

            if (anchorPlaneMidCenters.Count > 0)
            {
                Vector3 nearestMidPositionInWorld = nearestAnchor.transform.TransformPoint(anchorPlaneMidCenters[currentPoint]);
                for (int i = 0; i < anchorPlaneMidCenters.Count; i++)
                {
                    Vector3 pos = nearestAnchor.transform.TransformPoint(anchorPlaneMidCenters[i]);
                    if (GetDistanceFromPlayer(pos) < GetDistanceFromPlayer(nearestMidPositionInWorld))
                    {
                        currentPoint = i;
                        nearestMidPositionInWorld = pos;
                    }
                }

                Quaternion direction = Quaternion.LookRotation((nearestAnchor.transform.position - nearestMidPositionInWorld));

                return direction;
            }
        }
        return Quaternion.identity;
    }

    private float GetDistanceFromPlayer(Vector3 pos)
    {
        return Vector3.Distance(pos, ovrCameraRig.centerEyeAnchor.position);
    }

}

