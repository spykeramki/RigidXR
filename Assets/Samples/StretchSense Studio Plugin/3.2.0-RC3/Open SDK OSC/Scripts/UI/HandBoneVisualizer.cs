using System.Collections.Generic;
using UnityEngine;

namespace StretchSense
{
    public class HandBoneVisualizer : MonoBehaviour
    {
        public int performerId = 0;
        public OpenSDKHandedness handedness = OpenSDKHandedness.UNKNOWN;
        [SerializeField]
        [Tooltip("The order of sliders from the root bone to the tip.")]
        public List<OpenSDKSlider> sliderChain = new();
        public Transform rootBone;
        public Gradient lineGradient;
        [Tooltip("Render the line in reverse order")]
        public bool renderInReverse = false;
        public GameObject spherePrefab;
        public Color sphereColor = Color.white;
        public float sphereRadius = 0.01f;
        private GameObject[] sphereObjects;
        [SerializeField]
        private Transform[] handBones;
        private LineRenderer lineRenderer;

        void Start()
        {
            if(sliderChain.Count == 0)
            {
                Debug.LogWarning("Slider chain is empty. Please assign sliders in the order the respective bend joints are present in the character's bone heirarchy.");
            }

            Animator animator = GetComponent<Animator>();
            handBones = GetHandBones(animator,rootBone);

            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.005f;
            lineRenderer.endWidth = 0.005f;
            lineRenderer.material = new Material(Shader.Find("TextMeshPro/Distance Field Overlay"));
            lineRenderer.colorGradient = lineGradient; // Set the color gradient
            lineRenderer.numCornerVertices = 5;
            lineRenderer.numCapVertices = 5;

            sphereObjects = new GameObject[handBones.Length];
            CreateSpheres();

        }

        void FixedUpdate()
        {
            if (handedness == OpenSDKHandedness.UNKNOWN)
            {
                HideSpheres();
                lineRenderer.positionCount = 0; // Hide the line renderer
                return;
            }

            UpdateHandVisualization(handBones);
            UpdateSpherePositions();
        }

        private void CreateSpheres()
        {
            if(spherePrefab == null)
            {
                Debug.LogWarning("Sphere prefab is null. Please assign a prefab to the HandBoneVisualizer script in the inspector.");
                return;
            }

            for (int i = 0; i < handBones.Length; i++)
            {
                sphereObjects[i] = Instantiate(spherePrefab);
                sphereObjects[i].transform.parent = transform; // Keep hierarchy tidy
                sphereObjects[i].transform.position = handBones[i].position;
                sphereObjects[i].transform.localScale = Vector3.one * sphereRadius;

                SliderVisualizer sliderVisualizer = sphereObjects[i].GetComponent<SliderVisualizer>();
                sliderVisualizer.performerId = performerId;
                sliderVisualizer.handedness = handedness;

                if (sliderChain.Count > 0 && i > 0 && i - 1 < sliderChain.Count)
                {
                    sliderVisualizer.sliderType = sliderChain[i-1];
                }

                if (sliderVisualizer.sliderType == OpenSDKSlider.UNKNOWN)
                {
                    sliderVisualizer.Hide();
                }
                else
                {
                    sliderVisualizer.Show();
                }

                

                Renderer renderer = sphereObjects[i].GetComponent<Renderer>();
                renderer.material = new Material(Shader.Find("TextMeshPro/Distance Field Overlay"));
                renderer.material.SetColor("_FaceColor", sphereColor); // Set the color gradient
            }
        }

        private void UpdateSpherePositions()
        {
            if (spherePrefab == null)
            {
                return;
            }

            for (int i = 0; i < handBones.Length; i++)
            {
                if(sphereObjects[i] == null || handBones[i] == null)
                {
                    continue;
                }

                sphereObjects[i].transform.position = handBones[i].position;
            }
        }

        private void HideSpheres()
        {
            foreach (GameObject sphere in sphereObjects)
            {
                sphere.SetActive(false);
            }
        }

        private void UpdateHandVisualization(Transform[] handBones)
        {
            // Ensure positionCount is correct first
            lineRenderer.positionCount = handBones.Length;

            if (renderInReverse)
            {
                // Render lines in reverse order for correct directionality
                for (int i = handBones.Length - 1; i >= 0; i--)
                {
                    lineRenderer.SetPosition(handBones.Length - 1 - i, handBones[i].position);
                }
            }
            else
            {
                // Normal rendering order
                for (int i = 0; i < handBones.Length; i++)
                {
                    lineRenderer.SetPosition(i, handBones[i].position);
                }
            }
        }

        private Transform[] GetHandBones(Animator animator, Transform rootBone)
        {
            List<Transform> boneList = new List<Transform>();
            boneList.Add(rootBone.parent);
            boneList.Add(rootBone);

            // Recurse through finger bones
            CollectFingerBonesRecursively(rootBone, boneList);

            return boneList.ToArray();
        }

        private void CollectFingerBonesRecursively(Transform parentBone, List<Transform> boneList)
        {
            for (int i = 0; i < parentBone.childCount; i++)
            {
                Transform childBone = parentBone.GetChild(i);
                boneList.Add(childBone);
                CollectFingerBonesRecursively(childBone, boneList);
            }
        }

    }
}