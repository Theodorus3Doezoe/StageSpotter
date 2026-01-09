using StageSpotter.Domain.Models;

namespace StageSpotter.Data.Interfaces
{
    public interface IUserRepository
    {
        int CreateUser(User user);
        User GetUserByEmail(string email);
    }
}
