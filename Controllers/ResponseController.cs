using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Apoa.Models;
using Apoa.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace Apoa.Controllers
{

    [Authorize]
    public class ResponseController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _manager;

        public ResponseController(UserManager<ApplicationUser> manager, ApplicationDbContext context)
        {

            this._context = context;
            _manager = manager;

        }
        public async Task<IActionResult> Index(int index = 0)
        {
            var user = await _manager.FindByEmailAsync(User.Identity.Name);
            var model = new ResponseViewModel();
            var userGroups = _context.UserGroups.Where(ug => ug.UserId == user.Id);


            if (userGroups.Any())
            {

                foreach (var g in userGroups)
                {
                    
                    var category = _context.Categories
                        .Include(c => c.Assessments)
                        .ThenInclude(a => a.ResponseOptions)
                        .Include(c => c.CategoryGroups)
                        .Where(c => c.CategoryGroups.Select(cg => cg.GroupId).Contains(g.GroupId));

                    if (category != null)
                        model.categories.AddRange(category);
                    // if(category.Count() != 0) {
                    //     foreach (var c in category)
                    //     {
                    //         if(!model.categories.Contains(c))
                    //             model.categories.Add(c);
                    //     }
                    // }

                }

            }
            model.categories = model.categories.Distinct().Where(c => c.Assessments.Count > 0).ToList();
            model.UserId = user.Id;
            model.index = index;

            return View(model);
        }

        public async Task<IActionResult> AddResponse(List<Response> responses)
        {

            var user = await _manager.FindByEmailAsync(User.Identity.Name);
            bool status = false;
            string message = "Käyttäjää ei löytynyt";

            if (user == null)
                return Json(new { status = status, message = message });

            foreach (var r in responses)
            {
                r.UserId = user.Id;
                r.AnswerDate = DateTime.Now;
                r.Week = GetWeekNumber();
                r.Year = r.AnswerDate.Year;
                r.Month = r.AnswerDate.Month;
            }

            try
            {
                status = true;
                message = "Vastauksesi tallennettiin";
                _context.Responses.AddRange(responses);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                status = false;
                message = "Tietokanta virhe";
            }

            return Json(new { status = status, message = message });
        }

        public int GetWeekNumber()
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }

    }
}