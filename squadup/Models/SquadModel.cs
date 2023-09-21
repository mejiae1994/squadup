namespace squadup.Models
{
    public class SquadModel
    {
        public string squadId { get; set; }

        public string squadName { get; set; }

        public DateTimeOffset createdAt { get; set; }

        public string[] members { get; set;}

        public List<SquadEventModel>? events { get; set; }

        public string unparsedMembers { get; set; }
        
    }
}
