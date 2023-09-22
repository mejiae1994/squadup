using squadup.Models;

namespace squadup.Repository
{
    public interface IGroupRepository
    {
        void CreateGroup(FormInputModel.Squad group);

        void DeleteGroup(string groupId);

        void GetSingleGroup(string groupId);

        void GetAllGroups();

    }
}
