using API_Practice.Model.DTO;

namespace API_Practice.Repository.IRepository
{
    public interface IAuthRepository
    {
        bool IsUniqueUser(string username);

        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);

        UserDTO Register(RegisterationRequestDTO requestDTO);
    }
}
