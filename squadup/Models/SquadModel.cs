namespace squadup.Models
{
    public class SquadModel
    {
        public string squadId { get; set; }

        public string squadName { get; set; }

        public DateTimeOffset createdAt { get; set; }

        public List<SquadMemberModel>? members { get; set;}

        public List<SquadEventModel>? events { get; set; }

    }
}
