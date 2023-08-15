using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NewsAPIProject.Helpers;
using NewsAPIProject.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NewsAPIProject.Repository
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly JWT _jwt;

        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
        }



        public async Task<AuthModel> Registerasync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email Is Already Registered!" };
            if (await _userManager.FindByNameAsync(model.username) is not null)
                return new AuthModel { Message = "username Is Already Registered!" };

            var newUser = new User
            {
                UserName = model.username,
                Email = model.Email,
                firstName = model.FirstName,
                lastName = model.LastName,
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} ,";
                }
                return new AuthModel { Message = errors };
            }

            await _userManager.AddToRoleAsync(newUser, "admin");
            var jwtSecurityToken = await createJwtToken(newUser);

            return new AuthModel
            {
                Email = newUser.Email,
                Expire = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "admin" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),

            };
        }

        public async Task<AuthModel> Loginasync(LoginModel model)
        {
            var authmodel = new AuthModel();
            authmodel.Roles = new List<string>();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authmodel.Message = "Email Or Password is incorrect";
                return authmodel;
            }
            if (await _userManager.IsInRoleAsync(user, "admin"))
            {
                authmodel.Roles.Add("admin");
            }

            var jwtSecurityToken = await createJwtToken(user);
            var rolemodel = await _userManager.GetRolesAsync(user);


            authmodel.IsAuthenticated = true;
            authmodel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authmodel.Expire = jwtSecurityToken.ValidTo;
            authmodel.Email = user.Email;
            authmodel.Roles = rolemodel.ToList();

            return authmodel;
        }
        public async Task<string> Roleasync(RoleModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.UserId);
            if (user == null || !await _roleManager.RoleExistsAsync(model.UserId))
            {
                return "Invalid User Id or Role ";
            }
            if (await _userManager.IsInRoleAsync(user, model.RoleName))
            {
                return "User Already assigned to this role";
            }
            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            return result.Succeeded ? string.Empty : "somthing went wrong";

        }

        private async Task<JwtSecurityToken> createJwtToken(User user)
        {

            var userclaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleclaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleclaims.Add(new Claim("roles", role));
            }

            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("uid",user.Id),
                    new Claim("uname", user.UserName),

            }
            .Union(roleclaims)
            .Union(userclaims);

            var symetricSecKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symetricSecKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials
                );
            ;
            return jwtSecurityToken;
        }

    }
}
