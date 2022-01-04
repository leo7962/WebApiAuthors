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
    private readonly IDataProtector _dataProtector;

    public AccountsController(UserManager<IdentityUser> userManager, IConfiguration configuration,
        SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtectionProvider,
        HashService hashService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
        _hashService = hashService;
        _dataProtector =
            dataProtectionProvider.CreateProtector("b220ed5967a57820c3b1293bc0a65f2c9f824a0135e1b136c4d1afc1ec291611");
    }

    [HttpGet("hash/{plainText}")]
    public ActionResult MakeHash(string plainText)
    {
        var result1 = _hashService.Hash(plainText);
        var result2 = _hashService.Hash(plainText);

        return Ok(new
        {
            textoPlano = plainText,
            hash1 = result1,
            hash2 = result2
        });
    }

    [HttpGet("Encriptar")]
    public ActionResult Encript()
    {
        var plainText = "Leonardo Hernández";
        var encriptText = _dataProtector.Protect(plainText);
        var desencriptText = _dataProtector.Unprotect(encriptText);

        return Ok(new
        {
            textoPlano = plainText,
            textoCifrado = encriptText,
            textoDescifradp = desencriptText
        });
    }

    [HttpGet("EncriptarPorTiempo")]
    public ActionResult EncriptTime()
    {
        var timeProtector = _dataProtector.ToTimeLimitedDataProtector();
        var plainText = "Leonardo Hernández";
        var encriptText = timeProtector.Protect(plainText, lifetime: TimeSpan.FromSeconds(5));
        var desencriptText = timeProtector.Unprotect(encriptText);

        return Ok(new
        {
            textoPlano = plainText,
            textoCifrado = encriptText,
            textoDescifradp = desencriptText
        });
    }

    [HttpPost("registrar")] //api/cuentas/registrar
    public async Task<ActionResult<AuthenticationResponse>> Register(UserCredential userCredential)
    {
        var user = new IdentityUser {UserName = userCredential.Email, Email = userCredential.Email};
        var result = await _userManager.CreateAsync(user, userCredential.Password);

        if (result.Succeeded) return await BuildToken(userCredential);

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponse>> Login(UserCredential userCredential)
    {
        var result = await _signInManager.PasswordSignInAsync(userCredential.Email, userCredential.Password,
            false, false);
        if (result.Succeeded) return await BuildToken(userCredential);

        return BadRequest("Login incorrecto");
    }

    [HttpGet]
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

    [HttpPost("HacerAdmin")]
    public async Task<ActionResult> CreateAdmin(EditAdminDto editAdminDto)
    {
        var user = await _userManager.FindByEmailAsync(editAdminDto.Email);
        await _userManager.AddClaimAsync(user, new Claim("isAdmin", "1"));
        return NoContent();
    }

    [HttpPost("RemoverAdmin")]
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
