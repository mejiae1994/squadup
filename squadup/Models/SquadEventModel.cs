namespace squadup.Models
{
    public class SquadEventModel
    {
        public string eventId { get; set; }

        public string eventName { get; set; }

        public DateTime eventDate {get; set;}

        public DateTimeOffset createdAt { get; set; }

    }
}
