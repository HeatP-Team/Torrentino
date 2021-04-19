using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMLProjectX.DAL.Models
{
    public class KinozalLink
    {
        public int KinozalLinkId { get; set; }

        public int FilmId { get; set; }

        public Film Film { get; set; }

        public string Link { get; set; }

        public string Size { get; set; }

        public string Quality { get; set; }
    }
}
