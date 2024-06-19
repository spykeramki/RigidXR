using OscCore;

namespace StretchSense
{
    public static class OpenSDKAttributeParser
    {
        /**
         * A constant value that represents the minimum signed integer value. This value is used to
         * determine if an integer value is undefined in the Open Glove SDK due to hardware missing
         * certain additional features like an SDK card or distal joint sensors.
         */
        public static int INT_UNKNOWN_VALUE = -2147483648;
        public static float TryReadFloat(OscMessageValues values, int index)
        {
            if (values.ElementCount > index)
            {
                // Check if the value is the minimum signed int value and store the
                // value as NaN to represent the slider value being not supported
                // for the current glove revision

                bool isNil = values.ReadIntElement(index) == INT_UNKNOWN_VALUE;
                if (isNil)
                {
                    return float.NaN;
                }
                else
                {
                    return values.ReadFloatElement(index);
                }
            }
            return float.NaN;
        }
    }
}