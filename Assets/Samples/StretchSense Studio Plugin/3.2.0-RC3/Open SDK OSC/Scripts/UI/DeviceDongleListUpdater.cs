using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace StretchSense
{
    public class DeviceDongleListUpdater : MonoBehaviour
    {
        [Header("Dongle Settings")]
        [Space(10)]

        [Tooltip("The timeout in seconds to wait for a dongle status update before showing the dongle as disconnected.")]
        public int timeout = 3;

        [Header("Visual Settings")]
        [Space(10)]

        [Tooltip("The UI prefab to instantiate for each dongle.")]
        public GameObject dongleUIPrefab;
        [Tooltip("The layout group to parent the dongle UI elements under.")]
        public Transform layoutGroupTransform;

        [Tooltip("The sprite animation rate (in seconds)")]
        public float animInterval = 1.0f;

        [Header("Dongle Indicator Sprites")]
        [Space(10)]

        [Tooltip("The sprite to show when the dongle is connected.")]
        public Sprite connectedSprite;
        [Tooltip("The sprites to cycle through the dongle is in an unknown state e.g. DeviceDongleStatus has been received via the Open SDK yet or the dongle is unplugged.")]
        public Sprite[] unknownSpriteFrames;

        [Header("DEBUG VARS")]
        [Space(10)]

        public List<DeviceDongleStatus> deviceStatuses;
        [Tooltip("The current connection status of previously connected dongles.")]
        public SerializableDictionary<string, OpenSDKConnectionStatus> connectionStatuses;

        [Space(10)]

        private Dictionary<string, GameObject> m_UIElements = new Dictionary<string, GameObject>();

        [Tooltip("The current frame of animated sprites")]
        private int m_CurrentFrameIndex = 0;

        [SerializeField]
        [Tooltip("The last time in seconds when the dongle status was updated.")]
        private int m_LastUpdateTime = 0;

        [SerializeField]
        [Tooltip("The current time counter in seconds since Start() was called")]
        private int m_CurrentPlayTime = 0;

        private void Start()
        {
            StartCoroutine("UpdateVisuals");
        }
        private void OnDestroy()
        {
            StopCoroutine("UpdateVisuals");
        }

        IEnumerator UpdateVisuals()
        {
            while (true)
            {
                m_CurrentPlayTime = System.DateTime.Now.Second;

                if (m_LastUpdateTime != 0 && (m_CurrentPlayTime - m_LastUpdateTime) > timeout)
                {
                    deviceStatuses.ForEach(dongle =>
                    {
                        bool hasDongleId = connectionStatuses.ToDictionary().ContainsKey(dongle.dongleId);

                        if (hasDongleId)
                        {
                            //Update the existing connection status
                            connectionStatuses.Set(dongle.dongleId, OpenSDKConnectionStatus.DISCONNECTED);
                        }
                    });
                }

                PopulateUI();
                yield return new WaitForSeconds(animInterval);
            }
        }


        public void UpdateStatuses(List<DeviceDongleStatus> newStatuses)
        {
            deviceStatuses = newStatuses;

            deviceStatuses.ForEach(deviceStatuses =>
            {
                connectionStatuses.Set(deviceStatuses.dongleId, OpenSDKConnectionStatus.CONNECTED);
            });

            m_LastUpdateTime = System.DateTime.Now.Second;
        }

        void PopulateUI()
        {
            foreach (DeviceDongleStatus dongle in deviceStatuses)
            {
                if(dongle.dongleId == null)
                {
                    continue;
                }

                DongleStatusSingleUpdater updater = GetUpdater(dongle);

                if (updater != null)
                {
                    CheckStatus(dongle, updater);
                }
            }
        }

        void CheckStatus(DeviceDongleStatus dongle, DongleStatusSingleUpdater updater)
        {
            OpenSDKConnectionStatus connectionStatus = OpenSDKConnectionStatus.DISCONNECTED;

            if (connectionStatuses.ToDictionary().ContainsKey(dongle.dongleId))
            {
                connectionStatus = connectionStatuses.ToDictionary()[dongle.dongleId];
            }

            if (connectionStatus == OpenSDKConnectionStatus.CONNECTED)
            {
                updater.UpdateUI(dongle.dongleId, dongle.firmwareVersion, dongle.versionStatus, connectedSprite);
            }
            else
            {
                Sprite sprite = unknownSpriteFrames[m_CurrentFrameIndex];
                m_CurrentFrameIndex = (m_CurrentFrameIndex + 1) % unknownSpriteFrames.Length; // Cycle through frames
                updater.UpdateUI(dongle.dongleId, dongle.firmwareVersion, dongle.versionStatus, sprite);
            }
        }

        DongleStatusSingleUpdater GetUpdater(DeviceDongleStatus dongle)
        {
            DongleStatusSingleUpdater updater;

            if (m_UIElements.ContainsKey(dongle.dongleId))
            {
                // Update existing UI element
                updater = m_UIElements[dongle.dongleId].GetComponent<DongleStatusSingleUpdater>();
            }
            else
            {
                GameObject newUIElement = Instantiate(dongleUIPrefab, layoutGroupTransform);
                // Store reference with id
                m_UIElements[dongle.dongleId] = newUIElement;
                updater = newUIElement.GetComponent<DongleStatusSingleUpdater>();
            }

            return updater;
        }
    }
}