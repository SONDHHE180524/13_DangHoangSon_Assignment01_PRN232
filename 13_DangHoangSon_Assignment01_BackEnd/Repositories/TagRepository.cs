using _13_DangHoangSon_Assignment01_BackEnd.DataAccess;
using _13_DangHoangSon_Assignment01_BackEnd.Models;

namespace _13_DangHoangSon_Assignment01_BackEnd.Repositories
{
    public class TagRepository : ITagRepository
    {
        public Task<List<Tag>> GetAllTagsAsync() => TagDAO.Instance.GetAllTagsAsync();
        public Task<Tag?> GetTagByIdAsync(int id) => TagDAO.Instance.GetTagByIdAsync(id);
    }
}
