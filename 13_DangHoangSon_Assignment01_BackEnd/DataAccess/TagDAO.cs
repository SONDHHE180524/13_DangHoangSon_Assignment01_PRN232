using _13_DangHoangSon_Assignment01_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace _13_DangHoangSon_Assignment01_BackEnd.DataAccess
{
    public class TagDAO
    {
        private static TagDAO? _instance;
        private static readonly object _lock = new object();

        private TagDAO() { }

        public static TagDAO Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new TagDAO();
                    }
                    return _instance;
                }
            }
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            using var context = new FunewsManagementContext();
            return await context.Tags.ToListAsync();
        }

        public async Task<Tag?> GetTagByIdAsync(int id)
        {
            using var context = new FunewsManagementContext();
            return await context.Tags.FirstOrDefaultAsync(t => t.TagId == id);
        }
    }
}
