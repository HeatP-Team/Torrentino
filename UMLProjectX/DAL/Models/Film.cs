using System.Collections.Generic;

namespace UMLProjectX.DAL.Models
{
    public class Film
    {
        public int FilmId { get; set; }
        public string RusName { get; set; }
        public int Year { get; set; }
        public int Genres { get; set; }
        public string Director { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public virtual ICollection<Review> Review { get; } = new List<Review>();

        public virtual ICollection<KinozalLink> KinozalLinks { get; } = new List<KinozalLink>();
    }
}
