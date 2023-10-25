using Microsoft.AspNetCore.Mvc;
using squadup.Models;
using squadup.Repository;
using System.Diagnostics;
using static squadup.Models.FormInputModel;

namespace squadup.Controllers
{
    public class SquadController : Controller
    {
        private readonly ILogger<SquadController> _logger;

        private readonly IGroupRepository _groupRepository;

        private readonly GoogleService _googleService;

        public SquadController(ILogger<SquadController> logger, IGroupRepository groupRepository)
        {
            _logger = logger;
            _groupRepository = groupRepository;
            _googleService = new GoogleService();
        }

        // {slug}? ? makes it optional
        [HttpGet("squad/{slug?}")]
        public IActionResult Index(string slug)
        {
            //if we visit the squad path with no param  then just return the regular view
            SquadModel squadViewData = null;
            if (string.IsNullOrEmpty(slug))
            {
                return View(squadViewData);
            }
            else
            {
                squadViewData = _groupRepository.GetSingleSquad(slug);
                TempData["SuccessMessage"] = $"No squad found for the following code: {slug}";
                return View(squadViewData);
            }
        }

        [HttpPost]
        public IActionResult Index(FormInputModel.Squad squad)
        {
            // Check if the provided slug is not empty
            if (!string.IsNullOrEmpty(squad.slug))
            {
                // Redirect to the GET action with the provided slug
                return RedirectToAction("Index", "Squad", new { slug = squad.slug });
            }
            else
            {
                TempData["ErrorMessage"] = "Slug cannot be empty.";
                return RedirectToAction("Index", "Squad");
            }
        }

        [HttpPost]
        public IActionResult Destination(FormInputModel.SquadEvent squadEvent)
        {
            string slugId = _groupRepository.AddSquadEvent(squadEvent);

            if (!string.IsNullOrEmpty(slugId))
            {
                return RedirectToAction("Index", "Squad", new { slug = slugId });
            }

            // If the slug is empty, you can handle it as needed, e.g., show an error message
            TempData["ErrorMessage"] = "Can't create squad event";

            // Redirect back to the GET action without a slug
            return View();
        }

        [HttpPost]
        public IActionResult DeleteEvent()
        {
            string eventId = Request.Form["eventId"];

            string slugId = _groupRepository.DeleteSquadEvent(long.Parse(eventId));

            if (!string.IsNullOrEmpty(slugId))
            {
                return RedirectToAction("Index", "Squad", new { slug = slugId });
            }

            // If the slug is empty, you can handle it as needed, e.g., show an error message
            TempData["ErrorMessage"] = "Can't create squad event";

            // Redirect back to the GET action without a slug
            return View();
        }

        [HttpGet("squad/GetEventAttendance")]
        public IActionResult GetEventAttendance(int eventId)
        {
            var eventAttendance = _groupRepository.GetEventMemberAttendance(eventId);

            return PartialView("_ViewEventAttendancePartial", eventAttendance);
        }

        [HttpPost]
        public IActionResult UpdateEventAttendance(int memberId, int attendanceCode, int eventId)
        {

            EventAttendance attendance = new EventAttendance
            {
                memberId = memberId,
                attendanceCode = (AttendanceCode)attendanceCode,
                eventId = eventId,
            };

            var eventAttendance = _groupRepository.UpdateEventMemberAttendance(attendance);
            return PartialView("_ViewEventAttendancePartial", eventAttendance);
        }

        [HttpPost]
        public IActionResult DeleteSquadMember(long memberId)
        {
            bool success = _groupRepository.DeleteSquadMember(memberId);

            if (success)
            {
                return Json(new { success = true, message = "Member deleted" });
            }

            return Json(new { success = false, message = "Failed to delete member" });
        }


        [HttpPost]
        public IActionResult AddSquadMember(long squadId, string squadMember)
        {
            bool success = _groupRepository.AddSquadMember(squadId, squadMember);

            if (success)
            {
                return Json(new { success = true, message = "Member added" });
            }

            return Json(new { success = false, message = "Failed to add member" });
        }

        [HttpGet("squad/AddGoogleCalendar")]
        public IActionResult AddGoogleCalendar()
        {
            bool success = _googleService.getCalendarList();

            if (success)
            {
                return Json(new { success = true, message = "Member added" });
            }

            return Json(new { success = false, message = "Failed to add member" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}