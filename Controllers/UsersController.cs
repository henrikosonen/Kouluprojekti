using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Apoa.Models;
using Microsoft.EntityFrameworkCore;
using Apoa.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Apoa.Controllers
{

    [Authorize(Roles = "Opettaja")]
    public class UsersController : Controller
    {

        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _manager;

        public UsersController(UserManager<ApplicationUser> manager, ApplicationDbContext context)
        {
            _context = context;
            _manager = manager;
        }
        public IActionResult Index()
        {
            var model = new UsersViewModel();

            model.Users = _context.Users.Include(u => u.UserGroups).ToList();
            model.Groups = _context.Groups.ToList();

            return View(model);

        }

        [HttpPost]
        public IActionResult EditGroup(int groupId, string userId)
        {

            if (!string.IsNullOrWhiteSpace(userId) && groupId > 0)
            {

                // Haetaan userin ensimmäinen kannasta tuleva ryhmä
                var userGroup = _context.UserGroups.FirstOrDefault(ug => ug.UserId == userId);

                //_context.UserGroups.Remove(userGroup);
                if (userGroup != null)
                {
                    _context.UserGroups.Remove(userGroup);
                    _context.SaveChanges();
                }

                _context.UserGroups.Add(new UserGroup() { UserId = userId, GroupId = groupId });
                // tallennetaan tietokantaan
                if (_context.SaveChanges() > 0)
                    return Json(new { status = true, message = "Käyttäjän rtyhmä on tallennettu" });
                else
                    return Json(new { status = false, message = "Jotain meni pieleen" });

            }


            return Json(new { status = false, message = "Käyttäjää ei löytynyt" });

            // TODO tässä pitää ottaa kiinni vain userin ryhmät
            // var model = new EditGroupViewModel() {
            //     Group = _context.Groups.Include(u => u.GroupUsers).FirstOrDefault(g => g.Id == id),
            //     UserGroups = _context.UserGroups.ToList()
            // };
            // model.UserIds = model.Group.GroupUsers.Select(ug => ug.GroupId).ToList();

        }

    }

}
