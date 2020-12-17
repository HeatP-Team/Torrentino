using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMLProjectX.DAL.Models
{
    [Flags]
    public enum Genre
    {
        Anime = 1,
        Biography = 2,
        Comedy = 4,
        Drama = 8,
        Fantasy = 16,
        Horror = 32,
        Melodrama = 64,
        ScienceFiction = 128,
        Thriller = 256
    }
}
