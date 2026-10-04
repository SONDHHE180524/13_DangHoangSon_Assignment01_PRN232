using _13_DangHoangSon_Assignment01_BackEnd.Models;

namespace _13_DangHoangSon_Assignment01_BackEnd.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(short id);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(short id);
    }
}