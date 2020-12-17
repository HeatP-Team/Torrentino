using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UMLProjectX.DAL.Models;

namespace UMLProjectX.Models
{
    public class FilmModel
    {
        [Required(ErrorMessage = "Не указано название")]
        public string RusName { get; set; }
        [Required(ErrorMessage = "Не указан год выхода")]
        public int Year { get; set; }
        public List<GenreCheckBox> Genres { get; set; }
        [Required(ErrorMessage = "Не указан режисcер")]
        public string Director { get; set; }
        [Required(ErrorMessage = "Не указано описание")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Не добавлена картинка")]
        public IFormFile Picture { get; set; }
        public int FilmId { get; set; }

        public FilmModel()
        {
            Genres = Enum.GetValues(typeof(Genre))
                .Cast<Genre>()
                .Select(g => new GenreCheckBox() {IsSelected = false, Name = g})
                .ToList();
        }

        public FilmModel(Film film)
        {
            FilmId = film.FilmId;
            RusName = film.RusName;
            Year = film.Year;
            Description = film.Description;
            Director = film.Director;
            Genres = Enum.GetValues(typeof(Genre))
                .Cast<Genre>()
                .Select(g => new GenreCheckBox() { IsSelected = ((film.Genres & (int)g) == (int)g), Name = g })
                .ToList();
        }
    }

    public class GenreCheckBox
    {
        public Genre Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
