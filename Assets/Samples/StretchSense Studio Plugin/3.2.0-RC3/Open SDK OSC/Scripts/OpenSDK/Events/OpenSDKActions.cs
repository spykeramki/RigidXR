using System;
using System.Collections.Generic;

namespace StretchSense
{
    public static class OpenSDKActions
    {
        /**
         * Event for when the animation data is received via OpenSDK on the background thread
         * 
         * @type BoneRotation: The bone rotation data received from OpenSDK
         * @type int: The timecode of the bone rotation data
         * @type bool: Whether the bone rotation data contains metacarpal bones
         */
        public static Action<List<AnimationBoneRotation>,int, bool> BoneRotationBackgroundReceived;

        public static Action<List<AnimationBoneRotation>, int, bool> BoneRotationMetacarpalReceived;

        /**
         * Event for when the bone rotation data is processed via OpenSDK on the main thread
         * The data is not included in this Action, as it is assumed that the data has already
         * been processed and stored in the `OpenSDKApplicatorAdvanced`'s `boneDataDictionary`.
         * If you need this data, you can access it directly from `OpenSDKApplicatorAdvanced`.
         */
        public static Action BoneRotationMainReceived;

        public static Action BoneMetacarpalMainReceived;

        /**
         * Event for when the animation data is received via OpenSDK on the background thread
         * 
         * @type List<PerformerGloveStatus>: All the glove status data received from OpenSDK
         * for staged performers only
         */
        public static Action<List<PerformerGloveStatus>> PerformerGloveStatusReceived;

        /**
         * Event for when the performer data is received via OpenSDK on the background thread
         * @type List<Performer>: All the currently staged performer data received from OpenSDK
         */
        public static Action<List<Performer>> PerformerReceived;

        /**
         * Event for when the hand slider data is received via OpenSDK on the background thread.
         * Use this when your app needs to perform background thread functionality based on the
         * current hand pose derived from the hand slider data. Is faster than accessing the data
         * on the main thread.
         * @type List<HandSlider>: The most recent hand slider data for all staged performers
         */
        public static Action<List<HandSlider>> HandSliderBackgroundReceived;

        /**
         * Event for when the hand slider data is received via OpenSDK on the main thread.
         * Use this if you intend on setting GameObjects to being active/inactive, instantiating
         * GameObjects in the scene or making web requests based on the hand slider data.
         * @type List<HandSlider>: The most recent hand slider data for all staged performers
         */
        public static Action<List<HandSlider>> HandSliderMainReceived;

        /**
         * Event for when the device dongle status data is received via OpenSDK on the background thread.
         * Use this when your app needs to perform background thread functionality based on the
         * current device dongle status. Is faster than accessing the data on the main thread.
         * @type List<DeviceDongleStatus>: The most recent device dongle status
         */
        public static Action<List<DeviceDongleStatus>> DeviceDongleStatusReceived;
    }
}