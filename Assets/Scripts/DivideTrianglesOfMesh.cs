using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivideTrianglesOfMesh : MonoBehaviour
{
    private MeshRenderer m_MeshRenderer;
    private MeshFilter m_Filter;

    private Mesh m_Mesh;

    private Material m_Material;

    private bool isDestroyInvoked = false;
    public bool IsDestroyInvoked
    {
        get { return isDestroyInvoked; }
    }

    private List<GameObject> meshTriangles = new List<GameObject>();
    public List<GameObject> MeshTriangles
    {
        get { return meshTriangles; }
    }

    public float TrianglesToDestroy
    {
        get => meshTriangles.Count;
    }

    private void Start()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_Filter = GetComponent<MeshFilter>();
        m_Material = m_MeshRenderer.materials[0];
        m_Mesh = m_Filter.mesh;

        GetMeshTrianglesAndDivideThem();
    }

    private void GetMeshTrianglesAndDivideThem()
    {
        m_MeshRenderer.enabled = false;

        Vector3[] vertices = m_Mesh.vertices;
        int[] triangles = m_Mesh.triangles;
        List<int[]> triangleSet = new List<int[]>();
        for (int i = 0; i < triangles.Length; i=i+3)
        {
            int[] eachTriangle = new int[3];
            for (int j = 0; j<3; j++)
            {
                eachTriangle[j] = triangles[i + j];
                Debug.Log(i + j);
            }
            triangleSet.Add(eachTriangle);
        }

        foreach (int[] eachTriangle in triangleSet)
        {
            GameObject go = new GameObject("trianglePart");
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            go.transform.localScale = Vector3.one;
            Mesh mesh = new Mesh();
            MeshFilter triangleMeshFilter = go.AddComponent<MeshFilter>();
            MeshRenderer triangleRenderer = go.AddComponent<MeshRenderer>();
            go.AddComponent<TriangleCtrl>();
            //MeshCollider triangleMeshCollider = go.AddComponent<MeshCollider>();
            Rigidbody rb = go.AddComponent<Rigidbody>();

            List<Vector3> eachTriangleVertices = new List<Vector3>();
            foreach (int vertexIndex in eachTriangle)
            {
                Vector3 localSitePosition = transform.InverseTransformPoint(vertices[vertexIndex]);
                eachTriangleVertices.Add(transform.TransformPoint(localSitePosition));
            }
            mesh.name = "triangle";
            mesh.vertices = eachTriangleVertices.ToArray();
            mesh.triangles = new int[] { 0, 1, 2}; ;

            triangleMeshFilter.mesh = mesh;
            triangleRenderer.material = m_Material;
            rb.useGravity = false;
            //triangleMeshCollider.convex = true;
            //triangleMeshCollider.sharedMesh = mesh;

            meshTriangles.Add(go);

        }
        
    }

    private void StartSendingTrianglesFlyAway()
    {
        StartCoroutine(SendTrianglesFlying());
    }

    private IEnumerator SendTrianglesFlying()
    {
        int trianglesCount = meshTriangles.Count;
        while (meshTriangles.Count >= 0)
        {
            trianglesCount = meshTriangles.Count;
            int trianglesListIndex = Random.Range(0, trianglesCount);
            float force = Random.Range(20f, 30f);
            float timing = Random.Range(0.5f, 1f);
            TriangleCtrl triangleCtrl = meshTriangles[trianglesListIndex].GetComponent<TriangleCtrl>();
            triangleCtrl.ObjRigidbody.AddForce(((triangleCtrl.ObjTransform.position - Vector3.zero).normalized) * force);
            triangleCtrl.StartTimerToDestroyObject(10f);
            meshTriangles.Remove(triangleCtrl.gameObject);
            yield return new WaitForSeconds(timing);
        }

        if(trianglesCount < 0)
        {
            StopCoroutine("SendTrianglesFlying");
            Destroy(gameObject);
        }
    }

    public void SendTrianglesFlyingOnInteraction()
    {
        int trianglesListCount = meshTriangles.Count;
        float timerToDestroy = 10f;
        if (trianglesListCount > 0)
        {
            float force = Random.Range(20f, 30f);
            TriangleCtrl triangleCtrl = meshTriangles[trianglesListCount-1].GetComponent<TriangleCtrl>();
            triangleCtrl.ObjRigidbody.AddForce(((triangleCtrl.ObjTransform.position - Vector3.zero).normalized) * force);
            triangleCtrl.StartTimerToDestroyObject(timerToDestroy);
            meshTriangles.Remove(triangleCtrl.gameObject);
        }
        else 
        {
            if (!isDestroyInvoked)
            {
                isDestroyInvoked = true;
                Invoke("DestroyPlane", timerToDestroy);
            }
        }
    }

    private void DestroyPlane()
    {
        Destroy(gameObject);
    }

}
