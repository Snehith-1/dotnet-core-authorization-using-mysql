using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;



namespace StoryboardAPI.Authorization
{
    public class AuthorizationServerProvide : OAuthAuthorizationServerProvider
    {
        //validateUser objvalidateuser = new validateUser(); 
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            var companyCode = string.Join(",", context.Scope);
            var formData = await context.Request.ReadFormAsync();
            var RouterPrefix = formData.Get("RouterPrefix") ?? "default";
            var Username = formData.Get("username") ?? "default";
            {
                //var status = (Username, context.Password, RouterPrefix, companyCode);
                //if (status == true)
                //{
                //    identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
                //    identity.AddClaim(new Claim("username", "admin"));
                //    identity.AddClaim(new Claim(ClaimTypes.Name, "Admin"));                    
                //    context.Validated(identity);                   
                //}
                //else
                //{                   
                //    context.SetError("invalid_grant", "Provided user credentials are incorrect");
                //    return;
                //}
            }
        }
    }
}