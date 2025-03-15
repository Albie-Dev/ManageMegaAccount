using Microsoft.AspNetCore.Mvc;
using MMA.Domain;
using MMA.Service;

namespace MMA.API
{
    [ApiController]
    [Route("/api/v1/timer")]
    public class TimerController : ControllerBase
    {
        private readonly IDbRepository _dbRepository;
        private readonly ILogger<TimerController> _logger;

        public TimerController(
            IDbRepository dbRepository,
            ILogger<TimerController> logger
        )
        {
            _dbRepository = dbRepository;
            _logger = logger;
        }


        [HttpGet("run-add-movie")]
        public async Task<IActionResult> RunAddMovie()
        {
            Guid a = new Guid("00000000-0000-0000-0000-000000000000");
            var result = await _dbRepository.GetAsync<MovieEntity>(s => s.ActorIds.Contains(a));
            return Ok(result);
        }
    }
}