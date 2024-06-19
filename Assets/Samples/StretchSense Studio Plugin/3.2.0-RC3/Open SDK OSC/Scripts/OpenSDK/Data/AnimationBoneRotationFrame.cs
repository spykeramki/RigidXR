using System.Collections.Generic;

namespace StretchSense
{
    public struct AnimationBoneRotationFrame
    {
        public List<AnimationBoneRotation> boneRotations;
        public int timecode;
        public AnimationBoneRotationFrame(List<AnimationBoneRotation> boneRotations, int timecode)
        {
            this.boneRotations = boneRotations;
            this.timecode = timecode;
        }
    }
}