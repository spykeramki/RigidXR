using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace StretchSense
{
    public class DongleStatusUpdater : MonoBehaviour
    {
        [Header("DEBUG VARS")]
        [Space(10)]

        [SerializeField]
        [Tooltip("The raw DongleGloveStatus information.")]
        public List<DeviceDongleStatus> dongleStatuses;

        [Tooltip("The DeviceDongleListUpdater script to update the dongle status.")]
        public DeviceDongleListUpdater deviceDongleListUpdater;


        private void OnEnable()
        {
            OpenSDKActions.DeviceDongleStatusReceived += UpdateDongleStatus;
        }

        private void OnDisable()
        {
            OpenSDKActions.DeviceDongleStatusReceived -= UpdateDongleStatus;
        }

        private void Start()
        {
            if(deviceDongleListUpdater == null)
            {
                deviceDongleListUpdater = GetComponentInChildren<DeviceDongleListUpdater>();
            }
        }

        private void CheckStatus(List<DeviceDongleStatus>dongleStatuses)
        {
            deviceDongleListUpdater.UpdateStatuses(dongleStatuses);
        }
        public void UpdateDongleStatus(List<DeviceDongleStatus> allDongles)
        {
            dongleStatuses = allDongles;

            CheckStatus(allDongles);
        }
    }
}