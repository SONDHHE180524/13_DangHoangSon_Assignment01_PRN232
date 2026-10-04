using _13_DangHoangSon_Assignment01_BackEnd.DataAccess;
using _13_DangHoangSon_Assignment01_BackEnd.Models;

namespace _13_DangHoangSon_Assignment01_BackEnd.Repositories
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        public Task<List<SystemAccount>> GetAllAccountsAsync() => SystemAccountDAO.Instance.GetAllAccountsAsync();
        public Task<SystemAccount?> GetAccountByIdAsync(short id) => SystemAccountDAO.Instance.GetAccountByIdAsync(id);
        public Task<SystemAccount?> GetAccountByEmailAsync(string email) => SystemAccountDAO.Instance.GetAccountByEmailAsync(email);
        public Task AddAccountAsync(SystemAccount account) => SystemAccountDAO.Instance.AddAccountAsync(account);
        public Task UpdateAccountAsync(SystemAccount account) => SystemAccountDAO.Instance.UpdateAccountAsync(account);
        public Task<bool> DeleteAccountAsync(short id) => SystemAccountDAO.Instance.DeleteAccountAsync(id);
    }
}