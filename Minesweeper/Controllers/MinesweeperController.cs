using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Minesweeper.BusinessLogic.Models;
using Minesweeper.BusinessLogic.Services;
using Minesweeper.Models;
using Minesweeper.Models.Request;
using Minesweeper.Models.Response;

namespace Minesweeper.Controllers
{
    /// <summary>
    /// Minesweeper
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MinesweeperController(IMinesweeperService _service,
                                       IMapper             _mapper) : ControllerBase
    {

        [HttpPost("new")]
        public IActionResult CreateGame([FromBody] NewGameRequest request)
        {
            try
            {
                var game = _service.CreateGame(_mapper.Map<NewGameDto>(request));
                return Ok(_mapper.Map<GameInfoResponse>(game));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse { Error = ex.Message });
            }
        }

        [HttpPost("turn")]
        public IActionResult MakeMove([FromBody] GameTurnRequest request)
        {
            try
            {
                var game = _service.MakeMove(_mapper.Map<GameTurnDto>(request));
                return Ok(_mapper.Map<GameInfoResponse>(game));
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(new ErrorResponse { Error = ex.Message });
            }
        }
    }
}
