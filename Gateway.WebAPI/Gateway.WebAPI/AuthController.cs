using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.WebAPI;

/// <summary>
/// Controller that handles authentication and account management for the gateway.
/// - POST /register: creates a new user, ensures roles exist, and assigns roles to the user.
/// - POST /login: validates user credentials and returns a JWT token and expiration when successful.
/// - DELETE /{username}: deletes an existing user by username.
/// 
/// Notes on key APIs used:
/// - UserManager&lt;T&gt;: ASP.NET Core Identity service for creating, finding and deleting users.
/// - RoleManager&lt;T&gt;: manages roles; CreateAsync ensures roles exist before assigning them.
/// - CreateAsync(user, password): hashes and saves a new user with the supplied password.
/// - CheckPasswordAsync: verifies a password against the stored hash.
/// - JwtSecurityToken / JwtSecurityTokenHandler: used to create and serialize JWT bearer tokens.
/// - Token contains claims for user name and roles to support authorization checks in downstream services.
/// - The controller returns structured messages (Response) for clarity; on unexpected errors it returns generic 500 responses to avoid leaking sensitive info.
/// </summary>
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

    /// <summary>
    /// Registers a new user with the provided details.
    /// </summary>
    /// <param name="registerUserModel">The registration details.</param>
    /// <returns>A response indicating the success or failure of the registration.</returns>
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

                throw new Exception(message);
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
            return StatusCode(StatusCodes.Status500InternalServerError, new Response{Status="Error",Message=ex.Message});
        }
    }


    /// <summary>
    /// Authenticates a user and returns a JWT token if successful.
    /// </summary>
    /// <param name="loginUserModel">The login credentials.</param>
    /// <returns>A JWT token and its expiration time on successful login; Unauthorized otherwise.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginUserModel loginUserModel){
        try{
            var user=await _userManager.FindByEmailAsync(loginUserModel.EmailAddress);
            // Console.WriteLine(user);
            if(user!=null&& await _userManager.CheckPasswordAsync(user,loginUserModel.Password)){
                var userRole=await _userManager.GetRolesAsync(user);

                var authClaims=new List<Claim>{
                    new(ClaimTypes.Name,user.UserName),
                    new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new("profileSet",user.profileSet.ToString()),
                    new("profileId",user.profileId.ToString()!)
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


    /// <summary>
    /// Deletes a user account by username.
    /// </summary>
    /// <param name="username">The username of the account to delete.</param>
    /// <returns>A response indicating the success or failure of the deletion.</returns>
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