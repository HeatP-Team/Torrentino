using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMLProjectX.API
{
    public class Ivi : IStreamingPlatform
    {
        public Ivi()
        {

        }
        public string GetLinkByFilmName(string filmName)
        {
            return "https://www.ivi.ru/";
        }
    }
}
