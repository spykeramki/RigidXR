using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateDestroyablePlane : MonoBehaviour
{
    public GameObject prefabToInstantiate;

    public List<DivideTrianglesOfMesh> planesGenerated = new List<DivideTrianglesOfMesh>();

    private DivideTrianglesOfMesh currentPlaneDestroying = null;

    private void Start()
    {
        Invoke("ReplaceGeneratedMeshWithTriangles", 2f);
    }

    public void ReplaceGeneratedMeshWithTriangles()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        List<MRUKAnchor> anchorsToGenerateTriangles = room.WallAnchors;
        anchorsToGenerateTriangles.Insert(0, room.CeilingAnchor);

        foreach (MRUKAnchor anchor in anchorsToGenerateTriangles)
        {
            SetPlaneToMesh(anchor);
        }
        currentPlaneDestroying = planesGenerated[0];
    }

    private void SetPlaneToMesh(MRUKAnchor anchor)
    {
        Transform anchorMeshTransform = anchor.transform.GetChild(0);
        MeshFilter meshFilter = anchorMeshTransform.GetComponent<MeshFilter>();
        Vector3 meshSize = meshFilter.sharedMesh.bounds.size;

        Vector3 prefabSize = prefabToInstantiate.GetComponent<MeshFilter>().sharedMesh.bounds.size;

        Vector3 scaleOfObject = new Vector3(meshSize.x / prefabSize.x, 1f, meshSize.y / prefabSize.z);

        GameObject planeGenerated = Instantiate(prefabToInstantiate, anchorMeshTransform);
        planeGenerated.transform.localScale = scaleOfObject;
        planeGenerated.transform.localPosition = Vector3.zero;
        planeGenerated.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        planesGenerated.Add(planeGenerated.GetComponent<DivideTrianglesOfMesh>());

        anchorMeshTransform.GetComponent<MeshRenderer>().enabled = false;
    }

    public void DestoryTianglesInPlanes()
    {
        if(planesGenerated.Count > 0)
        {
            if (!currentPlaneDestroying.IsDestroyInvoked)
            {
                currentPlaneDestroying.SendTrianglesFlyingOnInteraction();
            }
            else
            {
                planesGenerated.RemoveAt(0);
                currentPlaneDestroying = (planesGenerated.Count > 0) ? planesGenerated[0]: null;
            }
        }
    }
}
