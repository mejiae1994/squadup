using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Net;
using System.Threading.Tasks;

namespace squadup.Repository
{
    public class GoogleService
    {

        private const string client_secret_path = "secret_service.json";
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
            return true;
        }

        public bool getCalendarList()
        {
            dynamic calListRequest = null;

            string eventTwo = "_60q30c1g60o30e1i60o4ac1g60rj8gpl88rj2c1h84s34h9g60s30c1g60o30c1g6go46ghg8p0k8h9n6ssk8e9g64o30c1g60o30c1g60o30c1g60o32c1g60o30c1g8krk4c9m64pj0dhi8ksj4dhk6l0j2g9j8l1kachg6csj0dq184o0";


            //we know both queries below work. Now we need to test inserting an event. We'll need to insert an event for every event created in the client and
            //save the shareable link so people can add it
            try
            {
                calListRequest = _calendarService.Events.Get("mejiae1994@gmail.com", eventTwo).Execute();
                //calListRequest = _calendarService.Events.List("mejiae1994@gmail.com").Execute();
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
