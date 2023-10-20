using squadup.Models;

namespace squadup.Repository
{
    public interface IGroupRepository
    {
        long CreateSquad(FormInputModel.Squad squad);

        string UpdateSquad(long squadId, string uniqueId);

        List<EventMemberAttendanceModel> GetEventMemberAttendance(long eventId);

        List<EventMemberAttendanceModel> UpdateEventMemberAttendance(FormInputModel.EventAttendance attendance);

        string AddSquadEvent(FormInputModel.SquadEvent squadEvent);

        void DeleteSquad(string squadId);

        bool DeleteSquadMember(long squadMemberId);

        SquadModel GetSingleSquad(string slugId);

        void GetAllSquads();

    }
}
