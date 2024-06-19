namespace StretchSense
{
    [System.Serializable]
    public struct AnimationBoneRotation
    {
        public int performerId;
        public string jointName;
        public OpenSDKHandedness handedness;
        public string fingerName;
        public float rotationQuatX;
        public float rotationQuatY;
        public float rotationQuatZ;
        public float rotationQuatW;
        public AnimationBoneRotation(int performerId, OpenSDKHandedness handedness, OpenSDKFinger finger, string jointName, float rotationQuatX, float rotationQuatY, float rotationQuatZ, float rotationQuatW)
        {
            this.performerId = performerId;
            this.handedness = handedness;
            this.fingerName = finger.ToString();
            this.jointName = jointName;
            this.rotationQuatX = rotationQuatX;
            this.rotationQuatY = rotationQuatY;
            this.rotationQuatZ = rotationQuatZ;
            this.rotationQuatW = rotationQuatW;
        }
    }
}