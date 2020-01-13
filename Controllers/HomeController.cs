using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Apoa.Models;
using System.Security.Principal;
using Apoa.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Apoa.Controllers
{
    [Authorize(Roles = "Opiskelija, Opettaja")]
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _manager;


        public HomeController(UserManager<ApplicationUser> manager, ApplicationDbContext context)
        {
            this._context = context;
            _manager = manager;
        }
        public async Task<IActionResult> Index(int cId)
        {

            var user = await _manager.FindByEmailAsync(User.Identity.Name);

            var responses = _context.Responses
                                .Include(r => r.Assessment)
                                    .ThenInclude(a => a.Category)
                                .Include(r => r.Assessment)
                                    .ThenInclude(a => a.ResponseOptions)
                                .Where(r => r.UserId == user.Id && r.AnswerDate > DateTime.Now.AddDays(-90)).ToList();

            //Otettaan kaikki kysymykset
            var assessments = responses.Select(r => r.Assessment).ToList();
            //Otetaan kaikki kysymys kategoriat kertaalleeen (distinct)
            var categories = assessments.Select(a => a.Category).Distinct().ToList();

            // Swap here listan ensimmäiseksi alkioksi listan eka
            if (cId != 0 && categories.Count() > 0)
            {
                var tmp = categories.FirstOrDefault(c => c.Id == cId);
                //tempin indexi laatikoiden sisälle
                categories[categories.IndexOf(tmp)] = categories[0];
                categories[0] = tmp;
            }

            var model = new HomeViewModel()
            {
                Categories = categories,
                User = user,
                Groups = _context.Groups.ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> GetYearData(int year, int cId) 
        {

            var user = await _manager.FindByEmailAsync(User.Identity.Name);

            if(user != null) {
                
                var category = _context.Categories
                                    .Include(c => c.Assessments)
                                        .ThenInclude(a => a.Responses)
                                    .Include(c => c.Assessments)
                                        .ThenInclude(a => a.ResponseOptions)
                                    .FirstOrDefault(c => c.Id == cId);

                List<object> aData = new List<object>();
                if(category != null){

                    foreach (var a in category.Assessments)
                    {
                        List<object> rData = new List<object>();

                        for(int i = 1; i <= 12; i++) {

                            var responsesInMont = a.Responses.Where(r => r.Month == i && r.Year==year);
                            var sum = 0;

                            foreach(var r in responsesInMont) 
                            {
                                sum += r.Value;
                            }

                            try {
                                rData.Add(new {Month = i, Value = (double) sum / responsesInMont.Count()});
                            } catch(DivideByZeroException) {
                                rData.Add(new {Month = i, Value = 0});
                            }

                        }
                        aData.Add(new {id = a.Id, name = a.Name, data = rData, min = a.ResponseOptions.Min, max = a.ResponseOptions.Max});      
                    }

                    return Json(aData);
                }

                return Json(new {status = false, message="Kategoriaa ei löytynyt"});

            } 

            return Json(new {status = false, message="Käyttäjää ei löytynyt"});
        }

        [HttpPost]
        public async Task<IActionResult> SaveStudentGroup(int groupId)
        {
            var user = await _manager.FindByEmailAsync(User.Identity.Name);

            bool status = false;
            string message = "Käyttäjä virhe";

            if (user == null)
                return Json(new { status = status, message = message });

            var group = _context.UserGroups.FirstOrDefault(ug => ug.UserId == user.Id);
            message = "Kuulut jo ryhmään";

            if (group != null)
                return Json(new { status = status, message = message });

            status = true;
            message = "Ryhmään liittyminen onnistui";

            group = new UserGroup() { UserId = user.Id, GroupId = groupId };

            try
            {
                _context.UserGroups.Add(group);
                _context.SaveChanges();
                // Jos tulee exception tiedetään, että kuuluu jo ryhmään
            }
            catch (Exception)
            {
                status = false;
                message = "Jokin meni pieleen";
            }

            return Json(new { status = status, message = message });

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}