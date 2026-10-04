using _13_DangHoangSon_Assignment01_FontEnd.Models;

namespace _13_DangHoangSon_Assignment01_FontEnd.Services
{
    public interface IApiService
    {
        // Authentication
        Task<LoginResponse?> LoginAsync(string email, string password);

        // System Accounts (Admin)
        Task<List<SystemAccountViewModel>> GetAccountsAsync(string? search = null);
        Task<SystemAccountViewModel?> GetAccountByIdAsync(short id);
        Task<(bool Success, string Message)> CreateAccountAsync(SystemAccountViewModel account);
        Task<(bool Success, string Message)> UpdateAccountAsync(short id, SystemAccountViewModel account);
        Task<(bool Success, string Message)> DeleteAccountAsync(short id);

        // Categories (Staff)
        Task<List<CategoryViewModel>> GetCategoriesAsync(string? search = null);
        Task<CategoryViewModel?> GetCategoryByIdAsync(short id);
        Task<(bool Success, string Message)> CreateCategoryAsync(CategoryViewModel category);
        Task<(bool Success, string Message)> UpdateCategoryAsync(short id, CategoryViewModel category);
        Task<(bool Success, string Message)> DeleteCategoryAsync(short id);

        // News Articles (Staff & Public)
        Task<List<NewsArticleViewModel>> GetNewsArticlesAsync(string? search = null, short? categoryId = null, bool activeOnly = false);
        Task<NewsArticleViewModel?> GetNewsArticleByIdAsync(string id);
        Task<(bool Success, string Message)> CreateNewsArticleAsync(NewsArticleEditViewModel article);
        Task<(bool Success, string Message)> UpdateNewsArticleAsync(string id, NewsArticleEditViewModel article);
        Task<(bool Success, string Message)> DeleteNewsArticleAsync(string id);

        // Tags
        Task<List<TagViewModel>> GetTagsAsync();

        // Report (Admin)
        Task<List<NewsArticleViewModel>> GetReportAsync(DateTime startDate, DateTime endDate);
    }
}
