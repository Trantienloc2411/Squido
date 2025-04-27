using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class UserController(IUserService userService, IMapper mapper) : ControllerBase
    {
        [HttpGet("{id}")]
        
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var user = await userService.GetUserByIdAsync(id);
                if(user.IsSuccess) {
                    var result = mapper.Map<UserViewModel>(user.Data);
                    return Ok(result);
                }
                else{
                    return BadRequest(user.Message);
                }
            }
            catch (System.Exception ex)
            {
                
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel userViewModel)
        {
            try
            {
                var userId = Guid.Parse(userViewModel!.Id!.ToString());
                var result = await userService.UpdateUserAsync(userViewModel, userId);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
