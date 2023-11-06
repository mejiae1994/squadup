namespace squadup.Models
{
    public class SquadEventModel
    {
        public long eventId { get; set; }

        public string eventName { get; set; }

        public DateTime eventDate { get; set; }

        public DateTimeOffset createdAt { get; set; }

        public long squadId { get; set; }

        public string? shareableLink { get; set; }

        public string eventDescription { get; set; }

        public int eventPrice { get; set; }

        public bool isSplitPrice { get; set; }

        public List<EventMemberAttendanceModel>? eventMemberAttendance { get; set; }

        public int attendingMembers
        {
            get
            {
                return eventMemberAttendance.Count(em => em.attendanceCode == AttendanceCode.Attending);
            }
            private set { }
        }

        public string memberFee
        {
            get
            {
                return eventPrice > 0 ? (isSplitPrice ? $"{string.Format("${0:0.00}", eventFee.ToString())} each" : string.Format("${0:0.00} each", eventPrice.ToString())) : "Free";
            }

            private set { }
        }

        private int eventFee
        {
            get
            {
                if (attendingMembers > 0)
                {
                    return eventPrice / attendingMembers;
                }
                else
                {
                    return eventPrice;
                }
            }
        }

    }
}
