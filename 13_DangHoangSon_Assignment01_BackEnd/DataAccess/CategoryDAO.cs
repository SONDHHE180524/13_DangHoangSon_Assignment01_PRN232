using _13_DangHoangSon_Assignment01_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace _13_DangHoangSon_Assignment01_BackEnd.DataAccess
{
    public class CategoryDAO
    {
        private static CategoryDAO? _instance;
        private static readonly object _lock = new object();

        private CategoryDAO() { }

        public static CategoryDAO Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CategoryDAO();
                    }
                    return _instance;
                }
            }
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            using var context = new FunewsManagementContext();
            return await context.Categories.Include(c => c.ParentCategory).ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(short id)
        {
            using var context = new FunewsManagementContext();
            return await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task AddCategoryAsync(Category category)
        {
            using var context = new FunewsManagementContext();
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            using var context = new FunewsManagementContext();
            var existing = await context.Categories.FindAsync(category.CategoryId);
            if (existing != null)
            {
                existing.CategoryName = category.CategoryName;
                existing.CategoryDesciption = category.CategoryDesciption;
                existing.ParentCategoryId = category.ParentCategoryId;
                existing.IsActive = category.IsActive;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteCategoryAsync(short id)
        {
            using var context = new FunewsManagementContext();
            // Đề bài: Không thể xóa Category nếu đã có bài viết thuộc về Category này
            bool hasArticles = await context.NewsArticles.AnyAsync(n => n.CategoryId == id);
            if (hasArticles)
            {
                return false;
            }

            var cat = await context.Categories.FindAsync(id);
            if (cat != null)
            {
                context.Categories.Remove(cat);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}