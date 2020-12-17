using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UMLProjectX.DAL.Models;
using UMLProjectX.Models;


namespace UMLProjectX.DAL
{
    public sealed class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<FilmScore> FilmScores { get; set; }
        public DbSet<BanWord> BanWords { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public IQueryable<User> ReadUsers() => Users;

        public IQueryable<Film> ReadFilms() => Films;

        public IQueryable<Review> ReadReviews() => Reviews;

        public IQueryable<FilmScore> ReadFilmScores() => FilmScores;

        public IQueryable<BanWord> ReadBanWords() => BanWords;

        public bool ContainsLogin(string login) => ReadUsers().FirstOrDefault(x => x.Login == login) != null;

        public Task<User> UserValid(string login, string password) =>
            ReadUsers().FirstOrDefaultAsync(x => x.Login == login && x.Password == password);

        public User AddUser(User user)
        {
            var ret = Users.Add(user);
            SaveChanges();

            return ret.Entity;
        }

        public void DeleteUser(User user)
        {
            Users.Remove(user);
            SaveChanges();
        }

        public void DeleteUserByLogin(string login)
        {
            var user = ReadUsers().FirstOrDefault(u => u.Login == login);
            DeleteUser(user);
        }

        public Film AddFilm(FilmModel filmModel)
        {
            var film = new Film()
            {
                RusName = filmModel.RusName,
                Year = filmModel.Year,
                Description = filmModel.Description,
                Director = filmModel.Director,
                Genres = 0,
            };
            foreach (var genre in filmModel.Genres.Where(g => g.IsSelected))
            {
                film.Genres |= (int)genre.Name;
            }

            byte[] imageData = null;
            using (var binaryReader = new BinaryReader(filmModel.Picture.OpenReadStream()))
            {
                imageData = binaryReader.ReadBytes((int)filmModel.Picture.Length);
            }

            film.Picture = imageData;

            var ret = Films.Add(film);

            SaveChanges();
            FilmScores.Add(new FilmScore()
            {
                FilmId = film.FilmId,
                AvgScore = 0.0,
                ReviewsNum = 0
            });
            SaveChanges();
            var test = ReadFilms().Count(x => true);
            return ret.Entity;
        }

        public Film FindFilmById(int id) => ReadFilms().FirstOrDefault(f => f.FilmId == id);
        public User FindUserByLogin(string login) => ReadUsers().FirstOrDefault(u => u.Login == login);
        public FilmScore FindFilmScoreByFilmId(int id) => ReadFilmScores().FirstOrDefault(fs => fs.FilmId == id);

        public Review AddReview(ReviewModel model)
        {
            var user = FindUserByLogin(model.UserLogin);
            var film = FindFilmById(model.FilmId);

            var oldReview = FindReviewsForFilm(film.FilmId).FirstOrDefault(x => x.UserId == user.UserId);
            if (oldReview != null) 
                DeleteReview(oldReview);

            var review = Reviews.Add(new Review()
            {
                Time = DateTime.Now,
                Score = model.Score,
                ReviewText = model.ReviewText,
                UserId = user.UserId,
                UserLogin = user.Login,
                User = user,
                FilmId = film.FilmId,
                Film = film,
                FilmName = film.RusName
            });

            SaveChanges();

            UpdateFilmScore(film.FilmId, model.Score);

            SaveChanges();

            return review.Entity;
        }

        public List<Review> FindReviewsForFilm(int filmId) => ReadReviews().Where(r => r.FilmId == filmId).ToList();

        public List<Review> FindReviewsForUser(int userId) => ReadReviews().Where(r => r.UserId == userId).ToList();

        public Film EditFilm(FilmModel filmModel)
        {
            var film = new Film()
            {
                FilmId = filmModel.FilmId,
                RusName = filmModel.RusName,
                Year = filmModel.Year,
                Description = filmModel.Description,
                Director = filmModel.Director,
                Genres = 0,
            };
            foreach (var genre in filmModel.Genres.Where(g => g.IsSelected))
            {
                film.Genres |= (int)genre.Name;
            }

            byte[] imageData = null;
            using (var binaryReader = new BinaryReader(filmModel.Picture.OpenReadStream()))
            {
                imageData = binaryReader.ReadBytes((int)filmModel.Picture.Length);
            }

            film.Picture = imageData;

            var newVal = Films.Update(film);

            SaveChanges();

            return newVal.Entity;
        }

        public void DeleteFilm(Film film)
        {
            Films.Remove(film);

            SaveChanges();
        }

        public void DeleteFilmById(int filmId)
        {
            var film = ReadFilms().FirstOrDefault(r => r.FilmId == filmId);
            DeleteFilm(film);
        }

        public void DeleteReview(Review review)
        {
            UpdateFilmScore2(review.FilmId, review.Score);
            Reviews.Remove(review);
            SaveChanges();
        }

        public void DeleteReviewById(int reviewId)
        {
            var review = ReadReviews().FirstOrDefault(r => r.ReviewId == reviewId);
            DeleteReview(review);
        }

        public FilmScore EditFilmScore(int filmId, FilmScore fs)
        {
            var oldVal = FindFilmScoreByFilmId(filmId);
            FilmScores.Remove(oldVal);
            var newVal = FilmScores.Add(fs);

            SaveChanges();

            return newVal.Entity;
        }

        private FilmScore UpdateFilmScore(int filmId, int score)
        {
            var filmScore = FindFilmScoreByFilmId(filmId);

            var newAvg = (filmScore.AvgScore * filmScore.ReviewsNum + score) / (filmScore.ReviewsNum + 1);

            return EditFilmScore(filmId, new FilmScore()
            {
                AvgScore = newAvg,
                ReviewsNum = filmScore.ReviewsNum + 1,
                FilmId = filmScore.FilmId
            });
        }

        private FilmScore UpdateFilmScore2(int filmId, int score)
        {
            var filmScore = FindFilmScoreByFilmId(filmId);
            
            var newAvg = (filmScore.AvgScore * filmScore.ReviewsNum - score) / (filmScore.ReviewsNum - 1);
            if (double.IsNaN(newAvg) || filmScore.ReviewsNum == 1)
            {
                newAvg = 0;
            }

            return EditFilmScore(filmId, new FilmScore()
            {
                AvgScore = newAvg,
                ReviewsNum = filmScore.ReviewsNum - 1,
                FilmId = filmScore.FilmId
            });
        }

        public BanWord AddBanWord(BanWord word)
        {
            var ret = BanWords.Add(word);
            SaveChanges();

            return ret.Entity;
        }
    }
}
