using _13_DangHoangSon_Assignment01_BackEnd.DataAccess;
using _13_DangHoangSon_Assignment01_BackEnd.Models;

namespace _13_DangHoangSon_Assignment01_BackEnd.Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public IQueryable<NewsArticle> GetNewsArticlesQuery(FunewsManagementContext context) => NewsArticleDAO.Instance.GetNewsArticlesQuery(context);
        public Task<List<NewsArticle>> GetAllNewsArticlesAsync() => NewsArticleDAO.Instance.GetAllNewsArticlesAsync();
        public Task<NewsArticle?> GetNewsArticleByIdAsync(string id) => NewsArticleDAO.Instance.GetNewsArticleByIdAsync(id);
        public Task AddNewsArticleAsync(NewsArticle article, List<int> tagIds) => NewsArticleDAO.Instance.AddNewsArticleAsync(article, tagIds);
        public Task UpdateNewsArticleAsync(NewsArticle article, List<int> tagIds) => NewsArticleDAO.Instance.UpdateNewsArticleAsync(article, tagIds);
        public Task<bool> DeleteNewsArticleAsync(string id) => NewsArticleDAO.Instance.DeleteNewsArticleAsync(id);
    }
}