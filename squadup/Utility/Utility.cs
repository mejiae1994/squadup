namespace squadup.Utility
{
    public class DateUtility
    {
        public static T convertDateToLocal<T>(T time)
        {
            if (time is DateTime dateTime)
            {
                return (T)(object)dateTime.ToLocalTime().ToString();
            }
            else if (time is DateTimeOffset dateTimeOffset)
            {
                return (T)(object)dateTimeOffset.ToLocalTime();
            }
            else
            {
                throw new ArgumentException("Unsupported type for conversion");
            }
        }
    }
}
