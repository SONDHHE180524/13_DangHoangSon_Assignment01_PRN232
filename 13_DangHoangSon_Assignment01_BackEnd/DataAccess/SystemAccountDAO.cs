using _13_DangHoangSon_Assignment01_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace _13_DangHoangSon_Assignment01_BackEnd.DataAccess
{
    public class SystemAccountDAO
    {
        private static SystemAccountDAO? _instance;
        private static readonly object _lock = new object();

        private SystemAccountDAO() { }

        public static SystemAccountDAO Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SystemAccountDAO();
                    }
                    return _instance;
                }
            }
        }

        public async Task<List<SystemAccount>> GetAllAccountsAsync()
        {
            using var context = new FunewsManagementContext();
            return await context.SystemAccounts.ToListAsync();
        }

        public async Task<SystemAccount?> GetAccountByIdAsync(short id)
        {
            using var context = new FunewsManagementContext();
            return await context.SystemAccounts.FirstOrDefaultAsync(a => a.AccountId == id);
        }

        public async Task<SystemAccount?> GetAccountByEmailAsync(string email)
        {
            using var context = new FunewsManagementContext();
            return await context.SystemAccounts.FirstOrDefaultAsync(a => a.AccountEmail == email);
        }

        public async Task AddAccountAsync(SystemAccount account)
        {
            using var context = new FunewsManagementContext();
            if (account.AccountId == 0)
            {
                short maxId = await context.SystemAccounts.AnyAsync()
                    ? await context.SystemAccounts.MaxAsync(a => a.AccountId)
                    : (short)0;
                account.AccountId = (short)(maxId + 1);
            }
            await context.SystemAccounts.AddAsync(account);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAccountAsync(SystemAccount account)
        {
            using var context = new FunewsManagementContext();
            var existing = await context.SystemAccounts.FindAsync(account.AccountId);
            if (existing != null)
            {
                existing.AccountName = account.AccountName;
                existing.AccountEmail = account.AccountEmail;
                existing.AccountRole = account.AccountRole;
                existing.AccountPassword = account.AccountPassword;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteAccountAsync(short id)
        {
            using var context = new FunewsManagementContext();
            // Đề bài: Không thể xóa tài khoản nếu đã từng tạo bài viết tin tức
            bool hasArticles = await context.NewsArticles.AnyAsync(n => n.CreatedById == id);
            if (hasArticles)
            {
                return false;
            }

            var account = await context.SystemAccounts.FindAsync(id);
            if (account != null)
            {
                context.SystemAccounts.Remove(account);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}