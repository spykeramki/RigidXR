using UnityEngine;
using OscCore;
using System.Collections.Generic;
using UnityEditor;
using BlobHandles;
using UnityEngine.Events;
using System.Net.Sockets;

namespace StretchSense
{
    public class OpenSDKReceiver : MonoBehaviour
    {
        [Tooltip("The port to listen for incoming OSC messages on. Make sure this matches what is set in Hand Engine's Open SDK settings.")]
        public int oscPort = 9400;

        [Tooltip("The list of OpenSDKApplicatorAdvanced components to bind to. If m_AutoSetupMessageHandlers is true, this list will be populated with the OpenSDKApplicatorAdvanced components found in the scene.")]
        public List<OpenSDKApplicatorAdvanced> applicators = new();

        [Tooltip("Whether or not to store incoming messages in the DebugOutput property for inspection. Disable to increase performance.")]
        public bool debugMode = false;

        [Header("EVENTS")]
        [Space(10)]
        [Tooltip("Event for when the bone rotation data is processed via OpenSDK on the main thread")]
        [SerializeField]
        UnityEvent<AnimationBoneRotationFrame> m_OnBoneRotationReceived = new UnityEvent<AnimationBoneRotationFrame>();

        [Space(10)]
        [Tooltip("Event for when the bone rotation metacarpal data is processed via OpenSDK on the main thread")]
        [SerializeField]
        UnityEvent<AnimationBoneRotationFrame> m_OnBoneMetacarpalReceived = new UnityEvent<AnimationBoneRotationFrame>();


        [Tooltip("Event for when a performer is received via Open SDK")]
        [SerializeField]
        UnityEvent<Performer[]> m_OnPerformerReceived = new UnityEvent<Performer[]>();

        [Tooltip("Event for when a performer glove status update is received")]
        [SerializeField]
        UnityEvent<PerformerGloveStatus[]> m_OnGloveStatusReceived = new UnityEvent<PerformerGloveStatus[]>();

        [Tooltip("Event for when hand slider data is received")]
        [SerializeField]
        UnityEvent<HandSlider[]> m_OnHandSliderReceived = new UnityEvent<HandSlider[]>();

        [Tooltip("Event for when dongle data is received")]
        [SerializeField]
        UnityEvent<DeviceDongleStatus[]> m_OnDongleStatusReceived = new UnityEvent<DeviceDongleStatus[]>();

        [Header("DEBUG VARS")]
        [Space(10)]
        [SerializeField]
        [Tooltip("All incoming OSC messages in their raw form. Brackets before each OSC attribute content indicates the data type. A type of (unknown) means the incoming attribute isn't currently supported by the DebugOutput list.")]
        // Dictionary mapping jointName to AnimationBone data
        public SerializableDictionary<string, string> m_DebugOutput = new();

        [SerializeField]
        public AnimationBoneRotationFrame lastBoneRotationData;
        private OscServer m_Server;

        [SerializeField]
        [Tooltip("The OSC receiver component that sets up the connection with Hand Engine.")]
        OscReceiver oscReceiver;

        [SerializeField]
        [Tooltip("The list of performers in Hand Engine")]
        private SerializableDictionary<string, Performer> m_Performers = new();
        public SerializableDictionary<string, Performer> Performers { get => m_Performers; set => m_Performers = value; }

        [SerializeField]
        [Tooltip("The list of gloves connected to performers in Hand Engine")]
        private SerializableDictionary<string, PerformerGloveStatus> m_Gloves = new();
        public SerializableDictionary<string, PerformerGloveStatus> Gloves { get => m_Gloves; set => m_Gloves = value; }

        [SerializeField]
        [Tooltip("The list of dongles connected to performers in Hand Engine")]
        private SerializableDictionary<string, DeviceDongleStatus> m_Dongles = new();
        public SerializableDictionary<string, DeviceDongleStatus> Dongles { get => m_Dongles; set => m_Dongles = value; }

        [SerializeField]
        [Tooltip("The list of sliders for each hand in Hand Engine")]
        private SerializableDictionary<string, HandSlider> m_HandSliders = new();
        public SerializableDictionary<string, HandSlider> HandSliders { get => m_HandSliders; set => m_HandSliders = value; }

        void Start()
        {
            oscReceiver = gameObject.GetComponent<OscReceiver>();

            if (oscReceiver == null)
            {
                try
                {
                    oscReceiver = gameObject.AddComponent<OscReceiver>();
                    oscReceiver.Port = oscPort;
                }
                catch (SocketException exception)
                {
                    Debug.LogWarning("Could not initialize StretchSense Open SDK due to socket error. If running a standalone device, check your application has network permissions.");
                    Debug.LogWarning(exception.Message);
                }
            }

            m_Server = oscReceiver.Server;

            if (!oscReceiver.Running)
            {
                Debug.Log("Could not initialize OSC.");
                return;
            }

            InitAutoSetup();

            m_Server.TryAddMethodPair("/v1/animation/slider/all", HandleSliderMessageBackground, HandleSliderMessageMain);
            m_Server.TryAddMethodPair("/v1/performer/all", HandlePerformerMessageBackground, HandlePerformerMessageMain);
            m_Server.TryAddMethodPair("/v1/performer/glove/status", HandleGloveMessageBackground, HandleGloveMessageMain);
            m_Server.TryAddMethodPair("/v1/device/dongle/status", HandleDongleMessageBackground, HandleDongleMessageMain);

            if (debugMode)
            {
                m_Server.AddMonitorCallback(OnMonitorOscMessage);
            }
        }

        /**
         * This method is called when an OSC message is received. It is used to store the incoming message in the
         * DebugOutput property for inspection.
         * 
         * Don't use this method to extract data from the OSC message, use `TryAddMethodPair()` for each `performerId` and
         * `HandleBoneMessageBackground` instead. Or alternately manully add `OscQuaternionMessageHandler` components for
         * each `performerId` and `BoneName` to the `GameObject` with `OpenSDKApplicatorAdvanced` component on it. The assign
         * the `OpenSDKReceiver` reference and configure `OnMessageReceived` callback for `OpenSDKApplicatorAdvanced.UpdateBoneData()`.
         * 
         * @param address The OSC address of the incoming message
         * @param values The OSC message values
         */
        private void OnMonitorOscMessage(BlobString address, OscMessageValues values)
        {
            string debugOutput = "";

            values.ForEachElement((index, typeTag) =>
            {
                string prefix = "";
                if (debugOutput != "")
                {
                    prefix = ",";
                }
                switch (typeTag)
                {
                    case TypeTag.Float32:
                        debugOutput += $"{prefix}(f):{values.ReadFloatElement(index)}";
                        break;
                    case TypeTag.Int32:
                        debugOutput += $"{prefix}(i):{values.ReadIntElement(index)}";
                        break;
                    case TypeTag.String:
                        debugOutput += $"{prefix}(s):{values.ReadStringElement(index)}";
                        break;
                    case TypeTag.Blob:
                        byte[] blob = new byte[0];
                        values.ReadBlobElement(index, ref blob);
                        debugOutput += $"{prefix}(b):{blob}";
                        break;
                    default:
                        debugOutput += $"{prefix}(unknown)";
                        break;
                }
            });

            m_DebugOutput.Set(address.ToString(), debugOutput);
        }

        private void OnApplicationQuit()
        {
            StopServer();
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
#endif
            Debug.Log("Closing OSC server");

            StopServer();
        }

        /*
         * If valid, automatically bind to the joint names provided in the OpenSDKApplicatorAdvanced component
         */
        private void InitAutoSetup()
        {
            if (applicators.Count == 0)
            {
                applicators.AddRange(FindObjectsOfType<OpenSDKApplicatorAdvanced>(true));
            }

            foreach (OpenSDKApplicatorAdvanced applicator in applicators)
            {
                BindReceiverAddresses(applicator, OpenSDKHandedness.LEFT, OpenSDKFinger.Thumb);
                BindReceiverAddresses(applicator, OpenSDKHandedness.LEFT, OpenSDKFinger.Index);
                BindReceiverAddresses(applicator, OpenSDKHandedness.LEFT, OpenSDKFinger.Middle);
                BindReceiverAddresses(applicator, OpenSDKHandedness.LEFT, OpenSDKFinger.Ring);
                BindReceiverAddresses(applicator, OpenSDKHandedness.LEFT, OpenSDKFinger.Little);

                BindReceiverAddresses(applicator, OpenSDKHandedness.RIGHT, OpenSDKFinger.Thumb);
                BindReceiverAddresses(applicator, OpenSDKHandedness.RIGHT, OpenSDKFinger.Index);
                BindReceiverAddresses(applicator, OpenSDKHandedness.RIGHT, OpenSDKFinger.Middle);
                BindReceiverAddresses(applicator, OpenSDKHandedness.RIGHT, OpenSDKFinger.Ring);
                BindReceiverAddresses(applicator, OpenSDKHandedness.RIGHT, OpenSDKFinger.Little);
            }
        }

        private void BindReceiverAddresses(OpenSDKApplicatorAdvanced applicator, OpenSDKHandedness handedness, OpenSDKFinger finger)
        {
            if (!applicator.autoSetup)
            {
                Debug.LogWarning("OpenSDKApplicatorAdvanced.autoSetup is false, skipping auto setup for this applicator");
                return;
            }
            int performerId = applicator.performerId;
            string addressPathRotation = $"/v1/animation/{performerId}/{(int)handedness}/{finger}/rotation";

            m_Server.TryAddMethodPair(addressPathRotation, (values) => { HandleBoneMessageBackground(values, applicator, performerId, handedness, finger); }, HandleBoneMessageMain);

            string addressPathMetacarpalRotation = $"/v1/animation/{performerId}/{(int)handedness}/{finger}/rotationWithMetacarpals";

            m_Server.TryAddMethodPair(addressPathMetacarpalRotation, (values) => { HandleBoneMetacarpalBackground(values, applicator, performerId, handedness, finger); }, HandleBoneMetacarpalMain);

        }

        public void OnBeforeAssemblyReload()
        {
            StopServer();
        }

        private void StopServer()
        {
            if (debugMode)
            {
                m_Server.RemoveMonitorCallback(OnMonitorOscMessage);
            }
            OscServer.Remove(oscPort);
            m_Server?.Dispose();
        }
        private void HandleDongleMessageMain()
        {
            List<DeviceDongleStatus> dongles = m_Dongles.All;
            m_OnDongleStatusReceived.Invoke(dongles.ToArray());

            if (OpenSDKActions.DeviceDongleStatusReceived != null)
            {
                OpenSDKActions.DeviceDongleStatusReceived.Invoke(dongles);
            }

        }

        private void HandlePerformerMessageMain()
        {
            List<Performer> allPerformers = m_Performers.All;

            m_OnPerformerReceived.Invoke(allPerformers.ToArray());

            if (OpenSDKActions.PerformerReceived != null)
            {
                OpenSDKActions.PerformerReceived.Invoke(allPerformers);
            }
        }

        private void HandleGloveMessageMain()
        {
            List<PerformerGloveStatus> gloves = m_Gloves.All;
            m_OnGloveStatusReceived.Invoke(gloves.ToArray());

            if (OpenSDKActions.PerformerGloveStatusReceived != null)
            {
                OpenSDKActions.PerformerGloveStatusReceived.Invoke(gloves);
            }
        }
        private void HandleSliderMessageMain()
        {
            List<HandSlider> allHandSliders = m_HandSliders.All;

            // Invoke Action indicating that the hand slider data has been serialized on the main thread and can be read from HandSliders
            m_OnHandSliderReceived.Invoke(allHandSliders.ToArray());

            if (OpenSDKActions.HandSliderMainReceived != null)
            {
                OpenSDKActions.HandSliderMainReceived.Invoke(allHandSliders);
            }
        }

        /**
         * Handle the bone rotation data on the main thread. Use this to process the bone rotation data if performance is not a concern.
         * Use it sparingly as it will slow down the OSC server if the message queue is not processed quickly enough.
         * 
         * This main thread handler is called very quickly so any kind of object instantiation done in here will
         * slow down the animation data processing
         * 
         * In some cases where GameObjects need to be created or destroyed, or a web API call is needing to be made in response to the
         * animation data this method is necessary to avoid threading issues.
         */
        private void HandleBoneMessageMain()
        {
            // Invoke Action indicating that the bone rotation data has been serialized on the main thread and can be read from lastBoneRotationData
            if (OpenSDKActions.BoneRotationMainReceived != null)
            {
                OpenSDKActions.BoneRotationMainReceived.Invoke();
            }

            m_OnBoneRotationReceived.Invoke(lastBoneRotationData);
        }

        private void HandleBoneMetacarpalMain()
        {
            // Invoke Action indicating that the bone rotation data has been serialized on the main thread and can be read from lastBoneRotationData
            if (OpenSDKActions.BoneMetacarpalMainReceived != null)
            {
                OpenSDKActions.BoneMetacarpalMainReceived.Invoke();
            }

            m_OnBoneMetacarpalReceived.Invoke(lastBoneRotationData);
        }

        private void HandleDongleMessageBackground(OscMessageValues values)
        {
            if (values.ElementCount > 3)
            {
                Debug.LogError("Incorrect format for /v1/device/dongle/status message");
                return;
            }

            string dongleId = values.ReadStringElement(0);
            string firmwareVersion = values.ReadStringElement(1);
            OpenSDKVersionStatus versionStatus = (OpenSDKVersionStatus)values.ReadIntElement(2);

            DeviceDongleStatus dongleStatus = new DeviceDongleStatus(dongleId, firmwareVersion, versionStatus);
            m_Dongles.Set(dongleId, dongleStatus);
        }

        private void HandlePerformerMessageBackground(OscMessageValues values)
        {
            if (values.ElementCount > 3)
            {
                Debug.LogError("Incorrect format for /v1/performer/all message");
                return;
            }

            int performerId = values.ReadIntElement(0);
            OpenSDKStaged isStaged = values.ReadIntElement(1) > 0 ? OpenSDKStaged.STAGED : OpenSDKStaged.UNSTAGED;
            string name = values.ReadStringElement(2);

            Performer performer = new Performer(performerId, name, isStaged);

            m_Performers.Set($"{performerId}_{name}", performer);
        }


        /**
         * Handle the glove status data on the background thread and store it in the m_Gloves dictionary for later us in the main thread.
         * In most cases this method should be called every second as this is the default rate which PerformerGloveStatus data is sent from Hand Engine.
         */
        private void HandleGloveMessageBackground(OscMessageValues values)
        {
            if (values.ElementCount > 13)
            {
                Debug.LogError("Incorrect format for /v1/performer/glove/status message");
                return;
            }

            int performerId = values.ReadIntElement(0);
            OpenSDKHandedness profileHandedness = (OpenSDKHandedness)values.ReadIntElement(1);
            string dongleId = values.ReadStringElement(2);
            string gloveId = values.ReadStringElement(3);

            OpenSDKHandedness gloveHandedness = (OpenSDKHandedness)values.ReadIntElement(4);
            bool isCalibrated = values.ReadIntElement(5) > 0;
            string gloveRevision = values.ReadStringElement(6);
            string productName = values.ReadStringElement(7);
            string firmwareVersion = values.ReadStringElement(8);

            OpenSDKVersionStatus versionStatus = (OpenSDKVersionStatus)values.ReadIntElement(9);
            float batteryLevel = values.ReadFloatElement(10);
            int rawSdCardPresent = values.ReadIntElement(11);
            // Parse the OpenSDKCardStatus enum from the raw integer value, setting it to UNKNOWN if the value is undefined
            OpenSDKCardStatus isSdCardPresent = rawSdCardPresent == OpenSDKAttributeParser.INT_UNKNOWN_VALUE ? OpenSDKCardStatus.UNKNOWN : (OpenSDKCardStatus)rawSdCardPresent;

            OpenSDKJamSyncedStatus isJamSynced = (OpenSDKJamSyncedStatus)values.ReadIntElement(12);


            PerformerGloveStatus gloveStatus = new PerformerGloveStatus(performerId, gloveId, dongleId, profileHandedness, gloveHandedness, isCalibrated, gloveRevision, productName, firmwareVersion, versionStatus, batteryLevel, isSdCardPresent, isJamSynced);

            m_Gloves.Set($"{performerId}_{gloveId}", gloveStatus);
        }

        void HandleSliderMessageBackground(OscMessageValues values)
        {
            if (values.ElementCount > 26)
            {
                Debug.LogError("Incorrect format for /v1/performer/glove/status message");
                return;
            }
            int timecode = values.ReadIntElement(0);
            int performerId = values.ReadIntElement(1);
            OpenSDKHandedness handedness = (OpenSDKHandedness)values.ReadIntElement(2);
            string gloveId = values.ReadStringElement(3);
            string gloveRevision = values.ReadStringElement(4);
            float thumbBend1 = values.ReadFloatElement(5);
            float thumbBend2 = values.ReadFloatElement(6);
            float thumbBend3 = OpenSDKAttributeParser.TryReadFloat(values, 7);
            float thumbSplay = values.ReadFloatElement(8);
            float indexBend1 = values.ReadFloatElement(9);
            float indexBend2 = values.ReadFloatElement(10);
            float indexBend3 = OpenSDKAttributeParser.TryReadFloat(values, 11);
            float middleBend1 = values.ReadFloatElement(12);
            float middleBend2 = values.ReadFloatElement(13);
            float middleBend3 = OpenSDKAttributeParser.TryReadFloat(values, 14);
            float ringBend1 = values.ReadFloatElement(15);
            float ringBend2 = values.ReadFloatElement(16);
            float ringBend3 = OpenSDKAttributeParser.TryReadFloat(values, 17);
            float pinkyBend1 = values.ReadFloatElement(18);
            float pinkyBend2 = values.ReadFloatElement(19);
            float pinkyBend3 = OpenSDKAttributeParser.TryReadFloat(values, 20);
            float globalSplay = values.ReadFloatElement(21);
            float indexSplay = OpenSDKAttributeParser.TryReadFloat(values, 22);
            float middleSplay = OpenSDKAttributeParser.TryReadFloat(values, 23);
            float ringSplay = OpenSDKAttributeParser.TryReadFloat(values, 24);
            float pinkySplay = OpenSDKAttributeParser.TryReadFloat(values, 25);

            m_HandSliders.Set($"{performerId}_{gloveId}", new HandSlider(performerId, handedness, timecode, gloveId, gloveRevision, thumbBend1, thumbBend2, thumbBend3, thumbSplay, indexBend1, indexBend2, indexBend3, middleBend1, middleBend2, middleBend3, ringBend1, ringBend2, ringBend3, pinkyBend1, pinkyBend2, pinkyBend3, globalSplay, indexSplay, middleSplay, ringSplay, pinkySplay));
        }

        /**
         * This method is called when an OSC message is received. It is used to extract the data from the OSC message and
         * pass it to the OpenSDKApplicatorAdvanced component for processing.
         * 
         * @param values The OSC message values
         * @param applicator The OpenSDKApplicatorAdvanced component to apply the bone data to
         * @param jointName The name of the joint to apply the bone data to
         * @param performerId The performer ID of the bone data
         */
        private void HandleBoneMessageBackground(OscMessageValues values, OpenSDKApplicatorAdvanced applicator, int performerId, OpenSDKHandedness handedness, OpenSDKFinger finger)
        {
            if (values.ElementCount > 13)
            {
                Debug.LogError($"Incorrect format for /v1/animation/{performerId}/{handedness}/{finger}/rotation message");
                return;
            }

            AnimationBoneRotationFrame data = OpenSDKBoneReceiver.HandleBoneMessage(values, applicator, performerId, handedness, finger, false);

            // Invoke Action indicating that the bone rotation data has been serialized on the background thread and can be read from this Action's parameters
            if (OpenSDKActions.BoneRotationBackgroundReceived != null)
            {
                OpenSDKActions.BoneRotationBackgroundReceived.Invoke(data.boneRotations, data.timecode, false);
            }
        }

        private void HandleBoneMetacarpalBackground(OscMessageValues values, OpenSDKApplicatorAdvanced applicator, int performerId, OpenSDKHandedness handedness, OpenSDKFinger finger)
        {
            if (values.ElementCount > 18)
            {
                Debug.LogError($"Incorrect format for /v1/animation/{performerId}/{handedness}/{finger}/rotationWithMetacarpals message");
                return;
            }

            AnimationBoneRotationFrame data = OpenSDKBoneReceiver.HandleBoneMessage(values, applicator, performerId, handedness, finger, true);

            // Invoke Action indicating that the bone rotation data has been serialized on the background thread and can be read from this Action's parameters
            if (OpenSDKActions.BoneRotationBackgroundReceived != null)
            {
                OpenSDKActions.BoneRotationBackgroundReceived.Invoke(data.boneRotations, data.timecode, true);
            }
        }
    }
}