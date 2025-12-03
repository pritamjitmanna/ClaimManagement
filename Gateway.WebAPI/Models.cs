using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Gateway.WebAPI;

/// <summary>
/// - UserRoles: enum representing the various roles that can be assigned to gateway users.
/// - RegisterUserModel: DTO used during registration. Includes DataAnnotations for basic validation (EmailAddress).
/// - LoginUserModel: DTO for login requests (validates email format).
/// - AuthUser: IdentityUser-derived class to allow extension of the identity user if needed in the future.
///
/// Notes:
/// - DataAnnotations (EmailAddress) are used to provide simple, attribute-based validation at model binding time.
/// - Roles list in RegisterUserModel is a list of UserRoles enum values; the controller converts them to strings and ensures the roles exist.
/// </summary>
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
 


