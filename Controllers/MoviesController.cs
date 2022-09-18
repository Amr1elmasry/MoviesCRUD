using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesCRUD.Models;
using MoviesCRUD.ViewModels;

namespace MoviesCRUD.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private List<string>_allowedExtentions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.ToListAsync();
            return View(movies);
        }

        public async Task<IActionResult> Create()
        {
            var viewmodel = new MovieFormViewModel
            {
                Genres = await _context.Genres.OrderBy(m=>m.Name).ToListAsync()
            };

            return View("MovieForm",viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("MovieForm", model);
            }
            var files = Request.Form.Files;
            if (!files.Any())
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please Select Poster image");
                return View("MovieForm", model);

            }
            var poster = files.FirstOrDefault();
            

            if (!_allowedExtentions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only .jpg or .png are allowed");
                return View("MovieForm", model);
            }
            if (poster.Length > _maxAllowedPosterSize)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Poster is more than 1 mb");
                return View("MovieForm", model);
            }
            using var dataStream = new MemoryStream();
            await poster.CopyToAsync(dataStream);
            var movies = new Movie
            {
                Title = model.Title,
                GenerId = model.GenerId,
                Year = model.Year,
                Rate = model.Rate,
                Storyline = model.Storyline,
                Poster = dataStream.ToArray()

            };
            _context.Movies.Add(movies);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _context.Movies.FindAsync(id);

            if(movie==null)
                return NotFound();

            var viewmodel = new MovieFormViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                GenerId=movie.GenerId,
                Rate = movie.Rate,
                Poster=movie.Poster,
                Year = movie.Year,
                Storyline=movie.Storyline,
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
            };

            return View("MovieForm", viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                
            }

            var movie = await _context.Movies.FindAsync(model.Id);

            if (movie == null)
                return NotFound();

            var files = Request.Form.Files;

            if (files.Any())
            {
                var poster = files.FirstOrDefault();
                using var dataStream = new MemoryStream();
                await poster.CopyToAsync(dataStream);

                model.Poster = dataStream.ToArray();

                if (!_allowedExtentions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only .jpg or .png are allowed");
                    return View("MovieForm", model);
                }
                if (poster.Length > _maxAllowedPosterSize)
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Poster is more than 1 mb");
                    return View("MovieForm", model);
                }

                movie.Poster = model.Poster;
            }

            movie.Title = model.Title;
            movie.Year = model.Year;
            movie.Storyline = model.Storyline;
            movie.Rate = model.Rate;
            movie.GenerId = model.GenerId;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));

        }
    }
}
