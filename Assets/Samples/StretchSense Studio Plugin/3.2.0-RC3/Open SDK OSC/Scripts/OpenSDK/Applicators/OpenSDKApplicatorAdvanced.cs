using UnityEngine;
using System.Collections.Generic;

namespace StretchSense
{
    public class OpenSDKApplicatorAdvanced : MonoBehaviour
    {
        [Tooltip("The target character to animate")]
        public GameObject targetCharacter;

        [Tooltip("The Hand Engine performerId to animate")]
        public int performerId = 0;

        [Tooltip("If true, the applicator will automatically bind to the joint names provided in the OpenSDKReceiver component. If false, you must manually add OscQuaternionMessageHandlers and set their callbacks to OpenSDKApplicatiorAdvanced.UpdateBoneData() - do this if you only need a subset of bones processed or need custom logic to animate your character.")]
        public bool autoSetup = true;

        [Tooltip("If true, the applicator will store and use animation/rotationWithMetacarpals data when animating the hang rig. Data from animation/rotation will be discarded and not applied to the hand rig. Required for OpenXR hand rigs.")]
        public bool useMetacarpals = false;

        // Lists for customization in the Inspector
        public List<Transform> characterJoints;
        public List<string> jointNameMapping = new List<string>(){
            "LeftThumbProximal",
            "LeftThumbIntermediate",
            "LeftThumbDistal",
            "LeftIndexProximal",
            "LeftIndexIntermediate",
            "LeftIndexDistal",
            "LeftMiddleProximal",
            "LeftMiddleIntermediate",
            "LeftMiddleDistal",
            "LeftRingProximal",
            "LeftRingIntermediate",
            "LeftRingDistal",
            "LeftLittleProximal",
            "LeftLittleIntermediate",
            "LeftLittleDistal",
            "RightThumbProximal",
            "RightThumbIntermediate",
            "RightThumbDistal",
            "RightIndexProximal",
            "RightIndexIntermediate",
            "RightIndexDistal",
            "RightMiddleProximal",
            "RightMiddleIntermediate",
            "RightMiddleDistal",
            "RightRingProximal",
            "RightRingIntermediate",
            "RightRingDistal",
            "RightLittleProximal",
            "RightLittleIntermediate",
            "RightLittleDistal"
        };

        // Dictionary mapping jointName to AnimationBone data
        public SerializableDictionary<string, AnimationBoneRotation> boneDataDictionary = new();

        private void LateUpdate()
        {
            // Apply all stored bone data
            boneDataDictionary.entries.ForEach(entry =>
            {
                ApplyAnimationBoneData(entry.Key,entry.Value);
            });
        }

        public void ApplyAnimationBoneData(string jointName, AnimationBoneRotation boneRotation)
        {
            // 1. Find the matching Transform
            Transform matchingJoint = null;

            // Use the mapping list if provided and valid
            if (jointNameMapping.Count == characterJoints.Count)
            {
                int index = jointNameMapping.IndexOf(jointName);
                if (index != -1)
                {
                    matchingJoint = characterJoints[index];
                }
            }
            // Default: Try to find the Transform by name as a fallback. Is extremely slow!
            else
            {
                Debug.LogWarning($"Joint name mapping list does not have correct number of items in {gameObject.transform.name}, fall back to finding joint by name (SLOW!).");
                matchingJoint = FindJointByName(jointName);
            }

            // 2. Apply the data if a Transform is found
            if (matchingJoint != null)
            {
                Quaternion rotation = new Quaternion((float)boneRotation.rotationQuatX, (float)boneRotation.rotationQuatY, (float)boneRotation.rotationQuatZ, (float)boneRotation.rotationQuatW);
                matchingJoint.transform.localRotation = rotation;
            }
            else
            {
                //Debug.LogWarning("Joint not found: " + jointName);
            }
        }

        public void UpdateAllBoneData(AnimationBoneRotationFrame boneRotationFrame, bool hasMetacarpalBone)
        {
            boneRotationFrame.boneRotations.ForEach(boneRotation =>
            {
                UpdateBoneData(boneRotation, hasMetacarpalBone);
            });
        }

        public void UpdateBoneData(AnimationBoneRotation boneRotation, bool hasMetacarpalBone)
        {
            if (boneRotation.performerId != performerId || (hasMetacarpalBone != useMetacarpals))
            {
                return;
            }

            // Update the dictionary
            boneDataDictionary.Set(boneRotation.jointName, boneRotation);
        }

        // Helper method to find a child joint by its name
        private Transform FindJointByName(string name)
        {
            if (targetCharacter == null)
            {
                return null;
            }

            return targetCharacter.transform.Find(name); // You might need more advanced search logic depending on hierarchy
        }
    }
}
