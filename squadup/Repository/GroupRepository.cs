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

        public long CreateSquad(FormInputModel.Squad squad)
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
                    conn.Execute(query, new { squadName = squad.squadName }, transaction);

                    // Retrieve the generated squad ID (assuming it's a serial column)
                    long squadId = conn.ExecuteScalar<long>("SELECT LASTVAL()", null, transaction);

                    //insert each member
                    foreach (var memberName in squad.squadMembers)
                    {
                        string memberInsertQuery = "INSERT INTO SquadMember (membername, squadid) VALUES (@memberName, @squadId)";
                        conn.Execute(memberInsertQuery, new { memberName, squadId }, transaction);
                    }

                    // Commit the transaction
                    transaction.Commit();
                    return squadId;
                }
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                Console.WriteLine(ex);
                return 0;
            }

        }

        public string UpdateSquad(long squadId, string uniqueId)
        {
            string query = "UPDATE Squad SET slug = @uniqueId WHERE squadId = @squadId RETURNING slug";

            try
            {
                using (var conn = _context.CreateConnection())
                {
                    string slug = conn.ExecuteScalar<string>(query, new { uniqueId, squadId });

                    return slug;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public void DeleteSquad(string groupId)
        {
            throw new NotImplementedException();
        }

        public void GetAllSquads()
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

        public SquadModel GetSingleSquad(string slugId)
        {
            string squadQuery = "SELECT * FROM squad WHERE slug = @slugId";
            string squadMemberQuery = "SELECT * FROM squadmember WHERE squadId = (SELECT squadid FROM squad WHERE slug = @slugId LIMIT 1)";

            SquadModel squadModel = null;

            try
            {
                using (var conn = _context.CreateConnection())
                {
                    var multi = conn.QueryMultiple(squadQuery + ";" + squadMemberQuery, new { slugId });

                    var squad = multi.Read<SquadModel>().SingleOrDefault();
                    var squadMembers = multi.Read<SquadMemberModel>().ToList();

                    squadModel  = new SquadModel()
                    {
                        squadId = squad.squadId,
                        squadName = squad.squadName,
                        slug = squad.slug,
                        createdAt = squad.createdAt,
                        members = squadMembers,
                    };

                    return squadModel;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return squadModel;
            }
        }
    }
}
