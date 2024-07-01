using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateDestroyablePlane : MonoBehaviour
{
    public GameObject prefabToInstantiate;

    private void Start()
    {
        Invoke("ReplaceGeneratedMeshWithTriangles", 2f);
    }

    public void ReplaceGeneratedMeshWithTriangles()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        List<MRUKAnchor> anchorsToGenerateTriangles = room.WallAnchors;
        anchorsToGenerateTriangles.Add(room.CeilingAnchor);

        foreach (MRUKAnchor anchor in anchorsToGenerateTriangles)
        {
            SetPlaneToMesh(anchor);
        }
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

        anchorMeshTransform.GetComponent<MeshRenderer>().enabled = false;
    }
}
