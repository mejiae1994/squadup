namespace squadup.Models
{
    public class SquadMemberModel
    {
        public long memberId { get; set; }

        public string memberName { get; set; }

        public DateTimeOffset createdAt { get; set; }

    }
}
