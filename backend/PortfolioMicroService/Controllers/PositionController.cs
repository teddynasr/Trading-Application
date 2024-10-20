using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioMicroService.Models;
using PortfolioMicroService.Services;

namespace PortfolioMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PositionController : ControllerBase
    {
        private readonly PositionService _positionService;

        public PositionController(PositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Position>>> GetAllPositions()
        {
            var positions = await _positionService.GetAllPositionsAsync();
            return Ok(positions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Position>> GetPosition(Guid id)
        {
            var position = await _positionService.GetPositionByIdAsync(id);
            if (position == null)
            {
                return NotFound();
            }
            return Ok(position);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Position>>> GetUserPositions(Guid userId)
        {
            var position = await _positionService.GetPositionsByUserIdAsync(userId);
            if (position == null)
            {
                return NotFound();
            }
            return Ok(position);
        }

        [HttpGet("currencyPair/{currencyPairId}/user/{userId}")]
        public async Task<ActionResult<Position>> GetPositionBycurrencyPairId(Guid currencyPairId, Guid userId)
        {
            var position = await _positionService.GetPositionByCurrencyPairIdAsync(currencyPairId, userId);
            if (position == null)
            {
                return NotFound("This currency isn't currently an open position");
            }
            return Ok(position);
        }

        [HttpGet("currencyPair/{currencyPairId}/user/{userId}/updated")]
        public async Task<ActionResult<Position>> GetUpdatedPositionBycurrencyPairId(Guid currencyPairId, Guid userId)
        {
            var position = await _positionService.GetUpdatedPositionBycurrencyPairIdAsync(currencyPairId, userId);
            if (position == null)
            {
                return NotFound("This currency isn't currently an open position");
            }
            return Ok(position);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePosition(Guid id, [FromBody] Position position)
        {
            if (id != position.PositionID)
            {
                return BadRequest("Position ID mismatch.");
            }

            await _positionService.UpdatePositionAsync(position);

            return NoContent();
        }
    }
}
