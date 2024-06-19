namespace StretchSense
{
    [System.Serializable]
    public struct PerformerGloveStatus
    {
        public int performerId;
        public OpenSDKHandedness profileHandedness;
        public string dongleId;
        public string gloveId;
        public OpenSDKHandedness gloveHandedness;
        public bool isCalibrated;
        public string gloveRevision;
        public string productName;
        public string firmwareVersion;
        public OpenSDKVersionStatus versionStatus;
        public float batteryLevel;
        public OpenSDKCardStatus isSdCardPresent;
        public OpenSDKJamSyncedStatus isJamSynced;

        public PerformerGloveStatus(int performerId, string gloveId, string dongleId, OpenSDKHandedness profileHandedness, OpenSDKHandedness gloveHandedness, bool isCalibrated, string gloveRevision, string productName, string firmwareVersion, OpenSDKVersionStatus versionStatus, float batteryLevel, OpenSDKCardStatus isSdCardPresent, OpenSDKJamSyncedStatus isJamSynced)
        {
            this.performerId = performerId;
            this.gloveId = gloveId;
            this.dongleId = dongleId;
            this.profileHandedness = profileHandedness;
            this.gloveHandedness = gloveHandedness;
            this.isCalibrated = isCalibrated;
            this.gloveRevision = gloveRevision;
            this.productName = productName;
            this.firmwareVersion = firmwareVersion;
            this.versionStatus = versionStatus;
            this.batteryLevel = batteryLevel;
            this.isSdCardPresent = isSdCardPresent;
            this.isJamSynced = isJamSynced;
        }
    }
}