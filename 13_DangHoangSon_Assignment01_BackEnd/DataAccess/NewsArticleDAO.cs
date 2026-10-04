using _13_DangHoangSon_Assignment01_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace _13_DangHoangSon_Assignment01_BackEnd.DataAccess
{
    public class NewsArticleDAO
    {
        private static NewsArticleDAO? _instance;
        private static readonly object _lock = new object();

        private NewsArticleDAO() { }

        public static NewsArticleDAO Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new NewsArticleDAO();
                    }
                    return _instance;
                }
            }
        }

        public IQueryable<NewsArticle> GetNewsArticlesQuery(FunewsManagementContext context)
        {
            return context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .AsQueryable();
        }

        public async Task<List<NewsArticle>> GetAllNewsArticlesAsync()
        {
            using var context = new FunewsManagementContext();
            return await context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .ToListAsync();
        }

        public async Task<NewsArticle?> GetNewsArticleByIdAsync(string id)
        {
            using var context = new FunewsManagementContext();
            return await context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);
        }

        public async Task AddNewsArticleAsync(NewsArticle article, List<int> tagIds)
        {
            using var context = new FunewsManagementContext();
            if (tagIds.Any())
            {
                var tags = await context.Tags.Where(t => tagIds.Contains(t.TagId)).ToListAsync();
                article.Tags = tags;
            }
            await context.NewsArticles.AddAsync(article);
            await context.SaveChangesAsync();
        }

        public async Task UpdateNewsArticleAsync(NewsArticle article, List<int> tagIds)
        {
            using var context = new FunewsManagementContext();
            var existing = await context.NewsArticles
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == article.NewsArticleId);

            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(article);
                existing.Tags.Clear();
                if (tagIds.Any())
                {
                    var tags = await context.Tags.Where(t => tagIds.Contains(t.TagId)).ToListAsync();
                    existing.Tags = tags;
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteNewsArticleAsync(string id)
        {
            using var context = new FunewsManagementContext();
            var article = await context.NewsArticles
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);

            if (article != null)
            {
                article.Tags.Clear();
                context.NewsArticles.Remove(article);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}