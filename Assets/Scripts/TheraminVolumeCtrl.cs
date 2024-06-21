using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheraminVolumeCtrl : MonoBehaviour
{
    public Collider innerCollider;
    public TheraminCtrl theraminCtrl;

    private List<Collider> collidersEntered = new List<Collider>();
    private List<Transform> collidersTransformsEntered = new List<Transform>();

    private float maxTriggerDistance = 0.175f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand")
        {
            collidersEntered.Add(other);
            collidersTransformsEntered.Add(other.transform);
        }
    }

    private void FixedUpdate()
    {
        if (collidersEntered.Count > 0)
        {
            float smallestDistance = 100f;
            for (int i = 0; i < collidersEntered.Count; i++)
            {
                Vector3 pointOnInnerCollider = innerCollider.ClosestPointOnBounds(collidersTransformsEntered[i].position);
                Vector3 pointOnHandCollider = collidersEntered[i].ClosestPointOnBounds(pointOnInnerCollider);
                float distanceBetweenColliderPoints = Vector3.Distance(pointOnHandCollider, pointOnInnerCollider);
                if (distanceBetweenColliderPoints < smallestDistance)
                {
                    smallestDistance = distanceBetweenColliderPoints;
                }
            }
            float valueToBeAdjusted = 1 - (smallestDistance / maxTriggerDistance);
            if (valueToBeAdjusted < 0)
            {
                valueToBeAdjusted = 0;
            }
            Debug.Log(valueToBeAdjusted + " pitch valueToBeAdjusted");
            Debug.Log(smallestDistance + "smallest distance");
            //theraminCtrl.AdjustAudioPitch(valueToBeAdjusted);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Hand")
        {
            collidersEntered.Remove(other);
            collidersTransformsEntered.Remove(other.transform);
        }
    }
}
