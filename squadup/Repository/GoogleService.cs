using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System.Text;
using System.Text.Json;

namespace squadup.Repository
{
    public class GoogleService
    {
        private string primaryCalendar;
        private readonly CalendarService _calendarService;

        public GoogleService(IConfiguration configuration)
        {
            var config = configuration;

            var credentialJson = new
            {
                type = "service_account",
                project_id = config["GoogleCalendarApi:ProjectId"],
                private_key_id = config["GoogleCalendarApi:PrivateKeyId"],
                private_key = config["GoogleCalendarApi:PrivateKey"].Replace("\\n", "\n"),
                client_email = config["GoogleCalendarApi:ClientEmail"],
                client_id = config["GoogleCalendarApi:ClientId"],
                auth_uri = "https://accounts.google.com/o/oauth2/auth",
                token_uri = "https://oauth2.googleapis.com/token",
                auth_provider_x509_cert_url = "https://www.googleapis.com/oauth2/v1/certs",
                client_x509_cert_url = config["GoogleCalendarApi:CertUrl"],
                universe_domain = "googleapis.com",
            };

            var client_json = JsonSerializer.Serialize(credentialJson);
            GoogleCredential credentials = GoogleCredential.FromJson(client_json);
            var credential = credentials.CreateScoped(CalendarService.Scope.Calendar);

            primaryCalendar = config["GoogleCalendarApi:CalendarId"];

            // Create the CalendarService instance using the ServiceAccountCredential
            _calendarService = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Squadup Web App",
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

        public string DecodeBase64String(string encoded)
        {
            var base64EncodedBytes = Convert.FromBase64String(encoded);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
