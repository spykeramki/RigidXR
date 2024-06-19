using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StretchSense
{
    public class DongleStatusSingleUpdater : MonoBehaviour
    {
        [Tooltip("The label to display the dongle status")]
        public TMP_Text dongleLabel;
        [Tooltip("The TextMeshPro sprite to update with the dongle connection status.")]
        public Image dongleIndicator;
        // Start is called before the first frame update
        void Start()
        {
            if (dongleLabel == null)
            {
                dongleLabel = GetComponentInChildren<TMP_Text>();
            }

            if (dongleIndicator == null)
            {
                dongleIndicator = GetComponentInChildren<Image>();
            }
        }

        public void UpdateUI(string dongleId, string firmwareVersion, OpenSDKVersionStatus versionStatus, Sprite connectionIndicatorSprite)
        {
            dongleLabel.text = $"{dongleId} v{firmwareVersion} - {versionStatus}";
            dongleIndicator.overrideSprite = connectionIndicatorSprite;
        }
    }
}