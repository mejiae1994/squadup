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

    }
}
