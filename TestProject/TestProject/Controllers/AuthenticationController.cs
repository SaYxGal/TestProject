using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TestProject.Models.DTO;
using TestProject.Services;

namespace TestProject.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthenticationController(UserService userService) : ControllerBase
{
    private readonly UserService _userService = userService;

    [HttpPost]
    public async Task<IActionResult> SignIn([FromBody][Required] SignInUserDTO dto)
    {
        return Ok(await _userService.SignIn(dto));
    }

    [HttpPost]
    public async Task<IActionResult> SignUp([FromBody][Required] SignUpUserDTO dto)
    {
        await _userService.SignUp(dto);

        return Ok();
    }
}
