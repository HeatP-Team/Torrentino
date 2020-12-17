using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UMLProjectX.Models
{
    public class ReviewModel
    {
        [Required(ErrorMessage = "Не указана оценка")]
        public int Score { get; set; }
        public string ReviewText { get; set; }
        public int FilmId { get; set; }
        public string UserLogin { get; set; }
    }
}
