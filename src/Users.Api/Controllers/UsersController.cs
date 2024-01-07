using Microsoft.AspNetCore.Mvc;
using Users.Api.DataTransferObjects;
using Users.Api.Services;

namespace Users.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController(IUserService _service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var users = await _service.GetAllAsync(cancellationToken);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var user = await _service.GetByIdAsync(id,cancellationToken);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDtoForInsertion userDtoForInsertion, CancellationToken cancellationToken)
        {
            var result = await _service.CreateAsync(userDtoForInsertion,cancellationToken);
            return Ok(new { Result = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _service.DeleteAsync(id,cancellationToken);
            return Ok(new { Result = result });
        }
    }
}
