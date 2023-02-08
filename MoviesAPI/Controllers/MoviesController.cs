using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private new List<string> _allowedExtentions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;
        private readonly IMoviesService _moviesService;
        private readonly IGenresService _genresService;
        private readonly IMapper _mapper;

        public MoviesController(IMoviesService moviesService, IGenresService genresService, IMapper mapper)
        {
            this._moviesService = moviesService;
            this._genresService = genresService;
            this._mapper = mapper;
        }


        #region GetById
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _moviesService.GetById(id);

            if (movie == null)
            {
                return NotFound("No Movie Found with This ID!");
            }

            var dto = _mapper.Map<MovieDetailsDto>(movie);
            return Ok(dto);
        }
        #endregion

        #region GetAll
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _moviesService.GetAll();
            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
            return Ok(data);
        }
        #endregion

        #region GetByGenreId
        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var movies = await _moviesService.GetAll(genreId);

            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
            return Ok(data);
        }
        #endregion

        #region Create Movie
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
            if (dto.Poster == null)
            {
                return BadRequest("Poster is required!");
            }

            if (!_allowedExtentions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
            {
                return BadRequest("Only .png, .jpg are Allowed!");
            }
            if (dto.Poster.Length > _maxAllowedPosterSize)
            {
                return BadRequest("maximum file size is 1 MB !");
            }

            var isValidGenre = await _genresService.IsvalidGenre(dto.GenreId);
            if (!isValidGenre)
            {
                return BadRequest($"Invalid Genre ID!");
            }

            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);

            var movie = _mapper.Map<Movie>(dto);
            movie.Poster = dataStream.ToArray();

            _moviesService.Add(movie);
            return Ok(movie);
        }
        #endregion

        #region UpdateMovie

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDto dto)
        {
            var movie = await _moviesService.GetById(id);
            if (movie == null)
            {
                return NotFound($"No Movie was Found with ID{id}");
            }

            var isValidGenre = await _genresService.IsvalidGenre(dto.GenreId);
            if (!isValidGenre)
            {
                return BadRequest($"Invalid Genre ID!");
            }
            if (dto.Poster != null)
            {
                if (!_allowedExtentions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                {
                    return BadRequest("Only .png, .jpg are Allowed!");
                }
                if (dto.Poster.Length > _maxAllowedPosterSize)
                {
                    return BadRequest("maximum file size is 1 MB !");
                }

                using var dataStream = new MemoryStream();
                await dto.Poster.CopyToAsync(dataStream);

                movie.Poster = dataStream.ToArray();
            }
            movie.Title = dto.Title;
            movie.StoreLine = dto.StoreLine;
            movie.Rate = dto.Rate;
            movie.Year = dto.Year;
            movie.GenreId = dto.GenreId;


            _moviesService.Update(movie);
            return Ok(movie);
        }
        #endregion

        #region DeleteMovie

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _moviesService.GetById(id);

            if (movie == null)
            {
                return NotFound($"No Movie was Found with ID{id}");
            }

            _moviesService.Delete(movie);
            return Ok(movie);
        }

        #endregion
    }
}
