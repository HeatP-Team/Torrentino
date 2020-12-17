using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMLProjectX.API
{
    public class Kinopoisk : IStreamingPlatform
    {
        public Kinopoisk()
        {
            
        }

        public string GetLinkByFilmName(string filmName)
        {
            return "https://www.kinopoisk.ru/";
        }
    }
}
