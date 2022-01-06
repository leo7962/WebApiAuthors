using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApiAuthors.Dtos;
using WebApiAuthors.Services;

namespace WebApiAuthors.Controllers;

[ApiController]
[Route("api/cuentas")]
public class AccountsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly HashService _hashService;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountsController(UserManager<IdentityUser> userManager, IConfiguration configuration,
        SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtectionProvider,
        HashService hashService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
        _hashService = hashService;
    }

    [HttpPost("registrar",Name = "registrarUsuario")] //api/cuentas/registrar
    public async Task<ActionResult<AuthenticationResponse>> Register(UserCredential userCredential)
    {
        var user = new IdentityUser {UserName = userCredential.Email, Email = userCredential.Email};
        var result = await _userManager.CreateAsync(user, userCredential.Password);

        if (result.Succeeded) return await BuildToken(userCredential);

        return BadRequest(result.Errors);
    }

    [HttpPost("login",Name = "loginUsuario")]
    public async Task<ActionResult<AuthenticationResponse>> Login(UserCredential userCredential)
    {
        var result = await _signInManager.PasswordSignInAsync(userCredential.Email, userCredential.Password,
            false, false);
        if (result.Succeeded) return await BuildToken(userCredential);

        return BadRequest("Login incorrecto");
    }

    [HttpGet("ReonvarToken", Name = "renovarToken")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<AuthenticationResponse>> Renew()
    {
        var emailClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "email");
        var email = emailClaim?.Value;

        var credsUser = new UserCredential()
        {
            Email = email
        };

        return await BuildToken(credsUser);
    }

    [HttpPost("HacerAdmin", Name = "hacerAdmin")]
    public async Task<ActionResult> CreateAdmin(EditAdminDto editAdminDto)
    {
        var user = await _userManager.FindByEmailAsync(editAdminDto.Email);
        await _userManager.AddClaimAsync(user, new Claim("isAdmin", "1"));
        return NoContent();
    }

    [HttpPost("RemoverAdmin",Name = "removerAdmin")]
    public async Task<ActionResult> DeleteAdmin(EditAdminDto editAdminDto)
    {
        var user = await _userManager.FindByEmailAsync(editAdminDto.Email);
        await _userManager.RemoveClaimAsync(user, new Claim("isAdmin", "1"));
        return NoContent();
    }

    private async Task<AuthenticationResponse> BuildToken(UserCredential userCredential)
    {
        var claim = new List<Claim>
        {
            new("email", userCredential.Email)
        };

        var user = await _userManager.FindByEmailAsync(userCredential.Email);
        var claimsDb = await _userManager.GetClaimsAsync(user);

        claim.AddRange(claimsDb);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["keyjwt"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddMinutes(30);

        var securityToken = new JwtSecurityToken(null, null, claim, expires: expiration,
            signingCredentials: creds);

        return new AuthenticationResponse()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
            Expiration = expiration
        };
    }
}
