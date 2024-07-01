using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleCtrl : MonoBehaviour
{
    private MeshRenderer m_MeshRenderer;
    private MeshFilter m_MeshFilter;
    private MeshCollider m_MeshCollider;

    public MeshRenderer ObjMeshRenderer
    {
        get { return m_MeshRenderer; }
    }

    public MeshFilter ObjMeshFilter
    {
        get { return m_MeshFilter; }
    }

    public MeshCollider ObjMeshCollider
    {
        get { return m_MeshCollider; }
    }

    private void Awake()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_MeshFilter = GetComponent<MeshFilter>();
        m_MeshCollider = GetComponent<MeshCollider>();
    }
}
