using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.WebAPI;

[Route("[controller]")]
[ApiController]
public class AuthController:ControllerBase
{
    private readonly UserManager<AuthUser>_userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<AuthUser> userManager,RoleManager<IdentityRole>roleManager,IConfiguration configuration){
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]RegisterUserModel registerUserModel){

        try{

            var userExists=await _userManager.FindByEmailAsync(registerUserModel.EmailAddress);
            if(userExists!=null){
                return Conflict(new Response{Status="Error",Message="User already exists"});
            }

            AuthUser user=new AuthUser{
                Email=registerUserModel.EmailAddress,
                SecurityStamp=Guid.NewGuid().ToString(),
                UserName=registerUserModel.UserName,
            };

            var result=await _userManager.CreateAsync(user,registerUserModel.Password);

            if(!result.Succeeded){
                // We can use include to check if the error contains UserName, Email or Password
                string message="";
                foreach(var error in result.Errors){
                    message+=error.Description+'\n';
                }

                return StatusCode(StatusCodes.Status500InternalServerError,new Response{Status="Error", Message=message});
            }

            foreach(var role in registerUserModel.Roles){

                if(!await _roleManager.RoleExistsAsync(role.ToString())){
                    await _roleManager.CreateAsync(new IdentityRole(role.ToString()));
                }

                await _userManager.AddToRoleAsync(user,role.ToString());
            }


            return Ok(new Response{Status="Success",Message="User created successfully"});
        }
        catch(Exception ex){
            return StatusCode(StatusCodes.Status500InternalServerError, new Response{Status="Error",Message="Internal Server Error"});
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginUserModel loginUserModel){
        try{
            var user=await _userManager.FindByEmailAsync(loginUserModel.EmailAddress);
            if(user!=null&& await _userManager.CheckPasswordAsync(user,loginUserModel.Password)){
                var userRole=await _userManager.GetRolesAsync(user);

                var authClaims=new List<Claim>{
                    new(ClaimTypes.Name,user.UserName),
                    new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };

                foreach(var role in userRole){
                    authClaims.Add(new(ClaimTypes.Role,role));
                }

                var authSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token=new JwtSecurityToken(
                    issuer:_configuration["JWT:Issuer"],
                    audience:_configuration["JWT:Audience"],
                    expires:DateTime.Now.AddHours(3),
                    claims:authClaims,
                    signingCredentials:new SigningCredentials(authSigningKey,SecurityAlgorithms.HmacSha256)
                );

                return Ok(new{
                    token=new JwtSecurityTokenHandler().WriteToken(token),
                    expiration=token.ValidTo
                });
            }
            return Unauthorized();
        }
        catch(Exception ex){
            return StatusCode(StatusCodes.Status500InternalServerError, new Response{Status="Error",Message="Internal Server Error"});
        }
    }


    [HttpDelete("{username}")]
    public async Task<IActionResult> DeleteAccount(string username){
        try
        {
            // Retrieve the user by username
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                // User not found
                return NotFound(new { message = "User not found" });
            }

            // Delete the user
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { message = "User deleted successfully" });
            }
            else
            {
                // Handle errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }
        }
        catch (Exception ex)
        {
            
            // Return a 500 status code with a generic error message
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
        }
    }

}


public class Response
{
    public string Status { get; set; }
    public string Message { get; set; }
}