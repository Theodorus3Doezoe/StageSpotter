namespace StageSpotter.Business.Interfaces
{
    public interface IAuthService
    {
        System.Threading.Tasks.Task<int> RegisterAsync(string email, string password);
        System.Threading.Tasks.Task<int> RegisterAsync(string email, string password, StageSpotter.Domain.Models.UserType type, StageSpotter.Data.DTOs.BedrijfDto? bedrijfDto = null);
        System.Threading.Tasks.Task<string> LoginAsync(string email, string password);
    }
}
