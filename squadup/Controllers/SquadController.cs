using Microsoft.AspNetCore.Mvc;
using squadup.Models;
using squadup.Repository;
using System.Diagnostics;

namespace squadup.Controllers
{
    public class SquadController : Controller
    {
        private readonly ILogger<SquadController> _logger;

        private readonly IGroupRepository _groupRepository;

        public SquadController(ILogger<SquadController> logger, IGroupRepository groupRepository)
        {
            _logger = logger;
            _groupRepository = groupRepository;
        }

        // {slug}? ? makes it optional
        [HttpGet("squad/{slug?}")]
        public IActionResult Index(string slug)
        {
            Console.WriteLine(slug);

            SquadModel squadViewData = null;

            if (!string.IsNullOrEmpty(slug))
            {
                squadViewData = _groupRepository.GetSingleSquad(slug);
            }

            if(squadViewData != null) 
            {
                return View(squadViewData);
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}