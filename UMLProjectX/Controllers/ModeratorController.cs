using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMLProjectX.DAL;
using UMLProjectX.DAL.Models;
using UMLProjectX.Models;
using UMLProjectX.Tools;

namespace UMLProjectX.Controllers
{
    [Authorize(Roles = "mod")]
    public class ModeratorController : Controller
    {
        private DataContext _db;
        private AutoModeration _autoModeration;

        public ModeratorController(DataContext context)
        {
            _db = context;
            _autoModeration = new AutoModeration(context);
        }
        [HttpGet]
        public IActionResult AddFilm()
        {
            var filmModel = new FilmModel();
            return View(filmModel);
        }

        [HttpPost]
        public IActionResult AddFilm(FilmModel filmModel)
        {
            if (ModelState.IsValid && _autoModeration.PassAutoModeration(filmModel.Description))
            {
                _db.AddFilm(filmModel);

                return RedirectToAction("Index", "Home");
            }

            return View(filmModel);
        }

        [HttpGet]
        public IActionResult AddBanWord()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBanWord(BanWordModel model)
        {
            _db.AddBanWord(new BanWord() {Adder = User.Identity.Name, Content = model.Word});
            ViewBag.BanWords = _db.ReadBanWords();
            return View();
        }

        [HttpGet]
        public IActionResult BanComment(int reviewId, int filmId)
        {
            _db.DeleteReviewById(reviewId);
            return RedirectToAction("ShowFilm", "Home", new { filmId = filmId });
        }

        [HttpGet]
        public IActionResult EditDeleteFilm(int filmId)
        {
            var film = _db.FindFilmById(filmId);
            var model = new FilmModel(film);
            return View(model);
        }

        public IActionResult EditFilm(FilmModel model)
        {
            var res = _db.EditFilm(model);

            return RedirectToAction("EditDeleteFilm", "Moderator", new {filmId = res.FilmId});
        }

        public IActionResult DeleteFilm(int filmId)
        {
            _db.DeleteFilmById(filmId);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult BanUser(int userId)
        {
            _db.DeleteUserById(userId);

            return RedirectToAction("Index", "Home");
        }
    }
}
