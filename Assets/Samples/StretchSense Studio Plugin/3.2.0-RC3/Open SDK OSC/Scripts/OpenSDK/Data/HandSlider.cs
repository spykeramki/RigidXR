namespace StretchSense
{
    [System.Serializable]
    public struct HandSlider
    {
        public int performerId;
        public OpenSDKHandedness handedness;
        public int timecode;
        public string gloveId;
        public string gloveRevision;

        public float thumbBend1;
        public float thumbBend2;
        public float thumbBend3;
        public float thumbSplay;
        public float indexBend1;
        public float indexBend2;
        public float indexBend3;
        public float middleBend1;
        public float middleBend2;
        public float middleBend3;
        public float ringBend1;
        public float ringBend2;
        public float ringBend3;
        public float pinkyBend1;
        public float pinkyBend2;
        public float pinkyBend3;
        public float globalSplay;
        public float indexSplay;
        public float middleSplay;
        public float ringSplay;
        public float pinkySplay;

        public HandSlider(int performerId, OpenSDKHandedness handedness, int timecode, string gloveId, string gloveRevision,
            float thumbBend1 = 0,
            float thumbBend2 = 0,
            float thumbBend3 = 0,
            float thumbSplay = 0,
            float indexBend1 = 0,
            float indexBend2 = 0,
            float indexBend3 = 0,
            float middleBend1 = 0,
            float middleBend2 = 0,
            float middleBend3 = 0,
            float ringBend1 = 0,
            float ringBend2 = 0,
            float ringBend3 = 0,
            float pinkyBend1 = 0,
            float pinkyBend2 = 0,
            float pinkyBend3 = 0,
            float globalSplay = 0,
            float indexSplay = 0,
            float middleSplay = 0,
            float ringSplay = 0,
            float pinkySplay = 0
        )
        {
            this.performerId = performerId;
            this.handedness = handedness;
            this.timecode = timecode;
            this.gloveId = gloveId;
            this.gloveRevision = gloveRevision;

            this.thumbBend1 = thumbBend1;
            this.thumbBend2 = thumbBend2;
            this.thumbBend3 = thumbBend3;
            this.thumbSplay = thumbSplay;
            this.indexBend1 = indexBend1;
            this.indexBend2 = indexBend2;
            this.indexBend3 = indexBend3;
            this.middleBend1 = middleBend1;
            this.middleBend2 = middleBend2;
            this.middleBend3 = middleBend3;
            this.ringBend1 = ringBend1;
            this.ringBend2 = ringBend2;
            this.ringBend3 = ringBend3;
            this.pinkyBend1 = pinkyBend1;
            this.pinkyBend2 = pinkyBend2;
            this.pinkyBend3 = pinkyBend3;
            this.globalSplay = globalSplay;
            this.indexSplay = indexSplay;
            this.middleSplay = middleSplay;
            this.ringSplay = ringSplay;
            this.pinkySplay = pinkySplay;
        }
    }
}