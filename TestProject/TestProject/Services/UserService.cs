using Microsoft.AspNetCore.Identity;
using TestProject.Data;
using TestProject.Models;
using TestProject.Models.DTO;

namespace TestProject.Services;

public class UserService(UserManager<User> userManager, TokenService tokenService, SignInManager<User> signInManager, DataContext dataContext)
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly TokenService _tokenService = tokenService;
    private readonly DataContext _dataContext = dataContext;

    public async Task<string> SignIn(SignInUserDTO dto)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName);

        if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            return await _tokenService.GenerateToken(user);
        }
        else
        {
            throw new UnauthorizedAccessException("Логин или пароль пользователя указаны некорректно");
        }
    }

    public async Task SignUp(SignUpUserDTO dto)
    {
        var userExists = await _userManager.FindByNameAsync(dto.UserName);
        if (userExists != null)
        {
            throw new ApplicationException("Пользователь с таким логином уже существует");
        }

        var user = new User()
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            FullName = dto.UserName,
            UserName = dto.UserName,
        };

        var tr = await _dataContext.Database.BeginTransactionAsync();

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            throw new ApplicationException(result.Errors
                .Select(x => string.Format("{0}:{1}", x.Code, x.Description))
                .Aggregate((current, next) => current + ", " + next));

        await _userManager.AddToRoleAsync(user, UserRole.Student);

        await tr.CommitAsync();
    }
}
