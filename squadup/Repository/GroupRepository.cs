using System.Data;
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

        public string UpdateSquad(long squadId, string slugId)
        {
            string query = "UPDATE Squad SET slug = @slugId WHERE squadId = @squadId RETURNING slug";

            try
            {
                using (var conn = _context.CreateConnection())
                {
                    string slug = conn.ExecuteScalar<string>(query, new { slugId, squadId });

                    return slug;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public List<EventMemberAttendanceModel> GetEventMemberAttendance(long eventId) 
        {
            //we want a list that have member, attendancestatus for the event that is passed in
            string query = "SELECT * FROM eventmemberattendance WHERE eventId = @eventId";
            string eventQuery = "SELECT e.attendanceId, e.eventId, ev.eventName, e.memberId, s.memberName, e.attendanceCode FROM eventmemberattendance e JOIN squadmember s on e.memberId = s.memberId JOIN squadevent ev on e.eventId = ev.eventId WHERE e.eventId = @eventId";

            List<EventMemberAttendanceModel> eventMemberAttendance = null;

            try
            {
                using (var conn = _context.CreateConnection())
                {
                    eventMemberAttendance = conn.Query<EventMemberAttendanceModel>(eventQuery, new { eventId }).ToList();
                    return eventMemberAttendance;
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public string AddSquadEvent(FormInputModel.SquadEvent squadEvent)
        {
            string eventInsertQuery = "INSERT INTO squadevent (eventname, eventdate, squadId) VALUES (@eventName, @eventDate, @squadId)";

            string squadQuery = "SELECT * FROM squad WHERE squadId = @squadId";
            string squadMemberQuery = "SELECT * FROM squadmember WHERE squadId = @squadId";


            IDbTransaction transaction = null;

            try
            {
                using (var conn = _context.CreateConnection())
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    //execute first query
                    conn.Execute(eventInsertQuery, new { squadEvent.eventName, squadEvent.eventDate, squadEvent.squadId }, transaction);

                    // Retrieve the generated eventId (assuming it's a serial column)
                    long eventId = conn.ExecuteScalar<long>("SELECT LASTVAL()", null, transaction);

                    //now I have eventId, squadId, I need to retrieve all members from squadmember
                    var multi = conn.QueryMultiple(squadQuery + ';' + squadMemberQuery, new { squadEvent.squadId}, transaction);

                    string slug = multi.Read<SquadModel>().Select(x => x.slug).SingleOrDefault();
                    var squadMembers = multi.Read<SquadMemberModel>().ToList();

                    int attendanceCode = (int)AttendanceCode.NotAttending;

                    //insert each member
                    //need to figure out why column says it does not exist

                    string memberInsertQuery = "INSERT INTO public.eventmemberattendance (eventId, memberId, attendanceCode) VALUES (@eventId, @memberId, @attendancecode)";

                    foreach (var member in squadMembers)
                    {
                        //note to self, anonymous object properties have to match sql column names
                        conn.Execute(memberInsertQuery, new { eventId, member.memberId, attendanceCode }, transaction);
                        
                    }

                    // Commit the transaction
                    transaction.Commit();
                    return slug;
                }
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
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
            string eventQuery = "SELECT * FROM squadevent WHERE squadId = (SELECT squadid FROM squad WHERE slug = @slugId LIMIT 1)";

            SquadModel squadModel = null;

            try
            {
                using (var conn = _context.CreateConnection())
                {
                    var multi = conn.QueryMultiple(squadQuery + ";" + squadMemberQuery + ";" + eventQuery, new { slugId });

                    var squad = multi.Read<SquadModel>().SingleOrDefault();
                    var squadMembers = multi.Read<SquadMemberModel>().ToList();
                    var squadEvents = multi.Read<SquadEventModel>().ToList();

                    squadModel = new SquadModel()
                    {
                        squadId = squad.squadId,
                        squadName = squad.squadName,
                        slug = squad.slug,
                        createdAt = squad.createdAt,
                        members = squadMembers,
                        events = squadEvents,
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
