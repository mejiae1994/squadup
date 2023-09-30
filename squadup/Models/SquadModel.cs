namespace squadup.Models
{
    public class SquadModel
    {
        public long squadId { get; set; }

        public string squadName { get; set; }

        public string slug { get; set; }

        public DateTimeOffset createdAt { get; set; }

        public List<SquadMemberModel>? members { get; set; }

        public List<SquadEventModel>? events { get; set; }

    }
}
