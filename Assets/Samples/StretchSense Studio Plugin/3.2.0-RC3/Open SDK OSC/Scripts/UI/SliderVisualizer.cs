using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StretchSense
{
    public class SliderVisualizer : MonoBehaviour
    {
        public int performerId = 0;
        public OpenSDKHandedness handedness = OpenSDKHandedness.UNKNOWN;
        public OpenSDKSlider sliderType = OpenSDKSlider.UNKNOWN;

        [SerializeField]
        [Tooltip("The last value of the slider")]
        public float sliderValue => GetSliderValue(sliderType);

        [Tooltip("The label update rate (in seconds)")]
        public float animInterval = 0.5f;

        [SerializeField]
        [Tooltip("The HandSlider for the current performerId and handedness to use as the data source for label updates")]
        public HandSlider handSlider;

        [Tooltip("The TextMeshPro label to update with the slider value")]
        public TMP_Text currentValueLabel;

        [Tooltip("The TextMeshPro label to update with the slider name")]
        public TMP_Text sliderNameLabel;

        [Tooltip("The background image for the slider")]
        public Image sliderBG;

        [Tooltip("The background color for the slider")]
        public Color sliderBGColor = Color.black;

        public Canvas canvas;

        private void Start()
        {
            TMP_Text[] labels = GetComponentsInChildren<TMP_Text>();

            if (currentValueLabel == null && labels.Length > 0)
            {
                currentValueLabel = labels[0];
            }

            if (sliderNameLabel == null && labels.Length > 1)
            {
                sliderNameLabel = labels[1];
            }

            

            if (canvas == null)
            {
                canvas = GetComponentInChildren<Canvas>();
            }

            if (sliderBG == null)
            {
                sliderBG = canvas.GetComponent<Image>();
            }

            StartCoroutine("UpdateVisuals");

            sliderNameLabel.text = sliderType.ToString();
            
        }

        private void OnDestroy()
        {
            StopCoroutine("UpdateVisuals");
        }

        IEnumerator UpdateVisuals()
        {
            while (true)
            {
                UpdateSliderLabel(sliderValue);

                yield return new WaitForSeconds(animInterval);
            }
        }

        private void OnEnable()
        {
            OpenSDKActions.HandSliderMainReceived += UpdateAllSliders;
        }

        private void OnDisable()
        {
            OpenSDKActions.HandSliderMainReceived -= UpdateAllSliders;
        }

        private void UpdateAllSliders(List<HandSlider> handSliders)
        {
            // Store the relevent current HandSlider data struct for retrieval during the coroutine
            handSlider = handSliders.FindLast(slider => slider.performerId == performerId && slider.handedness == handedness);
        }

        public void Hide()
        {
            canvas.gameObject.SetActive(false);
        }

        public void Show()
        {
            canvas.gameObject.SetActive(true);
        }

        private float GetSliderValue(OpenSDKSlider sliderType)
        {
            switch (sliderType)
            {
                case OpenSDKSlider.THUMBBEND1:
                    return handSlider.thumbBend1;
                case OpenSDKSlider.THUMBBEND2:
                    return handSlider.thumbBend2;
                case OpenSDKSlider.THUMBSPLAY:
                    return handSlider.thumbSplay;
                case OpenSDKSlider.INDEXBEND1:
                    return handSlider.indexBend1;
                case OpenSDKSlider.INDEXBEND2:
                    return handSlider.indexBend2;
                case OpenSDKSlider.MIDDLEBEND1:
                    return handSlider.middleBend1;
                case OpenSDKSlider.MIDDLEBEND2:
                    return handSlider.middleBend2;
                case OpenSDKSlider.RINGBEND1:
                    return handSlider.ringBend1;
                case OpenSDKSlider.RINGBEND2:
                    return handSlider.ringBend2;
                case OpenSDKSlider.PINKYBEND1:
                    return handSlider.pinkyBend1;
                case OpenSDKSlider.PINKYBEND2:
                    return handSlider.pinkyBend2;
                case OpenSDKSlider.GLOBALSPLAY:
                    return handSlider.globalSplay;
                case OpenSDKSlider.THUMBBEND3:
                    return handSlider.thumbBend3;
                case OpenSDKSlider.INDEXBEND3:
                    return handSlider.indexBend3;
                case OpenSDKSlider.MIDDLEBEND3:
                    return handSlider.middleBend3;
                case OpenSDKSlider.RINGBEND3:
                    return handSlider.ringBend3;
                case OpenSDKSlider.PINKYBEND3:
                    return handSlider.pinkyBend3;
                case OpenSDKSlider.INDEXSPLAY:
                    return handSlider.indexSplay;
                case OpenSDKSlider.MIDDLESPLAY:
                    return handSlider.middleSplay;
                case OpenSDKSlider.RINGSPLAY:
                    return handSlider.ringSplay;
                case OpenSDKSlider.PINKYSPLAY:
                    return handSlider.pinkySplay;
                case OpenSDKSlider.UNKNOWN:
                default:
                    return 0f;
            }
        }

        private void UpdateSliderLabel(float sliderValue)
        {
            if(currentValueLabel == null)
            {
                Debug.LogWarning($"No label found for slider value update in {gameObject.name}");
            }
            string text = (sliderValue * 100f).ToString("F0");

            if (sliderValue >= 0.01)
            {
                currentValueLabel.text = $"<color=#FFFFFF>{text}</color>";
            }
            else
            {
                currentValueLabel.text = $"<color=#000000>{text}</color>";
            }

            sliderBG.color = sliderBGColor;

        }
    }
}