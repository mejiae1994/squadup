using squadup.Models;

namespace squadup.Repository
{
    public interface IGroupRepository
    {
        long CreateSquad(FormInputModel.Squad squad);

        string UpdateSquad(long squadId, string uniqueId);

        void DeleteSquad(string squadId);

        SquadModel GetSingleSquad(string squadId);

        void GetAllSquads();

    }
}
