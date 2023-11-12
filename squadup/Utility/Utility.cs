namespace squadup.Utility
{
    public class DateUtility
    {
        public static DateTime convertDateToLocal<T>(T time)
        {
            if (time is DateTime dateTime)
            {
                return (DateTime)(object)dateTime.ToLocalTime().ToString();
            }
            else if (time is DateTimeOffset dateTimeOffset)
            {
                return (DateTime)(object)dateTimeOffset.ToLocalTime().ToString();
            }
            else
            {
                throw new ArgumentException("Unsupported type for conversion");
            }
        }
    }
}
