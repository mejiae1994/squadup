namespace squadup.Models
{
    public class FormInputModel
    {
        public class Squad
        {
            public string squadName { get; set; }

            public string? unparsedMembers { get; set; }

            public string[]? squadMembers { get; set; }

            public string? slug { get; set; }
        }

        public class SquadMember
        {
            public string memberName { get; set; }

        }

        public class SquadEvent
        {
            public string eventName { get; set; }

            public long squadId { get; set; }

            public DateTime eventDate { get; set; }

            public string eventDescription { get; set; }

            public int eventPrice { get; set; }
        }

        public class EventAttendance
        {
            public long memberId { get; set; }

            public AttendanceCode attendanceCode { get; set; }

            public long eventId { get; set; }
        }
    }
}
