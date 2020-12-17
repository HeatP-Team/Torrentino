using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UMLProjectX.DAL.Models;
using UMLProjectX.Tools;

namespace UMLProjectX.Models
{
    public class FilmSearcherModel
    {
        public string Name { get; set; }
        public int Year { get; set; }
        public List<GenreCheckBox> Genres { get; set; }
        public string Director { get; set; }

        public FilmSearcherModel()
        {
            Genres = Enum.GetValues(typeof(Genre))
                .Cast<Genre>()
                .Select(g => new GenreCheckBox() { IsSelected = false, Name = g })
                .ToList();
        }

        public FilmSearcherDto ToDto()
        {
            var fsd =  new FilmSearcherDto()
            {
                Name = Name,
                Director = Director,
                Year = Year != 0 ? Year : (int?)null,
            };

            int genres = 0;
            foreach (var genre in Genres.Where(g => g.IsSelected))
            {
                genres |= (int)genre.Name;
            }

            fsd.Genres = genres;

            return fsd;
        }
    }
}
