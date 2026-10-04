using _13_DangHoangSon_Assignment01_BackEnd.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace _13_DangHoangSon_Assignment01_BackEnd.Controllers
{
    [Route("api/Tags")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagRepository _tagRepo;

        public TagsController(ITagRepository tagRepo)
        {
            _tagRepo = tagRepo;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tags = await _tagRepo.GetAllTagsAsync();
            return Ok(tags.AsQueryable());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var tag = await _tagRepo.GetTagByIdAsync(id);
            if (tag == null) return NotFound("Không tìm thấy tag.");
            return Ok(tag);
        }
    }
}
