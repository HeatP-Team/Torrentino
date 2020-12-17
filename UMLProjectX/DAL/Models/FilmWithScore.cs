using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMLProjectX.DAL.Models
{
    public class FilmWithScore
    {
        public Film Film { get; set; }
        public FilmScore Score { get; set; }

        public FilmWithScore(Film film, FilmScore filmScore)
        {
            Film = film;
            Score = filmScore;
        }
    }
}
