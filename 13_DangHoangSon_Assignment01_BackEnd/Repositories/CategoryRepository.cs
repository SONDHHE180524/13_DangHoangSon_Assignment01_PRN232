using _13_DangHoangSon_Assignment01_BackEnd.DataAccess;
using _13_DangHoangSon_Assignment01_BackEnd.Models;

namespace _13_DangHoangSon_Assignment01_BackEnd.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public Task<List<Category>> GetAllCategoriesAsync() => CategoryDAO.Instance.GetAllCategoriesAsync();
        public Task<Category?> GetCategoryByIdAsync(short id) => CategoryDAO.Instance.GetCategoryByIdAsync(id);
        public Task AddCategoryAsync(Category category) => CategoryDAO.Instance.AddCategoryAsync(category);
        public Task UpdateCategoryAsync(Category category) => CategoryDAO.Instance.UpdateCategoryAsync(category);
        public Task<bool> DeleteCategoryAsync(short id) => CategoryDAO.Instance.DeleteCategoryAsync(id);
    }
}