using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script to give access to the triangle data od meshes 
public class TriangleCtrl : MonoBehaviour
{
    public MeshRenderer m_MeshRenderer;
    public MeshFilter m_MeshFilter;
    public Rigidbody m_Rigidbody;
    public Transform ObjTransform;

    public MeshRenderer ObjMeshRenderer
    {
        get { return m_MeshRenderer; }
    }

    public MeshFilter ObjMeshFilter
    {
        get { return m_MeshFilter; }
    }

    public Rigidbody ObjRigidbody
    {
        get { return m_Rigidbody; }
    }



    private void Start()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_MeshFilter = GetComponent<MeshFilter>();
        m_Rigidbody = GetComponent<Rigidbody>();
        ObjTransform = transform;
    }

    //Destroying the triangle after some time of moving away from origin
    public void StartTimerToDestroyObject(float timer)
    {
        StartCoroutine(TimerToDestroyObject(timer));
    }

    private IEnumerator TimerToDestroyObject(float timer)
    {
        while(timer > 0)
        {
            yield return new WaitForSeconds(1f);
            timer--;
        }
        if(timer <= 0)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
