using NewsAPIProject.Models;

namespace NewsAPIProject.Repository
{
    public interface IAuthService
    {
        public Task<AuthModel> Registerasync(RegisterModel model);
        public Task<AuthModel> Loginasync(LoginModel model);

        Task<string> Roleasync(RoleModel model);


    }
}
