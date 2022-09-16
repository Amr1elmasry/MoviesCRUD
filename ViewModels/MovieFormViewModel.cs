using MoviesCRUD.Models;
using System.ComponentModel.DataAnnotations;

namespace MoviesCRUD.ViewModels
{
    public class MovieFormViewModel
    {

        [Required, StringLength(50)]
        public string? Title { get; set; }

        public int Year { get; set; }

        [Range(1,10)]
        public double Rate { get; set; }

        [Required, StringLength(2500)]
        public string? Storyline { get; set; }

        [Required]
        public byte[]? Poster { get; set; }

        [Display(Name ="Genre")]
        public byte GenerId { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
    }
}
