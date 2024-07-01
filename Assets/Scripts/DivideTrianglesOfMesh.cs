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

    private List<GameObject> meshTriangles = new List<GameObject>();
    public List<GameObject> MeshTriangles
    {
        get { return meshTriangles; }
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
            //MeshCollider triangleMeshCollider = go.AddComponent<MeshCollider>();

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
            //triangleMeshCollider.convex = true;
            //triangleMeshCollider.sharedMesh = mesh;

            meshTriangles.Add(go);

        }
    }

}
