using squadup.Models;
using static squadup.Models.FormInputModel;

namespace squadup.Repository
{
    public interface IGroupRepository
    {
        long CreateSquad(FormInputModel.Squad squad);

        string UpdateSquad(long squadId, string uniqueId);

        List<EventMemberAttendanceModel> GetEventMemberAttendance(long eventId);

        List<SquadEventModel> UpdateEventMemberAttendance(List<EventAttendance> eventAttendance);

        string AddSquadEvent(FormInputModel.SquadEvent squadEvent);

        List<SquadEventModel> DeleteSquadEvent(long eventId);

        void DeleteSquad(string squadId);

        bool DeleteSquadMember(long squadMemberId);

        bool AddSquadMember(long squadId, string squadMember);

        SquadModel GetSingleSquad(string slugId);

        void GetAllSquads();

    }
}
