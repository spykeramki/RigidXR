using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StretchSense
{
    public class PerformerNameplateUpdater : MonoBehaviour
    {
        public int performerId = 0;
        public string performerName = "Performer";
        public OpenSDKStaged isStaged = OpenSDKStaged.UNKNOWN;

        [SerializeField] public Color stagedColor = Color.green;
        [SerializeField] public Color unstagedColor = Color.red;
        [SerializeField] public Color unknownColor = Color.grey;

        [Tooltip("The TextMeshPro label to update with the performer's name.")]
        public TMP_Text label;

        [Tooltip("The TextMeshPro sprite to update with the performer's staged status.")]
        public Image isStagedIndicator;

        private void Start()
        {
            if(label == null)
            {
                label = GetComponentInChildren<TMP_Text>();
            }

            if(isStagedIndicator == null)
            {
                isStagedIndicator = GetComponentInChildren<Image>();
            }
        }

        private void OnEnable()
        {
            OpenSDKActions.PerformerReceived += UpdateNameplate;
        }

        private void OnDisable()
        {
            OpenSDKActions.PerformerReceived -= UpdateNameplate;
        }   

        public void UpdateNameplate(List<Performer> performers)
        {
            Performer performer = performers.FindLast(performer => performer.performerId == performerId);

            if(performer != null)
            {
                performerName = performer.performerName;
                isStaged = performer.isStaged;
            }
            else
            {
                isStaged = OpenSDKStaged.UNKNOWN;
            }

            UpdateVisuals(performerId, performerName, GetStagedColor(isStaged));
        }

        private Color GetStagedColor(OpenSDKStaged isStaged)
        {
            Color color = unknownColor;

            switch (isStaged)
            {
                case OpenSDKStaged.STAGED:
                    color = stagedColor;
                    break;
                case OpenSDKStaged.UNSTAGED:
                    color = unstagedColor;
                    break;
            }

            return color;
        }

        private void UpdateVisuals(int performerId, string performerName, Color color)
        {
            label.text = $"#{performerId} {performerName}";
            isStagedIndicator.color = color;
        }
    }
}