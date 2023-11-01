using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System.Text;

namespace squadup.Repository
{
    public class GoogleService
    {

        private const string client_secret_path = "secret_service.json";
        private const string primaryCalendar = "10474eeb3b2ed928bda49df3542100b9ea20aa153427e03d274fd2c22d13b7cd@group.calendar.google.com";
        private readonly CalendarService _calendarService;

        public GoogleService()
        {

            var credential = GoogleCredential.FromFile(client_secret_path).CreateScoped(CalendarService.Scope.Calendar);

            // Create the CalendarService instance using the ServiceAccountCredential
            _calendarService = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });

        }

        public string insertCalendarEvent(string eventName, DateTime eventDate, string eventDescription)
        {
            Event newEvent = createEventContext(eventName, eventDate, eventDescription);

            try
            {
                EventsResource.InsertRequest request = _calendarService.Events.Insert(newEvent, primaryCalendar);
                Event createdEvent = request.Execute();

                string shareableEventLink = getShareableLink(createdEvent.HtmlLink);
                return shareableEventLink;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public bool deleteCalendarEvent(string link)
        {
            string eventId = extractEventIdFromLink(link);

            try
            {
                EventsResource.DeleteRequest request = _calendarService.Events.Delete(primaryCalendar, eventId);
                var deleteResponse = request.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        public string getShareableLink(string htmlLink)
        {
            string[] stringParts = htmlLink.Split(new string[] { "event?" }, 2, StringSplitOptions.None);
            string firstArgument = "event?action=TEMPLATE&tm";
            string lastArgument = $"&tmsrc={primaryCalendar}";
            string shareableLink = stringParts[0] + firstArgument + stringParts[1] + lastArgument;

            return shareableLink;
        }

        public string extractEventIdFromLink(string link)
        {
            string[] stringParts = link.Split(new string[] { "eid=" }, 2, StringSplitOptions.None);
            string[] stringPartsTwo = stringParts[1].Split(new string[] { "&tmsrc" }, 2, StringSplitOptions.None);
            return DecodeBase64String(stringPartsTwo[0]).Split(new string[] { " " }, 2, StringSplitOptions.None)[0];
        }

        public Event createEventContext(string eventName, DateTime eventDate, string eventDesc)
        {
            string eventSummary = eventName;
            string eventLocation = "Catskill NY";
            string eventDescription = eventDesc;

            //string start = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz");
            string start = eventDate.ToString("yyyy-MM-ddTHH:mm:sszzz");
            string end = DateTime.Parse(start).AddDays(1).ToString("yyyy-MM-ddTHH:mm:sszzz");

            EventDateTime startDate = new EventDateTime()
            {
                DateTime = DateTime.Parse(start),
                TimeZone = "America/New_York",
            };

            EventDateTime endDate = new EventDateTime()
            {
                DateTime = DateTime.Parse(end),
                TimeZone = "America/New_York",
            };

            Event newEvent = new Event()
            {
                Summary = eventSummary,
                Description = eventDescription,
                Start = startDate,
                End = endDate,
            };

            return newEvent;
        }

        public bool getCalendarList()
        {
            dynamic calListRequest = null;

            string eventTwo = "_60q30c1g60o30e1i60o4ac1g60rj8gpl88rj2c1h84s34h9g60s30c1g60o30c1g6go46ghg8p0k8h9n6ssk8e9g64o30c1g60o30c1g60o30c1g60o32c1g60o30c1g8krk4c9m64pj0dhi8ksj4dhk6l0j2g9j8l1kachg6csj0dq184o0";

            try
            {
                calListRequest = _calendarService.Events.Get(primaryCalendar, eventTwo).Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine(calListRequest);
            return true;
        }

        public string DecodeBase64String(string encoded)
        {
            var base64EncodedBytes = Convert.FromBase64String(encoded);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
