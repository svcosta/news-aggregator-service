using Acme.NewsAggregator.Application.Dtos;
using Acme.NewsAggregator.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Acme.NewsAggregator.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        private readonly INewsAggregatorService _service;

        public StoriesController(INewsAggregatorService service) => _service = service;

        [HttpGet("best/{n}")]
        [ProducesResponseType(typeof(IEnumerable<StoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBestStories(int n)
        {
            if (n <= 0) return BadRequest("Count must be greater than zero.");

            var stories = await _service.GetBestStoriesAsync(n);
            return Ok(stories);
        }

    }
}
