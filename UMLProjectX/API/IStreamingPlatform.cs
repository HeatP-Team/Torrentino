using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMLProjectX.API
{
    interface IStreamingPlatform
    {
        string GetLinkByFilmName(string filmName);
    }
}
