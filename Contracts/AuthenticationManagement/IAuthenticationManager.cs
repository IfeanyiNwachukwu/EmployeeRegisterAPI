using Entities.DataTransferObjects.Writable.Creatable;
using System.Threading.Tasks;

namespace Contracts.AuthenticationManagement
{
    public interface IAuthenticationManager
    {
        Task<bool> ValidateUser(UserForAuthenticationDTOW userForAuthentication);
        Task<string> CreateToken();
    }
}
