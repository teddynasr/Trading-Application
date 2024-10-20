using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioMicroService.Models;
using PortfolioMicroService.Services;

namespace PortfolioMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserPortfolioController : ControllerBase
    {
        private readonly UserPortfolioService _userPortfolioService;

        public UserPortfolioController(UserPortfolioService userPortfolioService)
        {
            _userPortfolioService = userPortfolioService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserPortfolio>>> GetAllUserPortfolios()
        {
            var userPortfolios = await _userPortfolioService.GetAllUserPortfoliosAsync();
            return Ok(userPortfolios);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserPortfolio>> GetUserPortfolio(Guid userId)
        {
            var userPortfolio = await _userPortfolioService.GetUserPortfolioByUserIdAsync(userId);
            if (userPortfolio == null)
            {
                return NotFound();
            }
            return Ok(userPortfolio);
        }

        [HttpPost]
        public async Task<ActionResult<UserPortfolio>> AddUserPortfolio(UserPortfolio userPortfolio)
        {
            await _userPortfolioService.AddUserPortfolioAsync(userPortfolio);
            return CreatedAtAction(nameof(GetUserPortfolio), new { id = userPortfolio.PortfolioID }, userPortfolio);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserPortfolioByUserIdAsync(Guid userId)
        {
            await _userPortfolioService.UpdateUserPortfolioByUserIdAsync(userId);
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserPortfolio(Guid userId)
        {
            await _userPortfolioService.DeleteUserPortfolioAsync(userId);
            return NoContent();
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            var userPortfolio = await _userPortfolioService.UserPortfolioDeposit(request.UserId, request.DepositAmount);
            return Ok(userPortfolio);
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
        {
            var userPortfolio = await _userPortfolioService.UserPortfolioWithdraw(request.UserId, request.WithdrawAmount);
            return Ok(userPortfolio);
        }
    }
}
