using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UMLProjectX.API;
using UMLProjectX.DAL;
using UMLProjectX.DAL.Models;
using UMLProjectX.Models;
using UMLProjectX.Tools;

namespace UMLProjectX.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FilmSearcher _searcher;
        private readonly DataContext _db;
        private readonly AutoModeration _autoModeration;
        private readonly Kinopoisk _kinopoisk = new Kinopoisk();
        private readonly Ivi _ivi = new Ivi();

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _searcher = new FilmSearcher(context);
            _autoModeration = new AutoModeration(context);
            _db = context;
        }

        public IActionResult Index(FilmSearcherModel model)
        {
            if (model?.Genres == null)
            {
                model = new FilmSearcherModel();
            }
            ViewBag.Films = _searcher.Search(model?.ToDto());
            return View(model);
        }

        [HttpGet]
        public IActionResult ShowFilm(int filmId)
        {
            var film = _db.FindFilmById(filmId);
            ViewBag.RusName = film.RusName;
            ViewBag.Picture = film.Picture;
            if (User.Identity.Name == null)
                ViewBag.Mod = false;
            else
                ViewBag.Mod = _db.Users.FirstOrDefault(u => u.Login == User.Identity.Name).Role == "mod";

            ViewBag.AvgReviewScore = _db.FindFilmScoreByFilmId(film.FilmId).AvgScore;
            ViewBag.Year = film.Year;
            ViewBag.Director = film.Director;
            ViewBag.Description = film.Description;
            var reviews = _db.FindReviewsForFilm(filmId);
            if (User.Identity.Name != null)
            {
                var personalReview = reviews.FirstOrDefault(x => x.UserLogin == User.Identity.Name);
                if (personalReview == null)
                {
                    ViewBag.HaveReview = false;
                }
                else
                {
                    ViewBag.HaveReview = true;
                    ViewBag.PersonalReview = personalReview;
                    reviews.Remove(personalReview);
                }
            }
            ViewBag.Reviews = reviews;
            ViewBag.FilmId = filmId;
            ViewBag.Kinopoisk = _kinopoisk.GetLinkByFilmName(film.RusName);
            ViewBag.Ivi = _ivi.GetLinkByFilmName(film.RusName);
            ViewBag.KinozalLinks = _db.FindLinkForFilm(filmId);

            var genres = new List<string>();
            foreach (var genre in Enum.GetValues(typeof(Genre)).Cast<Genre>())
            {
                if (((int)genre & film.Genres) == (int)genre)
                    genres.Add(genre.ToString());
            }

            ViewBag.Genres = string.Join(", ", genres);

            return View();
        }

        [HttpPost]
        public IActionResult Review(ReviewModel reviewModel)
        {
            reviewModel.UserLogin = User.Identity.Name;

            if (!_autoModeration.PassAutoModeration(reviewModel.ReviewText))
            {
                return RedirectToAction("ShowFilm", "Home", new { filmId = reviewModel.FilmId });
            }

            var review = _db.AddReview(reviewModel);

            return RedirectToAction("ShowFilm", "Home", new {filmId = review.Film.FilmId});
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
