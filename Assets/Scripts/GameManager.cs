using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GenerateDestroyablePlane destroyablePlaneParentCtrl;

    private float _skyboxRotation = 0f;

    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        _skyboxRotation += Time.deltaTime;
        //Rotating skybox in the Mixed To VirtualReality transition Scene
        if(_skyboxRotation >= 360f)
        {
            _skyboxRotation = 0f;
        }
        RenderSettings.skybox.SetFloat("_Rotation", _skyboxRotation);
    }
}
