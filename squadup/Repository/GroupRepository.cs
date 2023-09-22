using System.Data;
using System.Transactions;
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

        public void CreateGroup(FormInputModel.Squad group)
        {
            string query = $"INSERT INTO Squad (squadName) VALUES (@squadName)";

            IDbTransaction transaction = null;

            try
            {
                using (var conn = _context.CreateConnection())
                {
                    //open the connection and begin a transaction
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    //execute first query
                    conn.Execute(query, new { squadName = group.squadName }, transaction);

                    // Retrieve the generated squad ID (assuming it's a serial column)
                    long squadId = conn.ExecuteScalar<long>("SELECT LASTVAL()", null, transaction);

                    //insert each member
                    foreach (var memberName in group.squadMembers)
                    {
                        string memberInsertQuery = "INSERT INTO SquadMember (membername, squadid) VALUES (@memberName, @squadId)";
                        conn.Execute(memberInsertQuery, new { memberName, squadId }, transaction);
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                Console.WriteLine(ex);
            }
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
            if (string.IsNullOrEmpty(groupId))
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
