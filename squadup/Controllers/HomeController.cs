using Microsoft.AspNetCore.Mvc;
using squadup.Models;
using squadup.Repository;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

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
        public IActionResult Index(FormInputModel.Squad squad)
        {
            //need to delete duplicate from group input
            if (ModelState.IsValid)
            {
                // Process and insert user data into the database
                squad.squadMembers = parseCommaString(squad.unparsedMembers);

                //insert params into database
                long squadId = _groupRepository.CreateSquad(squad);

                //generate uniqueId for db entry
                string uniqueId = generateSlug(squadId);

                //update squad record with unique identifier
                bool sucess = false;

                if (squadId > 0)
                {
                    string returningId = _groupRepository.UpdateSquad(squadId, uniqueId);

                    if (!string.IsNullOrEmpty(returningId))
                    {
                        sucess = true;
                    }
                }

                string resultMessage;
                resultMessage = sucess ? $"Squad created successfully with slug: {uniqueId}" : "Squad creation Failed";

                TempData["SuccessMessage"] = resultMessage;

                // Redirect to another page
                return RedirectToAction("Index", "Squad", new { slug = uniqueId });
            }

            return View(squad);
        }

        public string generateSlug(long squadId)
        {
            string url = "squadup";

            // Combine squad ID and URL
            string combined = url + squadId;

            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(combined);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Take the last 6 bytes of the hash (for 8 characters)
                byte[] last6Bytes = new byte[6];
                Buffer.BlockCopy(hashBytes, hashBytes.Length - 6, last6Bytes, 0, 6);

                // Perform base64 encoding with URL-safe characters
                string slug = Convert.ToBase64String(last6Bytes)
                    .Replace("+", "-") // Replace + with -
                    .Replace("/", "_") // Replace / with _
                    .Substring(0, 8); // Take the first 8 characters

                return slug;
            }
        }

        public string[] parseCommaString(string unparsedText)
        {
            string[] nameList = unparsedText.Trim(',').Trim().Split(',');

            return nameList;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}