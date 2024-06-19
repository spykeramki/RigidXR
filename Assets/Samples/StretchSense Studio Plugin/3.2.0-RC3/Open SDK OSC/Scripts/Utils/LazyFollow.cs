using UnityEngine;

namespace StretchSense
{
    public class LazyFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float followSpeed = 1.0f;
        [SerializeField] private float smoothness = 0.1f;
        [SerializeField] private Vector3 followOffset;
        [SerializeField] private Vector3 rotationOffset;
        [SerializeField] private bool rotateTowardsMainCamera = false;

        private Vector3 targetPosition;
        private Camera mainCamera;

        private void Start()
        {
            targetPosition = transform.position;
            mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            if (target != null)
            {
                // Jump to target position with offset
                transform.position = target.position + followOffset;
                targetPosition = transform.position;

                // Apply rotation offset
                transform.rotation = target.rotation * Quaternion.Euler(rotationOffset);
            }
        }

        private void LateUpdate()
        {
            if (target != null)
            {
                // Calculate smoothed target position with offset
                targetPosition = Vector3.Lerp(targetPosition, target.position + followOffset, smoothness);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);

                // Rotate towards main camera (if enabled)
                if (rotateTowardsMainCamera && mainCamera != null)
                {
                    // Rotate towards main camera first
                    transform.LookAt(mainCamera.transform);

                    // Apply rotation offset afterwards
                    transform.rotation *= Quaternion.Euler(rotationOffset);
                }
            }
        }
    }
}