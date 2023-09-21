namespace squadup.Models
{
    public class SquadEventModel
    {
        public string squadEventId { get; set; }

        public string squadEventName { get; set; }

        public DateTime? eventDate {get; set;}

        public DateTimeOffset createdAt { get; set; }

    }
}
