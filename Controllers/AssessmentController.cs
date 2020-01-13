using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Apoa.Models;
using Microsoft.EntityFrameworkCore;
using Apoa.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Apoa.Controllers
{

    [Authorize(Roles = "Opettaja")]
    public class AssessmentController : Controller
    {
        private ApplicationDbContext _context;

        public AssessmentController(ApplicationDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            var model = _context.Categories.Where(c => c.Deleted == null).Include(c => c.Assessments).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult AddAssessment(int id)
        {

            // if (_context.GetOrCreateUser(User.Identity.Name).RoleId == 2)
            //     return BadRequest();

            var model = _context.Categories.FirstOrDefault(a => a.Id == id);
            return PartialView("_AddAssessment", model);
        }

        [HttpGet]
        public ActionResult EditAssessment(int id)
        {

            //TODO ota vain userin kategoriat tähän malliin, nyt ottaa kaikki

            var model = new EditAssessmentViewModel()
            {
                Assessment = _context.Assessments.Include(a => a.Category).FirstOrDefault(a => a.Id == id),
                Categories = _context.Categories.ToList(),
                responseOptions = _context.ResponseOptions.ToList()
            };

            return PartialView("_EditAssessment", model);

        }

        [HttpGet]
        public ActionResult AddCategory()
        {

            return PartialView("_AddCategory");
        }

        [HttpGet]
        public ActionResult EditCategory(int id)
        {

            // TODO tässä pitää ottaa kiinni vain userin kategoriat

            var model = new EditCategoryViewModel()
            {
                Category = _context.Categories.Include(g => g.CategoryGroups).FirstOrDefault(c => c.Id == id),
                Groups = _context.Groups.ToList()
            };
            model.GroupIds = model.Category.CategoryGroups.Select(cg => cg.GroupId).ToList();

            return PartialView("_EditCategory", model);

        }

        [HttpPost]
        public IActionResult AddCategory(Category model)
        {
            string message = "Lisääminen onnistui";
            Boolean status = true;

            try
            {
                _context.Categories.Add(model);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                status = false;
                message = "Lisääminen ei onnistunut";
            }

            return Json(new { status = status, message = message });
        }

        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {

            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            Boolean status = false;
            string message = "Kategoriaa ei löytynyt";

            if (category != null)
            {

                try
                {
                    _context.Categories.Remove(category);
                    _context.SaveChanges();
                    message = "Kategorian poisto onnistui";
                    status = true;
                }
                catch (Exception)
                {
                    message = "Poistaminen ei onnistunut";
                    status = false;

                }

            }

            return Json(new { status = status, message = message });

        }

        [HttpPost]
        public IActionResult EditCategory(EditCategoryViewModel category)
        {
            var model = _context.Categories.Include(c => c.CategoryGroups).FirstOrDefault(c => c.Id == category.Id);

            string message = "Kategoriaa ei löytynyt";
            Boolean status = false;

            if (model != null)
            {
                model.Name = category.Name;
                model.CategoryGroups = new List<CategoryGroup>();

                if (category.GroupIds.Count() > 0)
                {

                    foreach (int g in category.GroupIds)
                    {

                        var categoryGroup = _context.CategoryGroups.FirstOrDefault(cg => cg.GroupId == g && cg.CategoryId == model.Id);

                        // Jos on olemassa, niin päivitetään, muuten luodaan uusi
                        if (categoryGroup != null)
                        {
                            categoryGroup.CategoryId = category.Id;
                            categoryGroup.GroupId = g;
                        }
                        else
                        {
                            categoryGroup = new CategoryGroup()
                            {
                                CategoryId = category.Id,
                                GroupId = g
                            };
                        }
                        _context.CategoryGroups.Add(categoryGroup);
                    }

                }

                try
                {
                    _context.SaveChanges();
                    status = true;
                    message = "Muokkaaminen onnistui";
                }
                catch (Exception)
                {
                    message = "Jotain meni pieleen";
                }

            }

            return Json(new { status = status, message = message });

        }
        [HttpPost]
        public IActionResult AddAssessment(Assessment model)
        {

            string message = "Lisääminen onnistui";
            Boolean status = true;

            try
            {
                model.ResponseOptionsId = 1;
                model.CreatedAt = DateTime.Now;
                _context.Assessments.Add(model);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                status = false;
                message = "Lisääminen ei onnistunut";
            }

            return Json(new { status = status, message = message });

        }

        [HttpPost]

        public IActionResult DeleteAssessment(int id)
        {
            var assessment = _context.Assessments.FirstOrDefault(a => a.Id == id);
            Boolean status = false;
            string message = "Kysymystä ei löytynyt";

            if (assessment != null)
            {

                try
                {
                    _context.Assessments.Remove(assessment);
                    _context.SaveChanges();
                    message = "Kysymyksen poisto onnistui";
                    status = true;
                }
                catch (Exception)
                {
                    message = "Poistaminen ei onnistunut";
                    status = false;
                }
            }

            return Json(new { status = status, message = message });
        }

        [HttpPost]
        public IActionResult EditAssessment(Assessment assessment)
        {

            var model = _context.Assessments.Include(a => a.Category).FirstOrDefault(a => a.Id == assessment.Id);
            string message = "Kysymystä ei löytynyt";
            Boolean status = false;

            if (model != null)
            {
                model.Name = assessment.Name;
                model.UpdateAt = DateTime.Now;
                model.CategoryId = assessment.CategoryId;
                model.ResponseOptionsId = assessment.ResponseOptionsId;

                try
                {
                    _context.SaveChanges();
                    status = true;
                    message = "Muokkaaminen onnistui";
                }
                catch (Exception)
                {
                    message = "Jotain meni pieleen";
                }

            }

            return Json(new { status = status, message = message });

        }

    }





}
