using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Apoa.Models;
using Microsoft.AspNetCore.Identity;
using Apoa.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Apoa.Controllers
{
    [Authorize(Roles = "Opettaja")]
    public class StudentsController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _manager;
        public StudentsController(ApplicationDbContext context, UserManager<ApplicationUser> manager)
        {
            _context = context;
            _manager = manager;
        }
        public async Task<IActionResult> Index(string id, int responseid = 0)
        {
            var user = await _context.Users.Include(u => u.UserGroups).FirstOrDefaultAsync(u => u.Email == User.Identity.Name);


            //Hae id:n perusteella oikea user mistä clickattiin

            //TODO vastaus vain kerran viikossa ja näytä käkkyrän x akselilla viikot tms.

            var ownUsers = _context.UserGroups
                        // Lisätään user ja sille rooli 
                        .Include(ug => ug.User)
                        // Lisätään ryhmä
                        .Include(ug => ug.Group)
                        .Where(ug => user.UserGroups.Select(ugg => ugg.GroupId).Contains(ug.GroupId)).ToList();


            return View(ownUsers);
        }

        public IActionResult Report(string id, int year, int cId = 0)
        {

            if (string.IsNullOrWhiteSpace(id))
                return View(null);

            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            //hakee kaikki userit kannasta ottaa ekan idn perusteella 
            // //Lisäksi kaikki vastaukset saadaan listalle
            //Otetaan kaikki

            var responses = _context.Responses.Where(r => r.UserId == id && r.AnswerDate > DateTime.Now.AddDays(-90))
                                                .Include(r => r.Assessment)
                                                    .ThenInclude(a => a.Category)
                                                .Include(r => r.Assessment)
                                                    .ThenInclude(a => a.ResponseOptions).ToList();
        
            //Otettaan kaikki kysymykset
            var assessments = responses.Select(r => r.Assessment);
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

            var model = new UserResponseViewModel()
            {
                Year = year,
                User = user,
                Categories = categories
            };


            // var model = new UserResponseViewModel(){
            //     User= _context.Users.FirstOrDefault(u => u.Id==id),
            //     Responses=_context.Responses.Where(r => r.AssessmentId==responseid).ToList()
            //     //Group= _context.Groups.Include(g => g.CategoryGroups).ThenInclude(cg => cg.Category).ThenInclude(c => c.Assessments).FirstOrDefault(g => g.Id == id)                
            // };
            // model.Assessments = _context.Responses.Include(r => r.Assessment).Select(r => r.Assessment).Distinct().ToList();

            //todo 
            //model.GroupResponses = _context.Responses.Include(r => r.Assessment).ThenInclude(a => a.Category).ThenInclude(c => c.CategoryGroups.Where(cg => model.User.UserGroups.Select(u => u.GroupId).Contains(cg.GroupId))).ToList();
            return View(model); //model

        }

        public IActionResult GetYearData(string userId, int year, int cId) {

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null) {

                var responses = _context.Responses.OrderBy(r => r.Month).Where(r => r.UserId == userId && r.Year == year)
                                                .OrderBy(r => r.AssessmentId)
                                                .OrderBy(r => r.Month)
                                                .Include(r => r.Assessment)
                                                    .ThenInclude(a => a.Category)
                                                .Include(r => r.Assessment)
                                                    .ThenInclude(a => a.ResponseOptions)
                                                .ToList();

                var assessments = responses.Where(r => r.Assessment.Category.Id == cId).Select(r => r.Assessment).Distinct().ToList();
                
                //Dictionary<int, double> data = new Dictionary<int, double>();
                List<object> data = new List<object>();
                
                foreach (var a in assessments)
                {
                    List<object> monthData = new List<Object>();      
                    for(int i = 1; i <= 12; i++) {

                        var responsesInMont = a.Responses.Where(r => r.Month == i);
                        int sum = 0;

                        foreach(var rm in responsesInMont) {

                            sum += rm.Value;

                        }

                        try {
                            monthData.Add(new { Month = i, Value = (double) sum / (double) responsesInMont.Count()});
                        } catch(DivideByZeroException) {
                            monthData.Add(new { Month = i, Value = 0});
                        }
                    }
                    data.Add(new {Id = a.Id, Name = a.Name, data = monthData, min = a.ResponseOptions.Min, max = a.ResponseOptions.Max});

                }

                return Json(data);          

            }

            return Json(new {});

        }

    }

}
