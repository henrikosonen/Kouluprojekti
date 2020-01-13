using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Apoa.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Apoa.Models.ViewModels;



namespace Apoa.Controllers
{

    [Authorize(Roles = "Opettaja")]
    public class GroupController : Controller
    {

        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _manager;
        public GroupController(UserManager<ApplicationUser> manager, ApplicationDbContext context)
        {
            _context = context;
            _manager = manager;
        }
        public IActionResult Index()
        {
            
            ApplicationUser user = null;

            try 
            {
                user = _context.Users.Include(u => u.UserGroups)
                                            .ThenInclude(ug => ug.Group)
                                            .FirstOrDefault(u => u.Email == User.Identity.Name);

            } catch(InvalidOperationException) {

                user = null;

            }

            if (user != null)
            {

                return View(user.UserGroups.Select(ug => ug.Group).ToList());

            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> AddGroup(Group group)
        {
            string message = "Ei voi alle kolme merkkiä pitkää";
            bool status = false;

            
            
            var user = await _manager.FindByEmailAsync(User.Identity.Name);
            


            if (group != null && group.Name.Length > 2)
            {

                try
                {

                    status = true;
                    message = "Onnistui";

                    _context.Groups.Add(group);
                    _context.SaveChanges();
                    _context.UserGroups.Add(new UserGroup() { GroupId = group.Id, UserId = user.Id });
                    _context.SaveChanges();

                }
                catch (Exception)
                {

                    status = false;
                    message = "Jotain meni pieleen";
                }




                return Json(new { status = status, message = message });
            }

            return Json(new { status = status, message = message });
        }

        public ActionResult Report(int id, int year)
        {

            if (id != 0)
            {

                var group = _context.Groups.Include(g => g.CategoryGroups)
                                            .ThenInclude(cg => cg.Category)
                                            .ThenInclude(c => c.Assessments)
                                            .ThenInclude(a => a.Responses)
                                            .FirstOrDefault(g => g.Id == id);

                if (group != null)
                {

                    var model = new GroupViewModel()
                    {
                        Group = group,
                        Categories = group.CategoryGroups.Select(cg => cg.Category).ToList(),
                        Year = year
                    };

                    return View(model);
                }

            }
            return View(null);
        }

        public IActionResult GetData(int cId, int year, int groupId) {


            var group = _context.Groups.Include(g => g.CategoryGroups)
                                            .ThenInclude(cg => cg.Category)
                                            .ThenInclude(c => c.Assessments)
                                            .ThenInclude(a => a.Responses)
                                        .Include(g => g.CategoryGroups)
                                            .ThenInclude(cg => cg.Category)
                                            .ThenInclude(a => a.Assessments)
                                            .ThenInclude(a => a.ResponseOptions)
                                        .FirstOrDefault(g => g.Id == groupId);


            var categoryG = group.CategoryGroups.FirstOrDefault(c => c.CategoryId == cId);

            Category category = null;
            if (categoryG != null)
                category = categoryG.Category;

            List<Object> aData = new List<object>();
            if (category != null && category.Assessments.Count() > 0) {
                foreach (var a in category.Assessments) 
                {

                    List<Object> rData = new List<object>();
                    for(int i = 1; i <= 12; i++) {
                        var responsesInMonth = a.Responses.Where(r => r.Month == i && r.Year == year).ToList();
                        var sum = 0;
                        foreach(var r in responsesInMonth) {
                            
                            sum += r.Value;

                        }
                        try {
                            rData.Add(new {Month = i, Value = (double) sum / responsesInMonth.Count()});
                        } catch(DivideByZeroException) {
                            rData.Add(new {Month = i, Value = 0});
                        }
                    }
                    aData.Add(new { Id = a.Id,
                                    Name = a.Name,
                                    responses = rData,
                                    min = a.ResponseOptions.Min,
                                    max = a.ResponseOptions.Max});
                    
                    return Json(aData);
                }
                return Json(new {});
            }
            return Json(new {});

        }
        

    }

}
