using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StretchSense
{
    public class GloveStatusUpdater : MonoBehaviour
    {
        [Header("Performer Settings")]
        [Space(10)]

        [Tooltip("The target performer to show glove status updates for. You must set this to the performer for which you want to see the glove status info.")]
        public int performerId;
        [Tooltip("The handedness of the glove to show status updates for. You must set this to match the left or right hand of your performer in order to see the glove status info for that hand.")]
        public OpenSDKHandedness handedness;

        [Tooltip("The timeout in seconds to wait for a glove status update before showing the glove as disconnected.")]
        public int timeout = 3;

        [Header("Visual Settings")]
        [Space(10)]

        [Tooltip("The label showing the battery percentage")]
        public TMP_Text batteryLabel;
        [Tooltip("The TextMeshPro sprite to update with the performer's staged status.")]
        public Image batteryIndicator;
        [Tooltip("The sprite animation rate (in seconds)")]
        public float animInterval = 1.0f;


        [Header("Battery Indicator Sprites")]
        [Space(10)]

        [Tooltip("The sprites to cycle through the battery is in an unknown state e.g. PerformerGloveStatus has been received via the Open SDK yet or the glove is switched off.")]
        public Sprite[] unknownSpriteFrames;

        public Sprite emptySprite;
        [Tooltip("The color to show when the battery is critical.")]
        public Sprite criticalSprite;
        [Tooltip("The sprite to show when the battery is low.")]
        public Sprite lowSprite;
        [Tooltip("The sprite to show when the battery is medium.")]
        public Sprite mediumSprite;
        [Tooltip("The sprite to show when the battery is high.")]
        public Sprite highSprite;
        [Tooltip("The sprite to show when the battery is full.")]
        public Sprite fullSprite;

        [Header("DEBUG VARS")]
        [Space(10)]

        [Tooltip("The current connection status of the glove.")]
        public OpenSDKConnectionStatus connectionStatus = OpenSDKConnectionStatus.UNKNOWN;

        [Space(10)]

        [Tooltip("The current frame of animated sprites")]
        private int m_CurrentFrameIndex = 0;

        [SerializeField]
        [Tooltip("The last time in seconds when the glove status was updated.")]
        private int m_LastUpdateTime = 0;

        [SerializeField]
        [Tooltip("The current time counter in seconds since Start() was called")]
        private int m_CurrentPlayTime = 0;

        [SerializeField]
        [Tooltip("The raw PerformerGloveStatus information.")]
        public PerformerGloveStatus gloveStatus;


        private void OnEnable()
        {
            OpenSDKActions.PerformerGloveStatusReceived += UpdateGloveStatus;
        }

        private void OnDisable()
        {
            OpenSDKActions.PerformerGloveStatusReceived -= UpdateGloveStatus;
        }

        private void Start()
        {
            if(batteryLabel == null)
            {
                batteryLabel = GetComponentInChildren<TMP_Text>();
            }

            if(batteryIndicator == null)
            {
                batteryIndicator = GetComponentInChildren<Image>();
            }

            StartCoroutine("UpdateVisuals");
        }
        private void OnDestroy()
        {
            StopCoroutine("UpdateVisuals");
        }

        IEnumerator UpdateVisuals()
        {
            while(true)
            {
                PerformerGloveStatus status = gloveStatus;
                
                m_CurrentPlayTime = System.DateTime.Now.Second;

                if (m_LastUpdateTime != 0 && (m_CurrentPlayTime - m_LastUpdateTime) > timeout)
                {
                    connectionStatus = OpenSDKConnectionStatus.DISCONNECTED;
                }

                CheckStatus(status);


                yield return new WaitForSeconds(animInterval);
            }
        }

        private void CheckStatus(PerformerGloveStatus status)
        {
            if (connectionStatus == OpenSDKConnectionStatus.CONNECTED)
            {
                UpdateUI(status.performerId, status.profileHandedness, status.batteryLevel, GetBatteryIndicatorSprite(status.batteryLevel));
            }
            else
            {
                Sprite sprite = unknownSpriteFrames[m_CurrentFrameIndex];
                m_CurrentFrameIndex = (m_CurrentFrameIndex + 1) % unknownSpriteFrames.Length; // Cycle through frames
                UpdateUI(status.performerId, status.profileHandedness, status.batteryLevel, sprite);
            }
        }

        public void UpdateGloveStatus(List<PerformerGloveStatus> allGloves)
        {
            PerformerGloveStatus gloveStatus = allGloves.FindLast(glove => glove.performerId == performerId && glove.profileHandedness == handedness);
            connectionStatus = OpenSDKConnectionStatus.CONNECTED;
            this.gloveStatus = gloveStatus;

            CheckStatus(gloveStatus);
            m_LastUpdateTime = System.DateTime.Now.Second;
        }

        private Sprite GetBatteryIndicatorSprite(float batteryLevel)
        {
            if (batteryLevel > 0.9)
            {
                return fullSprite;

            }else if (batteryLevel <= 0.9 && batteryLevel > 0.5)
            {
                return highSprite;
            }
            else if (batteryLevel <= 0.5 && batteryLevel > 0.25)
            {
                return mediumSprite;
            }
            else if (batteryLevel <= 0.25 && batteryLevel > 0.1)
            {
                return lowSprite;
            }
            else if (batteryLevel <= 0.1 && batteryLevel > 0)
            {
                return criticalSprite;
            }
            else
            {
                return emptySprite;
            }
        }

        private void UpdateUI(int performerId, OpenSDKHandedness handedness, float batteryLevel, Sprite batteryIndicatorSprite)
        {
            string handLabel = (handedness == OpenSDKHandedness.UNKNOWN) ? "?" : handedness.ToString()[0].ToString();
            batteryLabel.text = $"{handLabel}: {batteryLevel*100}%";
            batteryIndicator.overrideSprite = batteryIndicatorSprite;
        }
    }
}