using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenresService _genresService;

        public GenresController(IGenresService genresService)
        {
            this._genresService = genresService;
        }

        #region Get All
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await _genresService.GetAll();
            return Ok(genres);
        }
        #endregion

        #region Create Genre
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] GenreDto dto)
        {
            var genre = new Genre { Name = dto.Name };
            await _genresService.Add(genre);
            return Ok(genre);
        }
        #endregion

        #region Update Genre with ID
        [HttpPut("{id}")]
        //api/Genres/1
        public async Task<IActionResult> UpdateAsync(byte id, [FromBody] GenreDto dto)
        {
            var genre = await _genresService.GetById(id);
            if (genre == null)
            {
                return NotFound($"No genre was found with ID:{id}");
            }

            genre.Name = dto.Name;
            _genresService.Update(genre);
            return Ok(genre);
        }
        #endregion

        #region Delete Genre with ID

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(byte id)
        {
            var genre = await _genresService.GetById(id);
            if (genre == null)
            {
                return NotFound($"No genre was found with ID:{id}");
            }

            _genresService.Delete(genre);

            return Ok(genre);
        }

        #endregion
    }
}
