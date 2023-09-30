namespace squadup.Models
{

    public enum AttendanceCode
    {
        NotAttending = 1,
        Maybe = 2,
        Attending = 3   
    }

    public class EventMemberAttendanceModel
    {
        public long attendanceId { get; set; }

        public long eventId { get; set; }

        public string eventName { get; set; }

        public long memberId { get; set; }

        public string memberName { get; set; }

        public AttendanceCode attendanceCode { get; set; }
    }
}