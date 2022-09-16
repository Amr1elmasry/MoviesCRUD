using System.ComponentModel.DataAnnotations;

namespace MoviesCRUD.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string? Title { get; set; }

        public int Year { get; set; }

        public double Rate { get; set; }

        [Required, MaxLength(2500)]
        public string? Storyline { get; set; }

        [Required]
        public byte[]? Poster { get; set; }

        public byte GenerId { get; set; }
        public Genre? Genre {get; set; }
    }
}
