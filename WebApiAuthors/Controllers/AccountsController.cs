using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApiAuthors.Dtos;

namespace WebApiAuthors.Controllers;

[ApiController]
[Route("api/cuentas")]
public class AccountsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountsController(UserManager<IdentityUser> userManager, IConfiguration configuration,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
    }

    [HttpPost("registrar")] //api/cuentas/registrar
    public async Task<ActionResult<AuthenticationResponse>> Register(UserCredential userCredential)
    {
        var user = new IdentityUser {UserName = userCredential.Email, Email = userCredential.Email};
        var result = await _userManager.CreateAsync(user, userCredential.Password);

        if (result.Succeeded) return BuildToken(userCredential);

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponse>> Login(UserCredential userCredential)
    {
        var result = await _signInManager.PasswordSignInAsync(userCredential.Email, userCredential.Password,
            false, false);
        if (result.Succeeded) return BuildToken(userCredential);

        return BadRequest("Login incorrecto");
    }

    private AuthenticationResponse BuildToken(UserCredential userCredential)
    {
        var claim = new List<Claim>
        {
            new("email", userCredential.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["keyjwt"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddHours(2);

        var securityToken = new JwtSecurityToken(null, null, claim, expires: expiration,
            signingCredentials: creds);

        return new AuthenticationResponse()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
            Expiration = expiration
        };
    }
}
