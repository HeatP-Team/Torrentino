using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMLProjectX.Tools
{
    public class FilmSearcherDto
    {
        public string Name { get; set; }
        public int? Year { get; set; }
        public int? Genres { get; set; }
        public string Director { get; set; }
    }
}
