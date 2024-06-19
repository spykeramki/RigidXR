using OscCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StretchSense
{
    public static class OpenSDKBoneReceiver
    {
        public static AnimationBoneRotationFrame HandleBoneMessage(OscMessageValues values, OpenSDKApplicatorAdvanced applicator, int performerId, OpenSDKHandedness handedness, OpenSDKFinger finger, bool hasMetacarpalBone)
        {
            string logValue = "Message: ";
            values.ForEachElement((index, typeTag) =>
            {
                switch (typeTag)
                {
                    case TypeTag.Float32:
                        logValue += $" Float value: {values.ReadFloatElement(index)}";
                        break;
                    case TypeTag.Int32:
                        logValue += $" Int value: {values.ReadIntElement(index)}";
                        break;
                    case TypeTag.String:
                        logValue += $" String value: {values.ReadStringElement(index)}";
                        break;
                    case TypeTag.Nil:
                        logValue += $" Nil value: {values.ReadNilOrInfinitumElement(index)}";
                        break;
                    default:
                        Debug.Log("Read unknown type");
                        break;
                }
            });

            int timecode = values.ReadIntElement(0);
            List<string> jointNames = applicator.jointNameMapping;

            // Optional list of joint names to read from the incoming message - shouldn't be required once all joints are confirmed to work
            List<string> jointNamesThumbL = new List<string> { "LeftThumbProximal", "LeftThumbIntermediate", "LeftThumbDistal" };
            List<string> jointNamesThumbR = new List<string> { "RightThumbProximal", "RightThumbIntermediate", "RightThumbDistal" };
            List<string> jointNamesIndexL = new List<string> { "LeftIndexProximal", "LeftIndexIntermediate", "LeftIndexDistal" };
            List<string> jointNamesIndexR = new List<string> { "RightIndexProximal", "RightIndexIntermediate", "RightIndexDistal" };
            List<string> jointNamesMiddleL = new List<string> { "LeftMiddleProximal", "LeftMiddleIntermediate", "LeftMiddleDistal" };
            List<string> jointNamesMiddleR = new List<string> { "RightMiddleProximal", "RightMiddleIntermediate", "RightMiddleDistal" };
            List<string> jointNamesRingL = new List<string> { "LeftRingProximal", "LeftRingIntermediate", "LeftRingDistal" };
            List<string> jointNamesRingR = new List<string> { "RightRingProximal", "RightRingIntermediate", "RightRingDistal" };
            List<string> jointNamesLittleL = new List<string> { "LeftLittleProximal", "LeftLittleIntermediate", "LeftLittleDistal" };
            List<string> jointNamesLittleR = new List<string> { "RightLittleProximal", "RightLittleIntermediate", "RightLittleDistal" };
            List<string> allowedJointNames = new();

            if (hasMetacarpalBone)
            {
                jointNamesThumbL.Insert(0, "LeftThumbMetacarpal");
                jointNamesThumbR.Insert(0, "RightThumbMetacarpal");
                jointNamesIndexL.Insert(0, "LeftIndexMetacarpal");
                jointNamesIndexR.Insert(0, "RightIndexMetacarpal");
                jointNamesMiddleL.Insert(0, "LeftMiddleMetacarpal");
                jointNamesMiddleR.Insert(0, "RightMiddleMetacarpal");
                jointNamesRingL.Insert(0, "LeftRingMetacarpal");
                jointNamesRingR.Insert(0, "RightRingMetacarpal");
                jointNamesLittleL.Insert(0, "LeftLittleMetacarpal");
                jointNamesLittleR.Insert(0, "RightLittleMetacarpal");
            }

                if (handedness == OpenSDKHandedness.LEFT)
            {
                switch (finger)
                {
                    case OpenSDKFinger.Thumb:
                        allowedJointNames = jointNamesThumbL;
                        break;
                    case OpenSDKFinger.Index:
                        allowedJointNames = jointNamesIndexL;
                        break;
                    case OpenSDKFinger.Middle:
                        allowedJointNames = jointNamesMiddleL;
                        break;
                    case OpenSDKFinger.Ring:
                        allowedJointNames = jointNamesRingL;
                        break;
                    case OpenSDKFinger.Little:
                        allowedJointNames = jointNamesLittleL;
                        break;
                }
            }
            else if (handedness == OpenSDKHandedness.RIGHT)
            {
                switch (finger)
                {
                    case OpenSDKFinger.Thumb:
                        allowedJointNames = jointNamesThumbR;
                        break;
                    case OpenSDKFinger.Index:
                        allowedJointNames = jointNamesIndexR;
                        break;
                    case OpenSDKFinger.Middle:
                        allowedJointNames = jointNamesMiddleR;
                        break;
                    case OpenSDKFinger.Ring:
                        allowedJointNames = jointNamesRingR;
                        break;
                    case OpenSDKFinger.Little:
                        allowedJointNames = jointNamesLittleR;
                        break;
                }
            }

            List<AnimationBoneRotation> boneRotations = GetBoneRotations(applicator, allowedJointNames, values, performerId, handedness, finger, hasMetacarpalBone);
            return new AnimationBoneRotationFrame(boneRotations, timecode);
        }

        private static List<AnimationBoneRotation> GetBoneRotations(OpenSDKApplicatorAdvanced applicator, List<string> allowedJointNames, OscMessageValues values, int performerId, OpenSDKHandedness handedness, OpenSDKFinger finger, bool hasMetacarpalBone)
        {
            List<AnimationBoneRotation> boneRotations = new();
            int offset = 0;

            for (int i = 0; i < allowedJointNames.Count; i++)
            {
                string jointName;

                float rotationQuatX;
                float rotationQuatY;
                float rotationQuatZ;
                float rotationQuatW;

                jointName = allowedJointNames[i];

                // Skip joints not on allow list
                if (allowedJointNames.Contains(jointName))
                {
                    rotationQuatX = values.ReadFloatElement(offset + 1);
                    rotationQuatY = values.ReadFloatElement(offset + 2);
                    rotationQuatZ = values.ReadFloatElement(offset + 3);
                    rotationQuatW = values.ReadFloatElement(offset + 4);

                    AnimationBoneRotation boneRotation = new AnimationBoneRotation(performerId, handedness, finger, jointName, rotationQuatX, rotationQuatY, rotationQuatZ, rotationQuatW);
                    applicator.UpdateBoneData(boneRotation, hasMetacarpalBone);
                    boneRotations.Add(boneRotation);
                }

                offset += 4;
            }

            return boneRotations;
        }
    }
}