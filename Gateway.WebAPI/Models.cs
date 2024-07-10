using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Gateway.WebAPI;

public enum UserRoles
{
    Admin,
    InsuranceCompany,
    Surveyor,
    IRDA,
    Insurer
}


public class RegisterUserModel
{
    public string UserName { get; set;}
    [EmailAddress(ErrorMessage ="Enter a valid email address")]
    public string EmailAddress { get; set;}
    public string Password { get; set;}
    public List<UserRoles> Roles { get; set;}
}

public class LoginUserModel
{
    [EmailAddress(ErrorMessage = "The email is invalid")]
    public string EmailAddress { get; set;}
    public string Password { get; set;}
}

public class AuthUser:IdentityUser
{

}




