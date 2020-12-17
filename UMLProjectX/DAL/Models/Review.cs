using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMLProjectX.DAL.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public DateTime Time { get; set; }
        public int Score { get; set; }
        public string ReviewText { get; set; }
        public int UserId { get; set; }
        public string UserLogin { get; set; }
        public User User { get; set; }
        public int FilmId { get; set; }
        public string FilmName { get; set; }
        public Film Film { get; set; }
    }
}
