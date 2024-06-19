namespace StretchSense
{
    [System.Serializable]
    public class Performer
    {
        public int performerId;
        public string performerName;
        public OpenSDKStaged isStaged = OpenSDKStaged.UNKNOWN;
        public Performer(int performerId, string performerName, OpenSDKStaged isStaged)
        {
            this.performerId = performerId;
            this.performerName = performerName;
            this.isStaged = isStaged;
        }
    }
}