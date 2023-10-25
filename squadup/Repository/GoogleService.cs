using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

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

        public bool insertCalendarEvent()
        {
            Event newEvent = createEventContext();

            try
            {
                EventsResource.InsertRequest request = _calendarService.Events.Insert(newEvent, primaryCalendar);
                Event createdEvent = request.Execute();

                string shareableEventLink = getShareableLink(createdEvent.HtmlLink);
                Console.WriteLine("Event created: {0}", shareableEventLink);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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

        public Event createEventContext()
        {

            string eventSummary = "test event";
            string eventLocation = "Poconos";
            string eventDescription = "Poconos trip have fun";

            string start = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz");
            string end = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm:sszzz");

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
                Location = eventLocation,
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
    }
}
