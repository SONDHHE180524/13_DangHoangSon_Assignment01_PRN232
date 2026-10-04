using _13_DangHoangSon_Assignment01_BackEnd.Models;

namespace _13_DangHoangSon_Assignment01_BackEnd.Repositories
{
    public interface ISystemAccountRepository
    {
        Task<List<SystemAccount>> GetAllAccountsAsync();
        Task<SystemAccount?> GetAccountByIdAsync(short id);
        Task<SystemAccount?> GetAccountByEmailAsync(string email);
        Task AddAccountAsync(SystemAccount account);
        Task UpdateAccountAsync(SystemAccount account);
        Task<bool> DeleteAccountAsync(short id);
    }
}