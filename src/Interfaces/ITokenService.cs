
using api.src.Models.User;

namespace api.src.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}