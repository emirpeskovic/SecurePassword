using Microsoft.AspNetCore.Mvc;
using SecurePassword.Models.DTO;
using SecurePassword.Services;

namespace SecurePassword.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : Controller
{
    private readonly UserService _userService;
    
    public UserController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost]
    public IActionResult Register([FromBody] RegisterRequest registerRequest)
    {
        var user = _userService.CreateUser(registerRequest);
        return user == null ? BadRequest() : Ok();
    }
    
    [HttpPost]
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {
        var user = _userService.GetUser(loginRequest);
        return user == null ? Unauthorized() : Ok();
    }
}