using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace StretchSense
{
    public class FPSGraph : MonoBehaviour
    {
        public enum DataSource
        {
            // The number of frames per second the application is running at. This is calculated from the time between frames
            FPS,
            // The number of bone rotation animation frames received from OpenSDK per frame on the background thread
            BoneRotationBGMessages,
            // The number of bone rotation animation frames received from OpenSDK per frame on the main thread
            BoneRotationMainMessages,
            // The memory usage of the application in megabytes
            MemoryUsage
        }

        [Tooltip("The data source for the graph.")]
        public DataSource dataSource = DataSource.FPS;

        [SerializeField]
        [Tooltip("The input action to cycle the view mode of the graph. Can be set up to be a gamepad, VR controller button or a keyboard command. Default is spacebar.")]
        public InputActionReference cycleActionRef;

        public float graphWidth = 1400f;
        public float graphHeight = 50f;
        public float graphScale = 0.0007f;
        [Tooltip("The number of seconds of history to store in the graph. The larger this number the smoother the graph.")]
        public int graphHistory = 500;
        public float textScale = 0.003f;
        public int fontSize = 24;
        public float graphLineHeight = 19.2f;
        public float graphLineXOffset = -2f;
        public float lineWidth = -0.01f;

        public float optimalFPS = 60f; // Configurable optimal FPS value
        public Color optimalColor = new Color(0f, 1f, 0f, 1f); // Green for good FPS
        public Color warningColor = new Color(1f, 0.8f, 0f, 1f); // Yellow for warning FPS
        public Color criticalColor = new Color(1f, 0f, 0f, 1f); // Yellow for critical FPS

        public float historyAverageFPS => GetGraphHistoryAverage(fpsHistoryQueue);
        public float historyAverageAnimationBackground => GetGraphHistoryAverage(animationHistoryBackgroundQueue);

        public float historyAverageAnimationMain => GetGraphHistoryAverage(animationHistoryMainQueue);


        public Vector3 textOffset = new Vector3(-0.98f, 0.73f, -0.01f);
        public Vector3 graphOffset = new Vector3(-1f, 0.25f, -0.01f);
        public Quaternion graphRotationOffset = Quaternion.Euler(0, 180, 0);

        public Font font;

        [Tooltip("An optimal FPS for the application to run at. If the application is running at this frame rate or higher the graph will be green.")]
        public int fpsOptimal = 120;
        [Tooltip("An average FPS for the application to run at. If the application is running at this frame rate or higher the graph will be yellow.")]
        public int fpsAverage = 60;

        [Tooltip("An optimal number of bone rotation animations to be received per frame.")]
        public int boneAnimationOptimal = 30;

        [Tooltip("The number of performers staged and calibrated in Hand Engine. This will adjust the range of the graph when DataSource.BoneRotationMessages is set to compensate for the amount of extra expected messages.")]
        [Range(1, 6)]
        public int performerCount = 1;

        [Header("DEBUG VARS")]
        [Space(10)]

        [Tooltip("The number of frames per second the application is running at. This is calculated from the time between frames")]
        [SerializeField]
        private int currentFPS = 0;
        [Tooltip("The number of bone rotation animation frames received from OpenSDK per frame on the background thread")]
        [SerializeField]
        private int boneRotationBackgroundCount = 0;
        [Tooltip("The number of bone rotation animation frames received from OpenSDK per frame on the main thread. This is not shown in the inspector as it is zeroed every frame after drawing the graph.")]
        private int boneRotationMainCount = 0;

        

        private float deltaTime = 0.0f;

        private Queue<float> fpsHistoryQueue;
        private Queue<float> animationHistoryBackgroundQueue;
        private Queue<float> animationHistoryMainQueue;
        private Queue<float> memoryHistoryQueue;

        private LineRenderer lineRenderer;
        private RectTransform canvasRectTransform;
        private Text fpsText;
        private GameObject graphObject;

        private Color[] lineColors; // Array to store colors for each frame

        void Start()
        {
            fpsHistoryQueue = new Queue<float>(graphHistory);
            animationHistoryBackgroundQueue = new Queue<float>(graphHistory);
            animationHistoryMainQueue = new Queue<float>(graphHistory);
            memoryHistoryQueue = new Queue<float>(graphHistory);

            // Setup Line Renderer
            graphObject = new GameObject("Line Graph");
            
            graphObject.transform.SetParent(transform);

            lineRenderer = graphObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            
            lineRenderer.positionCount = graphHistory;
            lineRenderer.useWorldSpace = false; // Important!
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            // Get Canvas RectTransform
            canvasRectTransform = GetComponent<RectTransform>();

            // Setup FPS Text
            GameObject fpsTextObject = new GameObject("Label");
            fpsTextObject.transform.SetParent(transform);
            fpsTextObject.transform.localRotation = Quaternion.Euler(Vector3.forward);
            fpsTextObject.transform.localScale = new Vector3(textScale, textScale, textScale);
            fpsText = fpsTextObject.AddComponent<Text>();
            fpsText.font = font;
            fpsText.fontSize = fontSize;
            fpsText.rectTransform.pivot = new Vector2(0, 1);
            // Add Content Size Fitter
            ContentSizeFitter contentSizeFitter = fpsTextObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        void Update()
        {
            currentFPS = (int)GetFPSData();

            // Get new data from source
            float newData = -1;
            Queue<float> targetData;
            switch (dataSource)
            {
                default:
                case DataSource.FPS:
                    newData = currentFPS;
                    targetData = fpsHistoryQueue;
                    // Add new data to target data queue
                    EnqueueData(targetData, newData);
                    break;
                case DataSource.BoneRotationBGMessages:
                    newData = GetBoneRotationBackground();
                    targetData = animationHistoryBackgroundQueue;
                    EnqueueData(targetData, newData);
                    break;
                case DataSource.BoneRotationMainMessages:
                    newData = GetBoneRotationMain();
                    targetData = animationHistoryMainQueue;
                    EnqueueData(targetData, newData);
                    break;
                case DataSource.MemoryUsage:
                    targetData = memoryHistoryQueue;
                    // Add new data to target data queue
                    EnqueueData(targetData, newData);
                    break;
            }

            // Update line graph
            DrawGraphData(targetData);
            UpdateFPSLineColors(historyAverageFPS);
            UpdateAnimationLineColors(historyAverageAnimationBackground);
            UpdateAnimationLineColors(historyAverageAnimationMain);

            // Reset message counters
            ResetBoneRotationBackground();
            ResetBoneRotationMain();

            // Update FPS text
            fpsText.text = $"{dataSource}: {Mathf.RoundToInt(newData)}";
            graphObject.transform.localScale = new Vector3(transform.localScale.x * graphScale, transform.localScale.y * graphScale, transform.localScale.z * graphScale);
            graphObject.transform.localPosition = graphOffset;
            fpsText.transform.localPosition = textOffset;

        }

        private void OnEnable()
        {
            OpenSDKActions.BoneRotationBackgroundReceived += OnBoneRotationBackground;
            OpenSDKActions.BoneRotationMainReceived += OnBoneRotationMain;

            cycleActionRef.action.Enable();
            cycleActionRef.action.performed += OnCycle;
        }

        private void OnDisable()
        {
            OpenSDKActions.BoneRotationBackgroundReceived -= OnBoneRotationBackground;
            OpenSDKActions.BoneRotationMainReceived -= OnBoneRotationMain;

            cycleActionRef.action.Disable();
            cycleActionRef.action.performed -= OnCycle;
        }

        private void OnCycle(InputAction.CallbackContext context)
        {
            CycleViewMode();
        }

        private void CycleViewMode()
        {
            switch (dataSource)
            {
                case DataSource.FPS:
                    dataSource = DataSource.BoneRotationBGMessages;
                    break;
                case DataSource.BoneRotationBGMessages:
                    dataSource = DataSource.BoneRotationMainMessages;
                    break;
                case DataSource.BoneRotationMainMessages:
                    dataSource = DataSource.FPS;
                    break;
            }

            Debug.Log($"Switched to {dataSource} mode");
        }

        private void OnBoneRotationBackground(List<AnimationBoneRotation> boneRotations, int timecode, bool hasMetacarpalBone)
        {
            // Increment the bone rotation animation data count from the background thread
            // and ignore the incoming data as we don't want to graph it
            IncrementBackgroundDataCount();
        }

        private void OnBoneRotationMain()
        {
            // Increment the bone rotation animation data count from the main thread
            IncrementMainDataCount();
        }

        private void EnqueueData(Queue<float> targetData, float newData)
        {
            targetData.Enqueue(newData);
            if (targetData.Count > graphHistory)
            {
                targetData.Dequeue();
            }
        }

        /**
         * Increment the bone rotation animation data count from the background thread
         */
        public void IncrementBackgroundDataCount()
        {
            boneRotationBackgroundCount++;
        }

        /**
         * Increment the bone rotation animation data count from the main thread
         */
        public void IncrementMainDataCount()
        {
            boneRotationMainCount++;
        }

        private float GetFPSData()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;

            return fps;
        }

        private float GetBoneRotationBackground()
        {
            int newData = boneRotationBackgroundCount + 0;
            return newData;
        }

        private void ResetBoneRotationBackground()
        {
            boneRotationBackgroundCount = 0;
        }

        private float GetBoneRotationMain()
        {
            int newData = boneRotationMainCount + 0;
            return newData;
        }

        private void ResetBoneRotationMain()
        {
            boneRotationMainCount = 0;
        }

        private float GetGraphHistoryAverage(Queue<float> history)
        {
            float sum = 0;
            foreach (float data in history)
            {
                sum += data;
            }
            return sum / history.Count;
        }

        void DrawGraphData(Queue<float> targetData)
        {
            float xIncrement = (graphWidth / (float)(graphHistory - 1));
            float yScale = graphHeight / graphLineHeight;

            Vector3[] positions = new Vector3[targetData.Count];
            int i = 0;
            foreach (float frameRate in targetData)
            {
                positions[i] = new Vector3(i * xIncrement - graphLineXOffset, frameRate * yScale, 0);
                NormalizePositionToCanvas(ref positions[i]);
                
                i++;
            }
            lineRenderer.widthMultiplier = lineWidth;
            lineRenderer.SetPositions(positions);
            lineRenderer.transform.localRotation = graphRotationOffset;
        }

        void UpdateFPSLineColors(float average)
        {
            if (dataSource != DataSource.FPS)
            {
                return;
            }

            if (average > fpsOptimal)
            {
                lineRenderer.startColor = optimalColor;
                lineRenderer.endColor = optimalColor;
            }
            else if (average > fpsAverage)
            {
                lineRenderer.startColor = warningColor;
                lineRenderer.endColor = warningColor;
            }
            else
            {
                lineRenderer.startColor = criticalColor;
                lineRenderer.endColor = criticalColor;
            }
        }

        void UpdateAnimationLineColors(float average)
        {
            if(!(dataSource == DataSource.BoneRotationBGMessages || dataSource == DataSource.BoneRotationMainMessages))
            {
                return;
            }

            float optimal = boneAnimationOptimal * performerCount;

            float lowerBound = optimal * 0.5f; // 50% below optimal
            float upperBound = optimal * 1.5f; // 50% above optimal
            float criticalUpperBound = optimal * 2f; // 200% above optimal
            float criticalLowerBound = optimal * 0.1f; // 90% below optimal

            if (average >= lowerBound && average <= upperBound)
            {
                lineRenderer.startColor = optimalColor;
                lineRenderer.endColor = optimalColor;
            }else if(average >= criticalUpperBound || average < criticalLowerBound)
            {
                lineRenderer.startColor = criticalColor;
                lineRenderer.endColor = criticalColor;
            }
            else
            {
                lineRenderer.startColor = warningColor;
                lineRenderer.endColor = warningColor;
            }
        }

        // Helper function to normalize positions within canvas
        void NormalizePositionToCanvas(ref Vector3 position)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, position, null, out pos);
            position = canvasRectTransform.anchoredPosition + pos;
        }
    }
}