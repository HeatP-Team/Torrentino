using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMLProjectX.DAL.Models
{
    public class FilmScore
    {
        public int FilmScoreId { get; set; }
        public int FilmId { get; set; }
        public int ReviewsNum { get; set; }
        public double AvgScore { get; set; }
    }
}
