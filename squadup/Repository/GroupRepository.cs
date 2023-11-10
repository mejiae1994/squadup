using Dapper;
using squadup.Models;
using squadup.Utility;
using System.Data;
using static squadup.Models.FormInputModel;

namespace squadup.Repository
{
    public class GroupRepository : IGroupRepository
    {

        public readonly DatabaseContext _context;
        public readonly GoogleService _googleService;

        public GroupRepository(DatabaseContext context)
        {
            _context = context;
            _googleService = new GoogleService();

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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public string AddSquadEvent(FormInputModel.SquadEvent squadEvent)
        {
            string eventInsertQuery;
            if (string.IsNullOrEmpty(squadEvent.eventDescription))
            {
                eventInsertQuery = "INSERT INTO squadevent (eventname, eventdate, squadId, eventPrice, isSplitPrice) VALUES (@eventName, @eventDate, @squadId, @eventPrice, @isSplitPrice)";
            }
            else
            {
                eventInsertQuery = "INSERT INTO squadevent (eventname, eventdate, squadId, eventDescription, eventPrice, isSplitPrice) VALUES (@eventName, @eventDate, @squadId, @eventDescription, @eventPrice, @isSplitPrice)";
            }

            string squadQuery = "SELECT * FROM squad WHERE squadId = @squadId";
            string squadMemberQuery = "SELECT * FROM squadmember WHERE squadId = @squadId";
            string updateSquadEventQuery = "UPDATE squadevent SET shareableLink = @shareableLink WHERE eventId = @eventId";

            IDbTransaction transaction = null;

            try
            {
                using (var conn = _context.CreateConnection())
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    //execute first query
                    conn.Execute(eventInsertQuery, new { squadEvent.eventName, squadEvent.eventDate, squadEvent.squadId, squadEvent.eventDescription, squadEvent.eventPrice, squadEvent.isSplitPrice }, transaction);

                    // Retrieve the generated eventId (assuming it's a serial column)
                    long eventId = conn.ExecuteScalar<long>("SELECT LASTVAL()", null, transaction);

                    string shareableLink = null;
                    if (eventId > 0)
                    {
                        shareableLink = _googleService.insertCalendarEvent(squadEvent.eventName, squadEvent.eventDate, squadEvent.eventDescription);
                    }

                    //update squadEvent with shareableLink if calendarEvent was successfully added.
                    if (!string.IsNullOrEmpty(shareableLink))
                    {
                        conn.Query(updateSquadEventQuery, new { shareableLink, eventId }, transaction);
                    }

                    //now I have eventId, squadId, I need to retrieve all members from squadmember
                    var multi = conn.QueryMultiple(squadQuery + ';' + squadMemberQuery, new { squadEvent.squadId }, transaction);

                    string slug = multi.Read<SquadModel>().Select(x => x.slug).SingleOrDefault();
                    var squadMembers = multi.Read<SquadMemberModel>().ToList();

                    int attendanceCode = (int)AttendanceCode.NotAttending;
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

        public class SlugAndShareableLink
        {
            public string Slug { get; set; }

            public string ShareableLink { get; set; }

            public long squadId { get; set; }
        }

        public List<SquadEventModel> DeleteSquadEvent(long eventId)
        {
            string getSlugLinkQuery = "SELECT s.slug, e.shareableLink, s.squadId FROM squad s JOIN squadEvent e ON s.squadId = e.squadId WHERE e.eventId = @eventId";
            string deleteEventQuery = "DELETE FROM squadEvent WHERE eventId = @eventId";

            string squadEventQuery = "SELECT * FROM squadevent WHERE squadId = @squadId";

            string eventMemberAttendanceQuery = "SELECT e.attendanceId, e.eventId, ev.eventName, e.memberId, s.memberName, e.attendanceCode FROM eventmemberattendance e JOIN squadmember s on e.memberId = s.memberId JOIN squadevent ev on e.eventId = ev.eventId WHERE e.eventId = @eventId";

            List<EventMemberAttendanceModel> eventMemberAttendance = null;

            IDbTransaction transaction = null;

            try
            {
                using (var conn = _context.CreateConnection())
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    SlugAndShareableLink slugLink = conn.Query<SlugAndShareableLink>(getSlugLinkQuery, new { eventId }, transaction).SingleOrDefault();

                    //execute first query
                    var result = conn.Execute(deleteEventQuery, new { eventId }, transaction);

                    //grab all the events that are tried to the squad we are currently looking at
                    List<SquadEventModel> squadEvents = conn.Query<SquadEventModel>(squadEventQuery, new { slugLink.squadId }, transaction).ToList();

                    //embed the list of eventmemberattendance per squadevent in the squadevent list
                    foreach (SquadEventModel sEvent in squadEvents)
                    {
                        eventMemberAttendance = conn.Query<EventMemberAttendanceModel>(eventMemberAttendanceQuery, new { sEvent.eventId }, transaction).ToList();
                        sEvent.eventMemberAttendance = eventMemberAttendance;
                    }

                    bool deleted = false;
                    if (!string.IsNullOrEmpty(slugLink.ShareableLink))
                    {
                        deleted = _googleService.deleteCalendarEvent(slugLink.ShareableLink);
                    }

                    if (deleted)
                    {
                        Console.WriteLine("google calendar event deleted");
                    }
                    else
                    {
                        Console.WriteLine("google calendar event not found");
                    }
                    // Commit the transaction
                    transaction.Commit();
                    return squadEvents;
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

            string eventMemberAttendanceQuery = "SELECT e.attendanceId, e.eventId, ev.eventName, e.memberId, s.memberName, e.attendanceCode FROM eventmemberattendance e JOIN squadmember s on e.memberId = s.memberId JOIN squadevent ev on e.eventId = ev.eventId WHERE e.eventId = @eventId";

            List<EventMemberAttendanceModel> eventMemberAttendance = null;
            SquadModel squadModel = null;

            try
            {
                using (var conn = _context.CreateConnection())
                {
                    var multi = conn.QueryMultiple(squadQuery + ";" + squadMemberQuery + ";" + eventQuery, new { slugId });

                    var squad = multi.Read<SquadModel>().SingleOrDefault();
                    var squadMembers = multi.Read<SquadMemberModel>().ToList();
                    var squadEvents = multi.Read<SquadEventModel>().ToList();

                    foreach (SquadEventModel sEvent in squadEvents)
                    {
                        eventMemberAttendance = conn.Query<EventMemberAttendanceModel>(eventMemberAttendanceQuery, new { sEvent.eventId }).ToList();
                        sEvent.eventMemberAttendance = eventMemberAttendance;
                    }

                    if (squad != null)
                    {
                        squadModel = new SquadModel()
                        {
                            squadId = squad.squadId,
                            squadName = squad.squadName,
                            slug = squad.slug,
                            createdAt = squad.createdAt,
                            members = squadMembers,
                            events = squadEvents,
                        };
                    }

                    return squadModel;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return squadModel;
            }
        }

        public List<SquadEventModel> UpdateEventMemberAttendance(List<EventAttendance> eventAttendance)
        {
            string updateEventAttendanceQuery = "UPDATE eventmemberattendance SET attendanceCode = @attendanceCode WHERE memberId = @memberId AND eventId = @eventId";

            string squadEventQuery = "SELECT * FROM squadevent WHERE squadId = (SELECT squadId FROM squadEvent WHERE eventId = @eventId LIMIT 1)";

            string eventMemberAttendanceQuery = "SELECT e.attendanceId, e.eventId, ev.eventName, e.memberId, s.memberName, e.attendanceCode FROM eventmemberattendance e JOIN squadmember s on e.memberId = s.memberId JOIN squadevent ev on e.eventId = ev.eventId WHERE e.eventId = @eventId";

            List<EventMemberAttendanceModel> eventMemberAttendance = null;

            IDbTransaction transaction = null;

            try
            {
                using (var conn = _context.CreateConnection())
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    //update each member attendance
                    foreach (EventAttendance attendance in eventAttendance)
                    {
                        conn.Query(updateEventAttendanceQuery, new { attendance.attendanceCode, attendance.memberId, attendance.eventId }, transaction);
                    }

                    //grab all the events that are tried to the squad we are currently looking at
                    List<SquadEventModel> squadEvents = conn.Query<SquadEventModel>(squadEventQuery, new { eventAttendance.First().eventId }, transaction).ToList();

                    //embed the list of eventmemberattendance per squadevent in the squadevent list
                    foreach (SquadEventModel sEvent in squadEvents)
                    {
                        eventMemberAttendance = conn.Query<EventMemberAttendanceModel>(eventMemberAttendanceQuery, new { sEvent.eventId }, transaction).ToList();
                        sEvent.eventMemberAttendance = eventMemberAttendance;
                    }

                    transaction.Commit();
                    //return list of squadevents that each have a list of squadeventmemberattendace
                    return squadEvents;

                }
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                Console.WriteLine(ex);
                return null;
            }
        }

        public bool DeleteSquadMember(long squadMemberId)
        {
            string deleteQuery = "DELETE FROM SquadMember WHERE memberId = @MemberId";
            bool success = false;

            try
            {
                using (var conn = _context.CreateConnection())
                {
                    int rowsAffected = conn.Execute(deleteQuery, new { MemberId = squadMemberId });

                    if (rowsAffected > 0)
                    {
                        success = true;
                    }
                    return success;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return success;
            }
        }

        public string AddSquadMember(FormInputModel.SquadMember squadMember)
        {
            string memberInsertQuery = "INSERT INTO SquadMember (membername, squadid) VALUES (@memberName, @squadId) RETURNING memberId";
            string squadQuery = "SELECT slug FROM squad WHERE squadId = @squadId";
            string getEventListQuery = "SELECT e.eventId FROM squadEvent e JOIN squad s ON e.squadId = s.squadId WHERE s.squadId = @squadId";
            string attendanceInsertQuery = "INSERT INTO public.eventmemberattendance (eventId, memberId, attendanceCode) VALUES (@eventId, @memberId, @attendancecode)";

            int attendanceCode = (int)AttendanceCode.NotAttending;
            IDbTransaction transaction = null;
            List<long> memberIdList = new List<long>();

            try
            {
                using (var conn = _context.CreateConnection())
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    //insert each member
                    foreach (var memberName in squadMember.squadMembers)
                    {
                        long memberId = conn.ExecuteScalar<long>(memberInsertQuery, new { memberName, squadMember.squadId }, transaction);
                        memberIdList.Add(memberId);
                    }

                    IEnumerable<long> eventIds = conn.Query<long>(getEventListQuery, new { squadMember.squadId }, transaction);

                    //for each event that is tied to that squad, I need to insert default row for newly added member
                    foreach (var eventId in eventIds)
                    {
                        for (int i = 0; i < memberIdList.Count(); i++)
                        {
                            var memberId = memberIdList.ElementAt(i);
                            conn.Execute(attendanceInsertQuery, new { eventId, memberId, attendanceCode }, transaction);
                        }
                    }

                    string slug = conn.QuerySingle<string>(squadQuery, new { squadMember.squadId }, transaction);

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
    }
}
