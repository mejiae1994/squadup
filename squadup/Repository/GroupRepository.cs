using Dapper;
using squadup.Models;
using squadup.Utility;

namespace squadup.Repository
{
    public class GroupRepository : IGroupRepository
    {

        public readonly DatabaseContext _context;

        public GroupRepository(DatabaseContext context)
        {
            _context = context;
        }

        public void CreateGroup(SquadModel group)
        {
            throw new NotImplementedException();
        }

        public void DeleteGroup(string groupId)
        {
            throw new NotImplementedException();
        }

        public void GetAllGroups()
        {
            string query = "Select * from squad";

            try
            {
                using (var conn = _context.CreateConnection())
                {
                    IEnumerable<SquadModel> results = conn.Query<SquadModel>(query);

                    foreach (SquadModel group in results)
                    {
                        var timestamp = DateUtility.convertDateToLocal(group.createdAt);

                        Console.WriteLine($"results: {group.squadName} {group.squadId} {timestamp}");
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("done retrieving all groups");
        }

        public void GetSingleGroup(string groupId)
        {
            if(string.IsNullOrEmpty(groupId))
            {
                Console.WriteLine("missing groupId");
            }
            else
            {
                Console.WriteLine($"getting single group for {groupId}");
            }
        }
    }
}
