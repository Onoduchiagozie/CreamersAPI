using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AdonisAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AdonisAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<CreamUser> _userManager;
    private readonly SignInManager<CreamUser> _signInManager;
    
    public AuthController(
        UserManager<CreamUser> userManager,
        SignInManager<CreamUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    // GET
    [HttpPost("Register")]
    public async Task<IActionResult> Register( RegisterViewModel model)
    {
   
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        CreamUser newUser = new CreamUser
        {
         
            FullName = model.Username,
            Email = model.Email,
            IsSeller = false,
            IsSubscribed = false,
            UserName =  model.Username,
         };
        var result = await _userManager.CreateAsync(newUser, model.Password);

        if (result.Succeeded)
        {
            return Ok("User created successfully.");
        }
        else
        {
           //var errorList=  result.Errors.ToList();
           
           foreach (var error in result.Errors.ToList())
               return BadRequest(error.Description.ToList());
           
        }
        return BadRequest();
         
    }

    
    //CODE DONE AND COMPLETED  FROM BACKNED 
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLogin model)
    {
  
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized("Invalid username or password" );
        
        var tokenHandler = new JwtSecurityTokenHandler();
           
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
              
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
             }),
            Expires = DateTime.UtcNow.AddHours(7),
         //   NotBefore = DateTime.UtcNow.AddHours(-2),
           IssuedAt = DateTime.UtcNow.AddHours(-2),
            SigningCredentials = new SigningCredentials(new 
                    SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes("my_Super_Secret_Key_Here_Must_Not_Be_123, Or, Else")),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
            
        await _signInManager.PasswordSignInAsync(user.UserName,model.Password,true,false);
           return Ok(tokenString);
    }

}

public class UserLogin
{
    public string Username { get; set; }
    public string Password { get; set; }
}

