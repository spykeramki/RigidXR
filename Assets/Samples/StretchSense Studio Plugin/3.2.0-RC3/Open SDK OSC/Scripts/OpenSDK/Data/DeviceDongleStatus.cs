namespace StretchSense
{
    [System.Serializable]
    public struct DeviceDongleStatus
    {
        public string dongleId;
        public string firmwareVersion;
        public OpenSDKVersionStatus versionStatus;
        public DeviceDongleStatus(string dongleId, string firmwareVersion, OpenSDKVersionStatus versionStatus)
        {
            this.dongleId = dongleId;
            this.firmwareVersion = firmwareVersion;
            this.versionStatus = versionStatus; 
        }
    }
}