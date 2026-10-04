using _13_DangHoangSon_Assignment01_BackEnd.Models;

namespace _13_DangHoangSon_Assignment01_BackEnd.Repositories
{
    public interface INewsArticleRepository
    {
        IQueryable<NewsArticle> GetNewsArticlesQuery(FunewsManagementContext context);
        Task<List<NewsArticle>> GetAllNewsArticlesAsync();
        Task<NewsArticle?> GetNewsArticleByIdAsync(string id);
        Task AddNewsArticleAsync(NewsArticle article, List<int> tagIds);
        Task UpdateNewsArticleAsync(NewsArticle article, List<int> tagIds);
        Task<bool> DeleteNewsArticleAsync(string id);
    }
}