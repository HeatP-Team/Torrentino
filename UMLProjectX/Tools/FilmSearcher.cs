using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UMLProjectX.DAL;
using UMLProjectX.DAL.Models;
using static System.String;

namespace UMLProjectX.Tools
{
    public class FilmSearcher
    {
        private DataContext _db;
        public FilmSearcher(DataContext context)
        {
            _db = context;
        }

        public IEnumerable<FilmWithScore> Search(FilmSearcherDto dto)
        {
            if (dto == null)
                return _db.ReadFilms()
                    .Select(x => new FilmWithScore(x, _db.FindFilmScoreByFilmId(x.FilmId)));
            return _db.ReadFilms()
                .Where(x => IsNullOrEmpty(dto.Name) || x.RusName.Contains(dto.Name))
                .Where(x => dto.Year == null || dto.Year == 0|| x.Year == dto.Year)
                .Where(x => dto.Genres == null || dto.Genres == 0 || (x.Genres & dto.Genres) == dto.Genres)
                .Where(x => IsNullOrEmpty(dto.Director) || x.Director.Contains(dto.Director))
                .Join(_db.FilmScores, f => f.FilmId, fs => fs.FilmId, (f, fs) => new FilmWithScore(f, fs));
        }
    }
}
