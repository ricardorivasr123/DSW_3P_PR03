using CharacterHub.DTOs;

namespace CharacterHub.Services;

public interface IAuthService
{
    LoginResponse? Login(string username, string password);
}
