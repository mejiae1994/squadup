using Microsoft.AspNetCore.Mvc;
using squadup.Models;
using squadup.Repository;
using System.Diagnostics;
using System.Xml.Linq;

namespace squadup.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IGroupRepository _groupRepository;

        public HomeController(ILogger<HomeController> logger, IGroupRepository groupRepository)
        {
            _logger = logger;
            _groupRepository = groupRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // _groupRepository.GetAllGroups();
            return View();
        }


        [HttpPost]
        public IActionResult Index(FormInputModel.Squad group)
        {
            group.squadMembers = parseCommaString(group.unparsedMembers);

            //insert params into database
            _groupRepository.CreateGroup(group);

            return View("Index");
        }

        public string[] parseCommaString(string unparsedText)
        {
            string[] nameList = unparsedText.Trim(',').Trim().Split(',');

            return nameList;
        }

        public bool isvalidGroupId(string groupId)
        {
            return true;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}