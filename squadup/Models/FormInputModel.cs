﻿namespace squadup.Models
{
    public class FormInputModel
    {
        public class Squad
        {
            public string squadName { get; set; }

            public string? unparsedMembers { get; set; }

            public string[]? squadMembers { get; set; }
        }

        public class SquadMember
        {
            public string memberName { get; set; }

        }

        public class SquadEvent
        {
            public string eventName { get; set; }

            public DateTime? eventDate { get; set; }
        }
    }
}