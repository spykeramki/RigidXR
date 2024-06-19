using System;
using UnityEngine.Events;

namespace StretchSense
{
    [Serializable] public class BoneRotationUnityEvent : UnityEvent<AnimationBoneRotation> { }
    [Serializable] public class BoneFrameUnityEvent : UnityEvent<AnimationBoneRotationFrame> { }
    [Serializable] public class HandSliderUnityEvent : UnityEvent<HandSlider> { }
}